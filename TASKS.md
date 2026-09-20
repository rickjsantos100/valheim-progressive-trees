# Progressive Trees — Task Backlog

Work through these tasks in order. A task starts only after all listed dependencies pass their acceptance checks.

## Task 01 — Establish the build and inspection baseline

**Depends on:** none

- [ ] Locate the current Valheim installation and BepInEx/Jötunn environment.
- [x] Install or expose a compatible .NET build toolchain (.NET SDK 8.0.131).
- [ ] Enable the Jötunn publicized-assembly prebuild against the installed game.
- [ ] Confirm current signatures and fields for `TreeBase`, `HitData`, `ZNetView`, and ZDO health.
- [ ] Record the tested Valheim, BepInEx, Jötunn, and Unity versions.
- [ ] Build and load the unchanged plugin in Valheim.

**Acceptance:** the plugin builds reproducibly, loads without errors, and logs its version; verified game API notes are committed.

## Task 02 — Implement the pure sector engine

**Depends on:** Task 01

- [ ] Create game-independent sector state and hit-input/result types.
- [ ] Implement eight-sector angle mapping with first-hit rotation.
- [ ] Implement proportional two-sector boundary splits.
- [ ] Implement full-efficiency overflow from unfinished sectors.
- [ ] Implement 25% redirection from sectors complete before the hit.
- [ ] Implement nearest-unfinished-neighbor selection with deterministic tie handling.
- [ ] Implement 60% minimum / 80% average completion.
- [ ] Add exhaustive unit tests for conservation, limits, wrapping, and completion.

**Acceptance:** all sector tests pass without referencing Unity or Valheim assemblies.

## Task 03 — Implement state packing and migration

**Depends on:** Task 02

- [ ] Quantize eight progress values to bytes and pack them into one 64-bit value.
- [ ] Define namespaced ZDO keys for progress, height, rotation, mode, and schema.
- [ ] Implement deterministic encode/decode round trips.
- [ ] Add schema migration hooks and invalid-state fallback.
- [ ] Test boundary values, corrupt input, and unknown versions.

**Acceptance:** state round-trips within the documented quantization tolerance; invalid state cannot block damage.

## Task 04 — Add beech profiling and debug-only hit observation

**Depends on:** Tasks 01–03

- [ ] Identify the standing beech prefab variants.
- [ ] Measure and define the explicit beech trunk profile.
- [ ] Validate the profile against runtime bounds.
- [ ] Patch the owner-side tree damage path without changing damage yet.
- [ ] Anchor height and rotation on the first qualifying hit.
- [ ] Display sector selection, split weights, ownership, health, and mode in debug output.
- [ ] Confirm unsupported trees and invalid hits remain vanilla.

**Acceptance:** chopping a beech reliably identifies the expected local sector in single-player and a hosted game, with no gameplay change.

## Task 05 — Add persistence and remote synchronization

**Depends on:** Task 04

- [ ] Commit state only from the authoritative owner.
- [ ] Reconstruct state after save/reload and zone reload.
- [ ] Add staggered approximately 2 Hz remote state observation.
- [ ] Capture each tree's mode on its first cut.
- [ ] Handle simultaneous first hits deterministically.
- [ ] Add inspect, set, reset, fill, and clear development commands.

**Acceptance:** sector state survives reloads, converges across two clients, and remains correct after reconnect and ownership migration.

## Task 06 — Implement gameplay modes and vanilla felling handoff

**Depends on:** Task 05

- [ ] Implement `VisualOnly` without modifying accepted damage.
- [ ] Implement effective-damage allocation for `Minigame`.
- [ ] Preserve normal costs and award skill progress only for effective work.
- [ ] Hold vanilla health safely above zero when shape completion is missing.
- [ ] Let non-chopping damage bypass the minigame.
- [ ] Make the completing hit trigger the original vanilla felling path.
- [ ] Add fail-open behavior and throttled diagnostics.

**Acceptance:** neither mode reimplements tree destruction; minigame completion gates qualifying chop damage, while vanilla still produces the falling log, stump, drops, and effects.

## Task 07 — Build the rough low-poly visual pass

**Depends on:** Task 06

- [ ] Author four notch-stage meshes and completed-state treatment in Unity.
- [ ] Package and load the AssetBundle.
- [ ] Create up to eight local child renderers using shared meshes/materials.
- [ ] Blend adjacent sectors visually while retaining debug boundaries.
- [ ] Add nearby, simplified-medium, and hidden-far distance behavior.
- [ ] Add reduced fresh-chip feedback on completed sectors.
- [ ] Add stronger vanilla creaking feedback for health-gated incomplete trees.

**Acceptance:** sector state is readable without debug mode, remote changes appear within the accepted synchronization delay, and visuals do not affect collision.

## Task 08 — Complete multiplayer and compatibility validation

**Depends on:** Task 07

- [ ] Test player-hosted and dedicated-server sessions.
- [ ] Test simultaneous hits, reconnect, and ownership migration.
- [ ] Test invalid positions, insufficient tool tiers, and modded chop tools.
- [ ] Test environmental damage and falling-tree chain reactions.
- [ ] Test selected observer and damage-multiplier mods.
- [ ] Detect known full-handler conflicts and force visual-only behavior.
- [ ] Inspect Harmony patch owners/order in diagnostics.

**Acceptance:** all peers agree on state and felling; known failures degrade to vanilla or visual-only behavior instead of blocking trees.

## Task 09 — Verify performance and lifecycle cleanup

**Depends on:** Task 08

- [ ] Generate controlled scenes with 10, 50, and 100 loaded damaged beeches.
- [ ] Measure controller polling, rendering, draw calls, allocations, and frame cost.
- [ ] Confirm the 100-tree target stays near or below 0.5 ms/frame.
- [ ] Fell, unload, and destroy trees while watching controllers, renderers, registries, and generated resources.
- [ ] Verify no cut state transfers to logs or stumps.
- [ ] Migrate to one combined mesh per tree only if measurements justify it.

**Acceptance:** a felled tree leaves zero Progressive Trees runtime objects or work; the stress target passes or the measured optimization is completed.

## Task 10 — Prototype hardening and playtest review

**Depends on:** Task 09

- [ ] Run the full automated and in-game regression matrix.
- [ ] Tune damage calibration for enjoyable efficient and inefficient chopping.
- [ ] Verify existing-world safety and backup/restore instructions.
- [ ] Document current limitations and known compatibility behavior.
- [ ] Decide whether to continue to polished assets and a cutaway-shader experiment.

**Acceptance:** the agreed beech prototype is stable, multiplayer-safe, persistent, performant, and ready for a go/no-go visual-polish decision.
