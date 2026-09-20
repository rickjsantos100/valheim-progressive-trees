# Development baseline

This file records the environment verified for Task 01. Update it whenever the
game or modding dependencies change.

## Verified on 2026-09-20

| Component | Version | Status |
| --- | --- | --- |
| macOS | 26.5.2 (25F84), Apple silicon | Verified |
| .NET SDK | 8.0.131, arm64 | Installed and verified |
| Valheim | Not installed | Blocks game assembly inspection and load testing |
| BepInExPack for Valheim | Declared 5.4.2333 | Not installed |
| JotunnLib | Restored 2.27.1 | Runtime not installed |
| Unity editor | Project requests 6000.0.61f1 | Not installed |

The SDK band is pinned by `global.json`. `dotnet restore` succeeds. With Jotunn
prebuild disabled, the unchanged plugin build currently stops at the expected
missing `BepInEx` references because the game-side dependencies have not been
generated.

## Local setup after Valheim is installed

1. Install the BepInExPack for Valheim and Jotunn versions declared in
   `ProgressiveTrees/Package/manifest.json`.
2. Copy `Environment.props.example` to the ignored `Environment.props` file and
   replace `VALHEIM_INSTALL` with the actual Valheim directory.
3. Change `ExecutePrebuild` to `true` in `DoPrebuild.props`.
4. Run `dotnet restore ProgressiveTrees.sln` and then
   `dotnet build ProgressiveTrees.sln --configuration Debug`.
5. Confirm `Progressive Trees 0.1.0 loaded` appears in the BepInEx log after
   launching the game.

Jotunn's prebuild task publicizes the installed game assemblies and supplies the
BepInEx, Harmony, Unity, Jotunn, and Valheim references. Do not enable it against
placeholder paths.

## Game API inspection checklist

Once the installed `assembly_valheim.dll` and `Assembly-CSharp.dll` are
available, record the exact signatures used by the prototype here:

- `TreeBase` damage entry point and its registered RPC name.
- `HitData` chop damage, hit position, attacker, and tool-tier accessors.
- `ZNetView` ownership, validity, RPC registration, and ZDO accessors.
- ZDO health key/type used by standing trees.

The API notes must identify the inspected Valheim build ID and must come from
that installed build, not an online snippet, before Task 01 is marked complete.

## Current blocker

Steam is installed, but its configured library has no Valheim app manifest and
no Valheim directory. Consequently BepInEx/Jotunn cannot be installed into the
game, the publicizer cannot run, current game API signatures cannot be verified,
and the plugin cannot be load-tested yet.
