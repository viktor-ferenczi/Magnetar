# Configuration

There are **three** distinct folders involved, each overridable on the command
line:

| Folder | Holds | Default | Override |
| ------ | ----- | ------- | -------- |
| **Magnetar config dir** | Magnetar's own config, logs, preloader cache | Windows: `%APPDATA%\Magnetar`<br>Linux: `$XDG_CONFIG_HOME/Magnetar` → `~/.config/Magnetar` | `-config <dir>` |
| **DS install dir** | The dedicated-server binaries (`DedicatedServer64/`) | Auto-detected (see below) | `-ds64 <dir>` |
| **DS data dir (AppData)** | `SpaceEngineers-Dedicated.cfg` **and the world saves** (`Saves/`) | Windows: `%APPDATA%\SpaceEngineersDedicated`<br>(`%APPDATA%` = roaming AppData) | `-path <dir>` |

## Command-line parameters that change folders

### `-config <dir>` — Magnetar's own config/log directory

Overrides where Magnetar stores its own configuration, logs, and the preloader
cache. A relative path resolves against the launcher's directory. This does
**not** affect the dedicated server's config or saves.

* **Install dir (default, where the launcher lives)**
  * Windows — `%APPDATA%\Magnetar`.
  * Linux — `$XDG_DATA_HOME/Magnetar`, falling back to `~/.local/share/Magnetar`.

### `-ds64 <dir>` — dedicated-server install location

Points Magnetar at the `DedicatedServer64/` folder containing
`SpaceEngineersDedicated.exe`. A relative path resolves against the launcher's
directory.

When not given, the DS install is auto-detected from the Steam registry
(Windows) or `~/.steam/steam/steamapps/common/SpaceEngineersDedicatedServer/DedicatedServer64`
(Linux), or any Steam library listed in `libraryfolders.vdf`.

### `-path <dir>` — DS data directory (AppData: config + world saves)

This is the **dedicated server's own** argument; Magnetar passes the full
command line through to it. It sets the server *instance/data* directory — the
folder holding `SpaceEngineers-Dedicated.cfg` and the `Saves/` worlds. Without
it, the server uses its default instance, `%APPDATA%\SpaceEngineersDedicated` on
Windows.

> **Important — `-path` requires `-console` (or `-noconsole`).** The server only
> *applies* a custom data path inside its `-console`/`-noconsole` startup branch.
> Pass them together:
>
> ```sh
> MagnetarInterim -console -path "D:\SE\MyServerInstance"
> ```
>
> A bare `-path` (without `-console`/`-noconsole`) is ignored and the default
> AppData instance is used. The target directory **must already exist** — if it
> does not, the dedicated server aborts startup. Use an absolute path; relative
> `-path` values are resolved against the DS binaries' folder, not the launcher.

#### A note on `-console` / `-noconsole`

You do **not** need these to run headless — Magnetar already bypasses the
server's WinForms/Telerik configurator and starts it directly (with console
output enabled, equivalent to `-console`). They differ only in whether the
server, *when running interactively*, attaches to the parent console or
allocates a new console window; on a non-interactive host both are no-ops. The
**only** reason to pass one is to make `-path` take effect (above). When you do:

* **`-console`** matches Magnetar's default console behaviour.
* **`-noconsole`** skips the console attach entirely — the cleaner choice when
  running under Quasar with `-daemon` (which releases the console on Windows), so
  the server won't re-grab or pop a console window.

Related pass-through DS flags `-session:<path>` (selects which saved world to
load) and `-ignorelastsession` take effect with or without `-console`.

## Environment variables

| Variable             | Effect                                                           |
| -------------------- | ---------------------------------------------------------------- |
| `MAGNETAR_SAFE_MODE` | When `1`, disables preloader patches for a one-off recovery run. |
| `XDG_CONFIG_HOME`    | Overrides the Magnetar config-dir base (Linux).                  |
| `XDG_DATA_HOME`      | Overrides the Magnetar install-dir base (Linux).                 |
| `DS64`               | Build-time override for the DS reference path.                   |

Build-time overrides are covered in full in **[Build.md](Build.md)**.
