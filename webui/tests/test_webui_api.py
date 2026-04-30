"""Unit tests for the WebUI backend REST API.

These tests use FastAPI's TestClient (sync, no real HTTP) with mocked
admin_client calls and isolated temp directories for config files.
No running DS or Admin plugin is required.

Usage:
    cd webui
    uv run pytest tests/test_webui_api.py -v
"""

import xml.etree.ElementTree as ET
from pathlib import Path
from unittest.mock import AsyncMock, patch

import pytest

from app.models.dedicated_server import (
    ChatMessage,
    DedicatedConfig,
    PlayerInfo,
    ServerState,
    SessionSettings,
)
from app.models.pulsar import (
    CoreConfig,
    GitHubPluginConfig,
    LocalFolderConfig,
    Profile,
    SourcesConfig,
    RemoteHubConfig,
    LocalPluginConfig,
    ModConfig,
)


# ═══════════════════════════════════════════════════════════════════
#  Server config (DS XML) — no mocking needed, uses real file I/O
# ═══════════════════════════════════════════════════════════════════

class TestServerConfig:

    def test_get_default_config(self, client):
        resp = client.get("/api/server/config")
        assert resp.status_code == 200
        data = resp.json()
        assert data["server_name"] == ""
        assert data["server_port"] == 27016
        assert data["steam_port"] == 8766

    def test_put_config_roundtrip(self, client):
        config = DedicatedConfig(
            server_name="TestServer",
            server_port=27017,
            steam_port=8767,
            ip="192.168.1.100",
            administrators=["76561198000000001"],
            banned=[76561198000000002],
            session_settings=SessionSettings(
                max_players=16,
                inventory_size_multiplier=10.0,
                enable_ingame_scripts=True,
            ),
        )
        resp = client.put("/api/server/config", json=config.model_dump())
        assert resp.status_code == 200
        assert resp.json()["server_name"] == "TestServer"

        resp2 = client.get("/api/server/config")
        data = resp2.json()
        assert data["server_name"] == "TestServer"
        assert data["server_port"] == 27017
        assert data["ip"] == "192.168.1.100"
        assert data["session_settings"]["max_players"] == 16
        assert data["session_settings"]["inventory_size_multiplier"] == 10.0

    def test_put_config_preserves_session_settings(self, client):
        config = DedicatedConfig(
            session_settings=SessionSettings(
                game_mode=1,
                total_pcu=400000,
                enable_economy=True,
                weather_system=False,
            ),
        )
        client.put("/api/server/config", json=config.model_dump())
        data = client.get("/api/server/config").json()
        ss = data["session_settings"]
        assert ss["game_mode"] == 1
        assert ss["total_pcu"] == 400000
        assert ss["enable_economy"] is True
        assert ss["weather_system"] is False

    def test_config_administrators_list(self, client):
        config = DedicatedConfig(
            administrators=["76561198000000001", "76561198000000002"],
        )
        client.put("/api/server/config", json=config.model_dump())
        data = client.get("/api/server/config").json()
        assert len(data["administrators"]) == 2

    def test_config_banned_list(self, client):
        config = DedicatedConfig(banned=[111, 222, 333])
        client.put("/api/server/config", json=config.model_dump())
        data = client.get("/api/server/config").json()
        assert data["banned"] == [111, 222, 333]

    def test_config_plugins_list(self, client):
        config = DedicatedConfig(plugins=["Plugin.A", "Plugin.B"])
        client.put("/api/server/config", json=config.model_dump())
        data = client.get("/api/server/config").json()
        assert data["plugins"] == ["Plugin.A", "Plugin.B"]

    def test_config_auto_restart_fields(self, client):
        config = DedicatedConfig(
            auto_restart_enabled=True,
            auto_restart_time_in_min=120,
            auto_restart_save=False,
        )
        client.put("/api/server/config", json=config.model_dump())
        data = client.get("/api/server/config").json()
        assert data["auto_restart_enabled"] is True
        assert data["auto_restart_time_in_min"] == 120
        assert data["auto_restart_save"] is False


# ═══════════════════════════════════════════════════════════════════
#  Saved worlds listing
# ═══════════════════════════════════════════════════════════════════

class TestServerWorlds:

    def test_no_worlds(self, client):
        resp = client.get("/api/server/worlds")
        assert resp.status_code == 200
        assert resp.json() == []

    def test_list_worlds(self, client, tmp_dirs):
        ds_dir, _ = tmp_dirs
        saves = ds_dir / "Saves"
        saves.mkdir()
        world = saves / "TestWorld"
        world.mkdir()
        (world / "Sandbox.sbc").write_text("<xml/>")
        (world / "data.vx2").write_bytes(b"\x00" * 1024)

        resp = client.get("/api/server/worlds")
        assert resp.status_code == 200
        worlds = resp.json()
        assert len(worlds) == 1
        assert worlds[0]["name"] == "TestWorld"
        assert worlds[0]["size_mb"] >= 0

    def test_worlds_sorted_by_name(self, client, tmp_dirs):
        ds_dir, _ = tmp_dirs
        saves = ds_dir / "Saves"
        saves.mkdir()
        for name in ["Charlie", "Alpha", "Bravo"]:
            d = saves / name
            d.mkdir()
            (d / "Sandbox.sbc").write_text("<xml/>")

        worlds = client.get("/api/server/worlds").json()
        names = [w["name"] for w in worlds]
        assert names == ["Alpha", "Bravo", "Charlie"]


# ═══════════════════════════════════════════════════════════════════
#  Server live endpoints (proxied to Admin plugin — mocked)
# ═══════════════════════════════════════════════════════════════════

MOCK_STATE = ServerState(
    is_running=True,
    server_name="MockServer",
    players_online=3,
    max_players=16,
    sim_speed=1.0,
    uptime_seconds=3600,
)
MOCK_PLAYERS = [
    PlayerInfo(steam_id=111, display_name="Alice", faction="FAC", is_admin=True, ping_ms=20),
    PlayerInfo(steam_id=222, display_name="Bob", faction="", is_admin=False, ping_ms=45),
]
MOCK_CHAT = [
    ChatMessage(timestamp="12:00:00", sender="Alice", message="Hello"),
    ChatMessage(timestamp="12:00:05", sender="Bob", message="Hi there"),
]


class TestServerLiveState:

    @patch("app.services.admin_client.get_server_state", new_callable=AsyncMock)
    def test_get_state(self, mock_fn, client):
        mock_fn.return_value = MOCK_STATE
        resp = client.get("/api/server/state")
        assert resp.status_code == 200
        data = resp.json()
        assert data["isRunning"] is True
        assert data["serverName"] == "MockServer"
        assert data["playersOnline"] == 3

    @patch("app.services.admin_client.get_server_state", new_callable=AsyncMock)
    def test_state_offline(self, mock_fn, client):
        mock_fn.return_value = ServerState(is_running=False)
        data = client.get("/api/server/state").json()
        assert data["isRunning"] is False


class TestServerPlayers:

    @patch("app.services.admin_client.get_players", new_callable=AsyncMock)
    def test_get_players(self, mock_fn, client):
        mock_fn.return_value = MOCK_PLAYERS
        resp = client.get("/api/server/players")
        assert resp.status_code == 200
        players = resp.json()
        assert len(players) == 2
        assert players[0]["displayName"] == "Alice"
        assert players[0]["isAdmin"] is True

    @patch("app.services.admin_client.get_players", new_callable=AsyncMock)
    def test_empty_players(self, mock_fn, client):
        mock_fn.return_value = []
        assert client.get("/api/server/players").json() == []


class TestServerChat:

    @patch("app.services.admin_client.get_chat", new_callable=AsyncMock)
    def test_get_chat(self, mock_fn, client):
        mock_fn.return_value = MOCK_CHAT
        resp = client.get("/api/server/chat")
        assert resp.status_code == 200
        chat = resp.json()
        assert len(chat) == 2
        assert chat[0]["sender"] == "Alice"

    @patch("app.services.admin_client.get_chat", new_callable=AsyncMock)
    def test_get_chat_with_count(self, mock_fn, client):
        mock_fn.return_value = MOCK_CHAT[:1]
        resp = client.get("/api/server/chat", params={"count": 1})
        assert resp.status_code == 200
        mock_fn.assert_called_once_with(1)

    @patch("app.services.admin_client.send_chat", new_callable=AsyncMock)
    def test_send_chat_success(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/chat", json={"message": "Hello!"})
        assert resp.status_code == 200
        assert resp.json()["status"] == "sent"

    @patch("app.services.admin_client.send_chat", new_callable=AsyncMock)
    def test_send_chat_failure(self, mock_fn, client):
        mock_fn.return_value = False
        resp = client.post("/api/server/chat", json={"message": "Hello!"})
        assert resp.json()["status"] == "failed"


class TestServerActions:

    @patch("app.services.admin_client.save_world", new_callable=AsyncMock)
    def test_save(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/save")
        assert resp.json()["status"] == "saved"

    @patch("app.services.admin_client.save_world", new_callable=AsyncMock)
    def test_save_failure(self, mock_fn, client):
        mock_fn.return_value = False
        assert client.post("/api/server/save").json()["status"] == "failed"

    @patch("app.services.admin_client.stop_server", new_callable=AsyncMock)
    def test_stop(self, mock_fn, client):
        mock_fn.return_value = True
        assert client.post("/api/server/stop").json()["status"] == "stopping"

    @patch("app.services.admin_client.stop_server", new_callable=AsyncMock)
    def test_stop_failure(self, mock_fn, client):
        mock_fn.return_value = False
        assert client.post("/api/server/stop").json()["status"] == "failed"


class TestPlayerActions:

    @patch("app.services.admin_client.kick_player", new_callable=AsyncMock)
    def test_kick(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/players/111/kick")
        assert resp.json()["status"] == "kicked"
        mock_fn.assert_called_once_with(111)

    @patch("app.services.admin_client.ban_player", new_callable=AsyncMock)
    def test_ban(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/players/222/ban")
        assert resp.json()["status"] == "banned"

    @patch("app.services.admin_client.unban_player", new_callable=AsyncMock)
    def test_unban(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/players/333/unban")
        assert resp.json()["status"] == "unbanned"

    @patch("app.services.admin_client.promote_player", new_callable=AsyncMock)
    def test_promote(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/players/444/promote")
        assert resp.json()["status"] == "promoted"

    @patch("app.services.admin_client.demote_player", new_callable=AsyncMock)
    def test_demote(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/players/555/demote")
        assert resp.json()["status"] == "demoted"

    @patch("app.services.admin_client.kick_player", new_callable=AsyncMock)
    def test_action_failure(self, mock_fn, client):
        mock_fn.return_value = False
        assert client.post("/api/server/players/111/kick").json()["status"] == "failed"

    @patch("app.services.admin_client.update_session_settings", new_callable=AsyncMock)
    def test_session_settings(self, mock_fn, client):
        mock_fn.return_value = True
        resp = client.post("/api/server/session-settings", json={"maxPlayers": 32})
        assert resp.json()["status"] == "updated"


# ═══════════════════════════════════════════════════════════════════
#  Pulsar core config
# ═══════════════════════════════════════════════════════════════════

class TestPulsarConfig:

    def test_get_default_config(self, client):
        resp = client.get("/api/pulsar/config")
        assert resp.status_code == 200
        data = resp.json()
        assert data["data_handling_consent"] is False
        assert data["allow_ipv6"] is True
        assert data["network_timeout"] == 5000

    def test_put_config_roundtrip(self, client):
        config = CoreConfig(
            data_handling_consent=True,
            data_handling_consent_date="2026-04-30",
            allow_ipv6=False,
            network_timeout=10000,
            game_version="1.205.0",
        )
        resp = client.put("/api/pulsar/config", json=config.model_dump())
        assert resp.status_code == 200

        data = client.get("/api/pulsar/config").json()
        assert data["data_handling_consent"] is True
        assert data["data_handling_consent_date"] == "2026-04-30"
        assert data["allow_ipv6"] is False
        assert data["network_timeout"] == 10000
        assert data["game_version"] == "1.205.0"

    def test_config_without_game_version(self, client):
        config = CoreConfig(game_version=None)
        client.put("/api/pulsar/config", json=config.model_dump())
        data = client.get("/api/pulsar/config").json()
        assert data["game_version"] is None


# ═══════════════════════════════════════════════════════════════════
#  Pulsar sources config
# ═══════════════════════════════════════════════════════════════════

class TestPulsarSources:

    def test_get_default_sources(self, client):
        resp = client.get("/api/pulsar/sources")
        assert resp.status_code == 200
        data = resp.json()
        assert data["show_warning"] is True
        assert data["remote_hub_sources"] == []

    def test_put_sources_roundtrip(self, client):
        config = SourcesConfig(
            show_warning=False,
            max_source_age=5,
            remote_hub_sources=[
                RemoteHubConfig(
                    name="TestHub",
                    repo="owner/repo",
                    branch="dev",
                    enabled=True,
                    trusted=True,
                    hash="abc123",
                ),
            ],
            local_plugin_sources=[
                LocalPluginConfig(name="MyPlugin", folder="C:/plugins/my", enabled=True),
            ],
            mod_sources=[
                ModConfig(name="TestMod", id=12345, enabled=True),
            ],
        )
        client.put("/api/pulsar/sources", json=config.model_dump())

        data = client.get("/api/pulsar/sources").json()
        assert data["show_warning"] is False
        assert data["max_source_age"] == 5
        assert len(data["remote_hub_sources"]) == 1
        assert data["remote_hub_sources"][0]["name"] == "TestHub"
        assert data["remote_hub_sources"][0]["repo"] == "owner/repo"
        assert data["remote_hub_sources"][0]["trusted"] is True
        assert len(data["local_plugin_sources"]) == 1
        assert len(data["mod_sources"]) == 1
        assert data["mod_sources"][0]["id"] == 12345


# ═══════════════════════════════════════════════════════════════════
#  Pulsar profiles
# ═══════════════════════════════════════════════════════════════════

class TestPulsarProfiles:

    def test_list_empty(self, client):
        resp = client.get("/api/pulsar/profiles")
        assert resp.status_code == 200
        data = resp.json()
        assert data["profiles"] == []
        assert data["current"] == ""

    def test_create_profile(self, client):
        resp = client.post("/api/pulsar/profiles/TestProfile")
        assert resp.status_code == 200
        assert resp.json()["name"] == "TestProfile"

    def test_create_duplicate_profile(self, client):
        client.post("/api/pulsar/profiles/Dup")
        resp = client.post("/api/pulsar/profiles/Dup")
        assert resp.status_code == 409

    def test_get_profile(self, client):
        client.post("/api/pulsar/profiles/MyProfile")
        resp = client.get("/api/pulsar/profiles/MyProfile")
        assert resp.status_code == 200
        assert resp.json()["name"] == "MyProfile"

    def test_get_missing_profile(self, client):
        resp = client.get("/api/pulsar/profiles/NoSuchProfile")
        assert resp.status_code == 404

    def test_update_profile(self, client):
        client.post("/api/pulsar/profiles/Editable")
        profile = Profile(
            name="Editable",
            github=[GitHubPluginConfig(id="author/plugin", selected_version="1.0.0")],
            dev_folder=[LocalFolderConfig(id="dev1", data_file="plugin.dll", debug_build=True)],
            local=["local-plugin-id"],
            mods=[12345, 67890],
        )
        resp = client.put("/api/pulsar/profiles/Editable", json=profile.model_dump())
        assert resp.status_code == 200

        data = client.get("/api/pulsar/profiles/Editable").json()
        assert len(data["github"]) == 1
        assert data["github"][0]["id"] == "author/plugin"
        assert data["dev_folder"][0]["debug_build"] is True
        assert data["local"] == ["local-plugin-id"]
        assert data["mods"] == [12345, 67890]

    def test_delete_profile(self, client):
        client.post("/api/pulsar/profiles/ToDelete")
        resp = client.delete("/api/pulsar/profiles/ToDelete")
        assert resp.status_code == 200
        assert resp.json()["status"] == "deleted"
        assert client.get("/api/pulsar/profiles/ToDelete").status_code == 404

    def test_delete_missing_profile(self, client):
        resp = client.delete("/api/pulsar/profiles/Ghost")
        assert resp.status_code == 404

    def test_activate_profile(self, client):
        client.post("/api/pulsar/profiles/Active")
        resp = client.post("/api/pulsar/profiles/Active/activate")
        assert resp.status_code == 200
        assert resp.json()["status"] == "activated"

        profiles = client.get("/api/pulsar/profiles").json()
        assert profiles["current"] == "Active"

    def test_list_multiple_profiles(self, client):
        for name in ["Zulu", "Alpha", "Mike"]:
            client.post(f"/api/pulsar/profiles/{name}")
        data = client.get("/api/pulsar/profiles").json()
        assert data["profiles"] == ["Alpha", "Mike", "Zulu"]

    def test_profile_lifecycle(self, client):
        """Create, populate, activate, verify, delete."""
        client.post("/api/pulsar/profiles/Lifecycle")

        profile = Profile(
            name="Lifecycle",
            github=[GitHubPluginConfig(id="org/tool", selected_version="2.1.0")],
            mods=[99999],
        )
        client.put("/api/pulsar/profiles/Lifecycle", json=profile.model_dump())
        client.post("/api/pulsar/profiles/Lifecycle/activate")

        listing = client.get("/api/pulsar/profiles").json()
        assert listing["current"] == "Lifecycle"
        assert "Lifecycle" in listing["profiles"]

        loaded = client.get("/api/pulsar/profiles/Lifecycle").json()
        assert loaded["github"][0]["selected_version"] == "2.1.0"
        assert loaded["mods"] == [99999]

        client.delete("/api/pulsar/profiles/Lifecycle")
        assert client.get("/api/pulsar/profiles/Lifecycle").status_code == 404


# ═══════════════════════════════════════════════════════════════════
#  HTML page routes (smoke tests)
# ═══════════════════════════════════════════════════════════════════

class TestPageRoutes:

    @pytest.mark.parametrize("path", [
        "/",
        "/pulsar/config",
        "/pulsar/profiles",
        "/pulsar/sources",
        "/server/config",
        "/server/world",
        "/server/admin",
        "/server/dashboard",
    ])
    def test_page_returns_html(self, client, path):
        resp = client.get(path)
        assert resp.status_code == 200
        assert "text/html" in resp.headers["content-type"]
