# Usage

Run the `MagnetarLegacy` or `MagnetarInterim` executable from your Dedicated
Server installation in place of `SpaceEngineersDedicated.exe`. Magnetar resolves
the DS install, applies any preloader patches, loads enabled plugins, then hands
off to the dedicated server's own `Main`.

```sh
# Windows
%APPDATA%\Magnetar\MagnetarLegacy.exe
%APPDATA%\Magnetar\MagnetarInterim.exe

# Linux
~/.local/share/Magnetar/MagnetarInterim
```

See **[Configuration](Configuration.md)** for the config/install directories, DS
detection, and environment variables.

## Daemon mode

Pass `-daemon` to detach the process from its parent (typically
[Quasar](https://github.com/viktor-ferenczi/Quasar/releases)) at startup, so the
parent terminating does not take the server down with it.

* **Linux** — this is a `setsid()`: the process leaves the parent's session and
  process group (an explicit `kill -HUP <pid>` still reloads the config). When
  launched as a child it detaches in place, keeping the PID and inherited
  stdout/stderr so a managing parent keeps capturing the log stream until it
  exits. When the process is itself a process-group leader (e.g. a wrapper
  script that `exec`s it), `setsid()` is not permitted in place, so it re-execs
  a detached child and the original exits.
* **Windows** — it detaches from the inherited console.
