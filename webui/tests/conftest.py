"""Shared fixtures for the WebUI test suites."""

import os

import pytest
from fastapi.testclient import TestClient


@pytest.fixture()
def tmp_dirs(tmp_path):
    """Create isolated temp directories for DS and Pulsar config."""
    ds_dir = tmp_path / "SpaceEngineersDedicated"
    ds_dir.mkdir()
    pulsar_dir = tmp_path / "Magnetar"
    pulsar_dir.mkdir()
    return ds_dir, pulsar_dir


@pytest.fixture()
def client(tmp_dirs, monkeypatch):
    """FastAPI TestClient with config dirs pointing to temp directories."""
    ds_dir, pulsar_dir = tmp_dirs

    monkeypatch.setenv("DS_CONFIG_DIR", str(ds_dir))
    monkeypatch.setenv("PULSAR_CONFIG_DIR", str(pulsar_dir))

    from app.config import Settings
    import app.config
    new_settings = Settings()
    monkeypatch.setattr(app.config, "settings", new_settings)

    from app.services import ds_config, pulsar_config, admin_client
    monkeypatch.setattr(ds_config, "settings", new_settings)
    monkeypatch.setattr(pulsar_config, "settings", new_settings)
    monkeypatch.setattr(admin_client, "settings", new_settings)

    from app.main import app
    with TestClient(app) as c:
        yield c
