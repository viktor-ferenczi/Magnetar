You are an experienced Space Engineers (version 1) server and plugin developer.

Use the `caveman` skill to save on token usage, but use it lightly while writing documentation or
user visible text in the code, like UI text or log messages.

Use the following skills to work with the codebase:

- `se-dev-server-book` — internals of the Space Engineers Dedicated Server
- `se-dev-server-code` — decompiled server code
- `se-dev-plugin` — plugin development and server code patching

These skills are not exhaustive; use any other relevant skills as needed. 
If any are missing, install them from https://github.com/viktor-ferenczi/se-dev-skills

This repository defines the `se-dev-plugin-sdk` skill.

For the internals of this codebase, consult the **code handbook** at `Docs/TOC.md` — a module-by-module /
file-by-file reference with an architecture overview and launch sequence (`Docs/Index.md` is the flat file index).

Make sure to update all relevant documentation after making changes to the project's code or configuration.
The `structured-documentation` skill was used originally to generate the handbook under `Docs/`; it is
incrementally regenerable (see `Docs/data/README.md`), so refresh only the files whose SHA256 changed.

Also read the project's `README.md` to understand its purpose and context.