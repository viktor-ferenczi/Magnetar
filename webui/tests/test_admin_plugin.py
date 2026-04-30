"""Integration tests for the Admin plugin's REST API running inside a real DS.

These tests start a Dedicated Server with the Admin plugin loaded, wait for
the world to be ready, exercise the Admin HTTP API on port 9000, then shut
down the server and clean up the test world.

Requirements:
  - SE Dedicated Server installed (set DS_INSTALL_DIR env var or
    ADMIN_TEST_DS_DIR to the DedicatedServer64 folder).
  - The Admin plugin DLL built and placed in the DS Plugins directory.
  - These tests are slow (~60-120 s) and are skipped by default unless
    the ADMIN_INTEGRATION environment variable is set to "1".

Usage:
    set ADMIN_INTEGRATION=1
    set ADMIN_TEST_DS_DIR=C:\\SteamCMD\\steamapps\\common\\SpaceEngineersDedicatedServer\\DedicatedServer64
    uv run pytest tests/test_admin_plugin.py -v -s
"""

import os
import shutil
import socket
import subprocess
import sys
import textwrap
import time
import xml.etree.ElementTree as ET
from pathlib import Path

import httpx
import pytest

ADMIN_PORT = 19000
DS_PORT = 37016
STEAM_PORT = 38766
WORLD_NAME = "PulsarTestWorld"
SERVER_NAME = "PulsarTestServer"
STARTUP_TIMEOUT = 120
POLL_INTERVAL = 2.0

skip_unless_integration = pytest.mark.skipif(
    os.environ.get("ADMIN_INTEGRATION", "0") != "1",
    reason="Set ADMIN_INTEGRATION=1 to run integration tests against a real DS",
)


def _ds_install_dir() -> Path:
    d = os.environ.get("ADMIN_TEST_DS_DIR", "")
    if not d:
        d = os.environ.get("DS_INSTALL_DIR", "")
    return Path(d) if d else None


def _is_port_open(port: int, host: str = "127.0.0.1", timeout: float = 1.0) -> bool:
    try:
        with socket.create_connection((host, port), timeout=timeout):
            return True
    except (ConnectionRefusedError, TimeoutError, OSError):
        return False


def _create_test_world(config_dir: Path):
    """Write a minimal DS config that creates a new empty world."""
    config_dir.mkdir(parents=True, exist_ok=True)

    cfg_path = config_dir / "SpaceEngineers-Dedicated.cfg"
    root = ET.Element("MyConfigDedicated")
    root.set("xmlns:xsd", "http://www.w3.org/2001/XMLSchema")
    root.set("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")

    ss = ET.SubElement(root, "SessionSettings")
    for tag, val in [
        ("GameMode", "0"),
        ("OnlineMode", "3"),
        ("MaxPlayers", "4"),
        ("EnableSaving", "true"),
        ("AutoHealing", "true"),
        ("WorldSizeKm", "0"),
        ("ProceduralDensity", "0.35"),
        ("ProceduralSeed", "12345"),
        ("TotalPCU", "100000"),
        ("EnableIngameScripts", "false"),
        ("EnableSpectator", "false"),
        ("EnableVoxelDestruction", "true"),
    ]:
        e = ET.SubElement(ss, tag)
        e.text = val

    for tag, val in [
        ("ServerPort", str(DS_PORT)),
        ("SteamPort", str(STEAM_PORT)),
        ("ServerName", SERVER_NAME),
        ("WorldName", WORLD_NAME),
        ("PauseGameWhenEmpty", "false"),
        ("IgnoreLastSession", "true"),
        ("RemoteApiEnabled", "false"),
    ]:
        e = ET.SubElement(root, tag)
        e.text = val

    ET.SubElement(root, "Administrators")
    ET.SubElement(root, "Banned")

    tree = ET.ElementTree(root)
    ET.indent(tree, space="  ")
    tree.write(cfg_path, encoding="utf-8", xml_declaration=True)

    return cfg_path


def _cleanup_test_world(config_dir: Path):
    saves_dir = config_dir / "Saves"
    test_world = saves_dir / WORLD_NAME
    if test_world.exists():
        shutil.rmtree(test_world, ignore_errors=True)


class AdminAPI:
    """Thin wrapper for calling the Admin plugin HTTP API."""

    def __init__(self, base_url: str = f"http://127.0.0.1:{ADMIN_PORT}"):
        self.base = base_url

    def get(self, path: str, **kwargs) -> httpx.Response:
        return httpx.get(f"{self.base}{path}", timeout=5.0, **kwargs)

    def post(self, path: str, **kwargs) -> httpx.Response:
        return httpx.post(f"{self.base}{path}", timeout=10.0, **kwargs)


@skip_unless_integration
class TestAdminPluginIntegration:
    """Full integration tests against a running Dedicated Server."""

    _process: subprocess.Popen = None
    _config_dir: Path = None

    @classmethod
    def setup_class(cls):
        ds_dir = _ds_install_dir()
        if not ds_dir or not ds_dir.exists():
            pytest.skip(f"DS install dir not found: {ds_dir}")

        exe = ds_dir / "SpaceEngineersDedicated.exe"
        if not exe.exists():
            pytest.skip(f"DS executable not found: {exe}")

        cls._config_dir = Path(os.environ.get("APPDATA", "")) / "SpaceEngineersDedicated_Test"
        _create_test_world(cls._config_dir)

        cls._process = subprocess.Popen(
            [str(exe), "-console", "-noconsole", "-path", str(cls._config_dir)],
            cwd=str(ds_dir),
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
        )

        deadline = time.monotonic() + STARTUP_TIMEOUT
        while time.monotonic() < deadline:
            if _is_port_open(ADMIN_PORT):
                api = AdminAPI()
                try:
                    resp = api.get("/api/state")
                    if resp.status_code == 200 and resp.json().get("isRunning"):
                        return
                except Exception:
                    pass
            time.sleep(POLL_INTERVAL)

        cls._kill_server()
        pytest.skip("DS did not become ready within timeout")

    @classmethod
    def teardown_class(cls):
        cls._kill_server()
        if cls._config_dir:
            _cleanup_test_world(cls._config_dir)

    @classmethod
    def _kill_server(cls):
        if cls._process:
            cls._process.terminate()
            try:
                cls._process.wait(timeout=30)
            except subprocess.TimeoutExpired:
                cls._process.kill()
            cls._process = None

    @pytest.fixture(autouse=True)
    def api(self):
        self.api = AdminAPI()

    # ── Server state ──

    def test_get_server_state(self):
        resp = self.api.get("/api/state")
        assert resp.status_code == 200
        data = resp.json()
        assert data["isRunning"] is True
        assert isinstance(data["serverName"], str)
        assert isinstance(data["playersOnline"], int)
        assert isinstance(data["maxPlayers"], int)
        assert isinstance(data["simSpeed"], (int, float))
        assert isinstance(data["uptimeSeconds"], int)
        assert data["uptimeSeconds"] >= 0

    def test_server_state_has_pcu(self):
        data = self.api.get("/api/state").json()
        assert "totalPcu" in data
        assert "usedPcu" in data

    def test_server_state_has_version(self):
        data = self.api.get("/api/state").json()
        assert "gameVersion" in data

    # ── Players ──

    def test_get_players_returns_list(self):
        resp = self.api.get("/api/players")
        assert resp.status_code == 200
        data = resp.json()
        assert isinstance(data, list)

    def test_player_fields(self):
        players = self.api.get("/api/players").json()
        for p in players:
            assert "steamId" in p
            assert "displayName" in p
            assert "faction" in p
            assert "isAdmin" in p
            assert "pingMs" in p

    # ── Chat ──

    def test_get_chat_returns_list(self):
        resp = self.api.get("/api/chat")
        assert resp.status_code == 200
        assert isinstance(resp.json(), list)

    def test_get_chat_with_count(self):
        resp = self.api.get("/api/chat", params={"count": "5"})
        assert resp.status_code == 200
        assert len(resp.json()) <= 5

    def test_send_chat_and_receive(self):
        msg = f"test-{time.time_ns()}"
        resp = self.api.post("/api/chat", json={"message": msg})
        assert resp.status_code == 200
        assert resp.json()["status"] == "sent"

        time.sleep(0.5)
        chat = self.api.get("/api/chat", params={"count": "10"}).json()
        messages = [c["message"] for c in chat]
        assert msg in messages

    def test_send_chat_appears_from_server(self):
        msg = f"server-msg-{time.time_ns()}"
        self.api.post("/api/chat", json={"message": msg})
        time.sleep(0.5)
        chat = self.api.get("/api/chat").json()
        match = [c for c in chat if c["message"] == msg]
        assert len(match) == 1
        assert match[0]["sender"] == "Server"

    # ── Save ──

    def test_save_world(self):
        resp = self.api.post("/api/save")
        assert resp.status_code == 200
        assert resp.json()["status"] == "saved"

    # ── Player actions (no players connected, but endpoints should respond) ──

    def test_kick_nonexistent_player(self):
        resp = self.api.post("/api/players/99999999999/kick")
        assert resp.status_code == 200

    def test_ban_nonexistent_player(self):
        resp = self.api.post("/api/players/99999999999/ban")
        assert resp.status_code == 200

    def test_unban_nonexistent_player(self):
        resp = self.api.post("/api/players/99999999999/unban")
        assert resp.status_code == 200

    def test_promote_nonexistent_player(self):
        resp = self.api.post("/api/players/99999999999/promote")
        assert resp.status_code == 200

    def test_demote_nonexistent_player(self):
        resp = self.api.post("/api/players/99999999999/demote")
        assert resp.status_code == 200

    def test_unknown_player_action_returns_status(self):
        resp = self.api.post("/api/players/99999999999/explode")
        assert resp.status_code == 200
        assert resp.json()["status"] == "unknown_action"

    # ── Session settings (501 not implemented) ──

    def test_session_settings_not_implemented(self):
        resp = self.api.post("/api/session-settings", json={"maxPlayers": 8})
        assert resp.status_code == 501

    # ── Error cases ──

    def test_unknown_endpoint(self):
        resp = self.api.get("/api/nonexistent")
        assert resp.status_code == 404

    def test_cors_preflight(self):
        resp = httpx.options(
            f"http://127.0.0.1:{ADMIN_PORT}/api/state",
            timeout=5.0,
        )
        assert resp.status_code == 204
        assert "access-control-allow-origin" in resp.headers

    # ── Stop (run last) ──

    def test_zz_stop_server(self):
        """Stop the server gracefully. Named zz_ so it sorts last."""
        resp = self.api.post("/api/stop")
        assert resp.status_code == 200
        assert resp.json()["status"] == "stopping"
