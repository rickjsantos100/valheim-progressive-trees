# Progressive Trees — Prototype Plan

## Objective

Create a private, multiplayer-safe Valheim prototype that makes chopping a standing beech a localized, persistent activity. Axe strikes form one cut band around the trunk, and players work around eight sectors before the tree can fall. Vanilla remains responsible for spawning the falling log, stump, drops, effects, and physics.

The prototype must support both `Minigame` and `VisualOnly` modes. Publishing, final shader work, fallen-log scars, stumps, and tree types other than beech are out of scope.

## Player experience

- The first qualifying chop anchors the cut height and centers sector zero on the impact angle.
- The cut band has eight radial sectors. Later hit heights project onto that band.
- Hits near a sector boundary divide damage between the two neighboring sectors.
- Damage that finishes a previously unfinished sector spills to adjacent sectors at full efficiency.
- A hit that begins on a completed sector transfers 25% of its damage toward unfinished neighbors.
- A tree can fall when every sector is at least 60% complete and average completion is at least 80%.
- Efficient chopping should take roughly vanilla effort, subject to playtest tuning.
- Inefficient hits retain normal stamina and durability costs. Woodcutting progress follows effective damage.
- Completed-area hits use fewer fresh chips and duller feedback. A nearly dead but incomplete tree creaks more strongly.
- Vanilla determines fall direction from the final qualifying hit.

## Hit eligibility

A hit enters the system when it:

- originates from a character;
- contains positive chop damage;
- meets the vanilla tool-tier requirement; and
- contains a valid impact position.

This supports modded axes and NPC woodcutters without recognizing prefab names. Hits without a reliable position and non-chopping environmental damage bypass the minigame and follow vanilla behavior.

## Gameplay modes

### Minigame

Sector state modifies effective damage. Vanilla health follows effective chopping progress but is held at the smallest safe nonzero value if it would otherwise reach zero before the cut satisfies the 60%/80% completion rule. The hit that completes the cut is normalized into a valid lethal hit and passed through the original vanilla handler.

### VisualOnly

The same impact mapping, sector allocation, persistence, and visuals apply, but the mod never scales or gates damage.

The server selects the current mode. Each tree records the mode active when its first cut was created; later configuration changes affect untouched trees only.

## Architecture

### Pure sector engine

Game-independent code owns:

- local-angle sector selection;
- boundary weighting;
- full-efficiency overflow;
- completed-sector redirection;
- completion evaluation;
- damage-budget calibration; and
- deterministic result objects for the Valheim adapter.

This layer must not reference Unity, BepInEx, Jötunn, or Valheim classes.

### Persistent state

Each damaged tree uses its existing ZDO. State consists of:

- eight byte-quantized progress values packed into one 64-bit value;
- anchored local cut height;
- sector-grid rotation;
- captured gameplay mode; and
- schema version.

Known old schemas are migrated. Unknown or invalid state resets only the affected scar and fails open for the current hit.

### Valheim adapter

Patch the owner-side `TreeBase.RPC_Damage` while preserving the original method:

1. Validate the tree, ownership, hit, tool tier, attacker, and beech profile.
2. Read and decode state.
3. Convert the world impact point to tree-local height and angle.
4. Ask the pure engine to allocate effective damage.
5. Modify only the qualifying hit and allow vanilla to run.
6. Reconcile and commit authoritative state after vanilla processing.
7. When completion is reached, allow the current hit to trigger vanilla felling.

Non-chopping damage, unsupported trees, invalid hits, and runtime failures remain vanilla.

### Synchronization

- Require matching mod support on server and clients because tree ownership can migrate.
- Only the current tree owner writes sector state.
- Remote controllers check their packed ZDO value on a staggered interval of approximately 0.5 seconds.
- Do not create separate network objects or send a custom RPC for every hit.

### Beech profile

Use an explicit per-prefab geometry profile containing trunk center, radius, and valid cut-height bounds. Validate it against runtime renderer/collider bounds. A failed profile check disables the feature for that tree and leaves it vanilla.

## Visual prototype

- Author four rough low-poly notch stages through the included Unity project.
- Use a distinct completed-sector surface treatment without adding a fifth geometry stage.
- Start with up to eight local child renderers per damaged tree, swapping shared meshes and materials.
- Blend neighboring pieces visually even though debug mode shows sector boundaries.
- Show full geometry nearby, a simplified scar at medium range, and no custom geometry at long range.
- Reuse suitable vanilla impact and creaking sounds.
- Do not add colliders, rigidbodies, lights, per-sector `Update` methods, or separate `ZNetView`s.

If profiling justifies it, migrate from child renderers to one combined runtime mesh per tree. A cutaway shader with interior geometry is a later milestone only after the gameplay proves enjoyable.

## Lifecycle and performance

Partial cuts persist indefinitely while the standing tree exists. When the tree falls, the mod must leave:

- no scar renderers or controller;
- no polling or diagnostic registry entry;
- no generated runtime mesh;
- no state copied to the log or stump; and
- no mod-maintained history of the felled tree.

Prototype performance gate: 100 simultaneously loaded damaged trees should add no more than approximately 0.5 ms/frame, and a felled tree must contribute zero continuing work.

## Failure and compatibility policy

- Fail open: unexpected errors allow vanilla damage and emit throttled diagnostics.
- Repeated failures make only the affected tree vanilla-only for its lifetime.
- Unsupported and unprofiled trees remain vanilla.
- Known mods that replace the full tree-damage handler force `VisualOnly` behavior.
- Observer and multiplier mods should remain compatible and receive explicit tests.
- Follow Valheim's normal client-ownership trust model; no custom anti-cheat layer.
- Inert custom fields on living trees after uninstall are accepted.

## Debug tooling

Provide a local debug overlay showing the targeted tree's:

- sector boundaries and values;
- anchored height and grid rotation;
- owner and ZDO identity;
- captured gameplay mode;
- vanilla health and effective damage budget; and
- completion result.

Development commands can inspect, set, reset, fill, and clear the targeted tree's state.

## Verification matrix

### Automated

- Sector-center and boundary selection
- Weighted boundary splits
- Overflow order and conservation
- 25% completed-sector redirection
- 60% minimum / 80% average completion
- Damage-budget calibration
- Byte quantization and 64-bit packing
- Schema migration and invalid-state handling

### In game

- New and existing-world beech trees
- Previously damaged tree initialization using remaining health
- Save/reload and zone unload/reload
- Visual-only versus minigame behavior
- Tool-tier rejection and invalid impact positions
- Non-chopping damage and falling-tree chain reactions
- Two simultaneous first hits
- Player-hosted multiplayer
- Dedicated server
- Disconnect/reconnect and owner migration
- Known observer, multiplier, and conflict mods
- 100-tree performance stress test
- Complete resource and registry cleanup after felling

## Technical references

- Jötunn quickstart: https://valheim-modding.github.io/Jotunn/guides/quickstart.html
- Valheim object RPC reference: https://github.com/JereKuusela/valheim-expand_world_prefabs/blob/main/RPCs.md
- Jötunn RPC guidance: https://valheim-modding.github.io/Jotunn/tutorials/rpcs.html
- Jötunn network compatibility: https://valheim-modding.github.io/Jotunn/tutorials/networkcompatibility.html
- Harmony prefix/postfix guidance: https://harmony.pardeike.net/v2/articles/patching-prefix.html
- Harmony patch ordering: https://harmony.pardeike.net/v2/articles/priorities.html
