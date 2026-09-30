# Technology Tree Information UX Research

## Status

**Target:** Graveyard Keeper 1.407  
**Research status:** product hypothesis supported; production project intentionally not created yet.  
**Purpose:** establish whether the vanilla Technology tree has a material information/decision-support gap, identify the smallest useful product shape, and record the verified host data/seams that a future production mod could reuse.

This is research, not a production-mod specification. A future owning repository must re-establish its own acceptance envelope and production evidence gates before source mutation.

## Product question

The working hypothesis is that Graveyard Keeper exposes Technology unlocks too tersely for informed planning. The player can often see that a technology grants a recipe, blueprint, work action, or perk, but cannot reliably answer from the Technology screen:

- what materials the newly unlocked thing will require;
- where the recipe is crafted or the blueprint is built;
- whether the unlock is immediately usable with the player's other technology;
- what an unfamiliar unlock actually does;
- which information is a direct authored prerequisite versus an indirect practical dependency.

The desired product outcome is **better in-context decision support before spending technology points**, without turning the Technology tree into a spoiler-heavy external wiki or changing progression/gameplay.

## Community evidence

The problem is persistent rather than isolated to one old support question.

### Direct reports

- Steam, **"What's the most frustrating for new players?"** (2026):
  https://steamcommunity.com/app/599140/discussions/0/801218828360110625/
  - the opening post explicitly calls out the Technology tree because technologies do not say what is used/required for their unlocked elements;
  - players describe writing down requirements, repeated travel, and wanting hover tooltips;
  - one response says seeing recipes in the Technology tree would help because an unlocked technology can reveal that another technology is still needed before the result can be built;
  - the same thread also contains an important counterpoint: some players like Graveyard Keeper's discovery and interdependency. Better information should therefore expose concrete facts without flattening the progression into automatic recommendations.

- Steam, **"The technology tree is scuffed and it annoys me."** (2023):
  https://steamcommunity.com/app/599140/discussions/0/3872592051320559726/
  - the author describes technology purchases as a gamble because the newly unlocked result can depend on another technology not represented by the visible path;
  - limited early technology points make the missing dependency information feel like a wasted purchase rather than merely an inconvenience.

- Reddit, **"Discussion: my frustration with the progression"** (2025):
  https://www.reddit.com/r/GraveyardKeeper/comments/1l2486u/
  - the complaint is that practical dependencies are effectively impossible to infer from the tree alone;
  - a proposed fix is specifically to show required components when hovering a workstation technology and indicate the technologies associated with those components.

- Reddit, **"Graveyard Keeper 2: Please fix the instructions so we don't need a wiki"** (2026, discussing lessons from the first game):
  https://www.reddit.com/r/GraveyardKeeper/comments/1umrzzl/graveyard_keeper_2_please_fix_the_instructions_so/
  - the author cites first-game Technology recipes not being inspectable from the tree, blueprint locations not being stated, and the need to memorize or externally record build requirements.

- Steam, **"Church workbench?"** (2020):
  https://steamcommunity.com/app/599140/discussions/0/2997669078636739472/
  - answering one concrete unlock question requires explaining the exact tree/technology, the build location, the build materials, and a separate Smithing dependency for Complex Iron Parts. This is representative of the information bundle players currently reconstruct outside the Technology screen.

### Adjacent current signal

A September 2026 Reddit thread about Graveyard Keeper 1 describes player guidance generally as poor enough that external search often becomes the clearest way to learn systems:
https://www.reddit.com/r/GraveyardKeeper/comments/1w9mx25/i_hardly_ever_give_up_on_games_but_i_think_this/

This is broader than Technology UX, so it is supporting context rather than direct proof of the specific feature.

### Existing workarounds and player-proposed solutions

Observed workarounds:

- wiki / web search;
- handwritten or external notes;
- repeated travel back to build desks/workstations to re-check requirements;
- trial-and-error technology purchases.

Observed solution ideas:

- richer hover tooltips;
- recipes/materials visible directly in the Technology tree;
- build location information;
- pinning recipes/requirements.

A current Graveyard Keeper 1 **Recipe Pin** mod already addresses the last item from craft/build menus:
https://www.nexusmods.com/graveyardkeeper/mods/154

That makes pinning an adjacent solved/partly-solved need rather than a reason to overload the first Technology-information mod.

### Existing-mod landscape

Targeted searches of Graveyard Keeper 1 Nexus/GitHub results did not surface a dedicated mod whose primary purpose is to enrich vanilla Technology-tree unlock tooltips with recipe/build details.

This is a **bounded search result, not proof of absence**. Re-check before public release because the mod ecosystem changes.

## Pain taxonomy

The community evidence separates into several different problems that should not be conflated:

1. **Unlock-detail gap** — the tree names an unlock but does not expose enough concrete data about it.
2. **Practical dependency gap** — an unlocked recipe/blueprint can require components or stations gated by another technology.
3. **Location gap** — the player may not know at which workstation/build desk/area the new thing is used.
4. **Memory/backtracking cost** — build requirements are learned only at another UI/location, encouraging notes and repeated travel.
5. **Hidden/story gating** — some technologies or capabilities are revealed by quests/interactions rather than ordinary point purchase.
6. **Graph/readability issues** — a prerequisite can technically be represented by the tree but still be easy to miss.

The first four form a coherent information-UX problem. Hidden/story gating and graph readability require separate treatment; automatically revealing hidden state would be spoiler-prone and should not be folded into the initial solution.

## Verified Graveyard Keeper 1.407 host facts

Pinned source identity for the static host inspection:

`Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`

### Technology data ownership

`TechDefinition` owns the authored technology-level data:

- `crafts`
- `works`
- `phrases`
- `perks`
- parent/child technology links
- price
- hidden/invisible state
- DLC gate

`TechDefinition.GetUnlocksList()` converts those authored IDs into `TechUnlock` objects.

`TechUnlock` IDs prefixed with `@` are made `visible = false`. A presentation mod must preserve that authored invisibility and must not surface those unlocks merely because their underlying data can be inspected.

`TechDefinition.GetVisibleUnlocksList()` exposes at most three visible unlock entries because vanilla defines `MAX_VISIBLE_UNLOCKS = 3`.

### Existing Technology tooltip seam

Mouse path:

`TechTreeGUIItem.Draw -> TechTreeGUIUnlockItem.Draw(..., init_tooltip: true) -> TechUnlock.GetTooltip`

Gamepad path:

`TechTreeGUIItem.InitGamepadTooltip -> each visible TechUnlock.GetTooltip`

The gamepad Technology node combines the visible child-unlock tooltips into the parent tooltip; mouse mode gives the child unlocks their own tooltips.

Therefore `TechUnlock.GetTooltip` is a verified semantic seam shared by both input modes for visible unlock detail. A future implementation does not need separate mouse/gamepad knowledge models merely to enrich the information content.

Existing shared research also proves the Technology-tooltip NGUI width lifecycle: the inspected label uses native `ResizeFreely`, with finite `UILabel.overflowWidth` acting as the effective wrap/expansion ceiling. That result may be reused for layout if richer content reaches the same tooltip family, but the final content geometry still requires representative runtime acceptance.

### Verified vanilla blueprint-detail omission

`TechUnlock.GetData()` can identify a craft unlock as an `ObjectCraftDefinition` blueprint when ordinary `CraftDefinition` lookup fails and the ID follows the blueprint-style path.

However, `TechUnlock.GetTooltip()` does not carry that resolved `ObjectCraftDefinition` forward:

1. it tries `GameBalance.GetDataOrNull<CraftDefinition>(id)`;
2. when null and the ID contains `:`, it calls `GameBalance.GetData<ObjectCraftDefinition>(id)`;
3. that returned value is discarded;
4. `dataOrNull` remains null;
5. the method returns after the title/separator instead of rendering craft/build details.

**Status:** verified static 1.407 host fact.

This is a concrete host-level reason why at least the inspected blueprint unlock path is informationally sparse; it is not merely a localization-quality complaint.

### Native recipe/build data already available

For ordinary recipes, `CraftDefinition` owns useful presentation facts including:

- `needs` — required ingredients;
- `output`;
- `craft_in` — workstation/object IDs;
- craft type/subtype and related authored fields.

Shared research already proves that `GameBalance.GetItemCraftsIn(item_id)` is built from native `CraftDefinition.craft_in` data and feeds vanilla crafting-location tooltips.

For build blueprints, `ObjectCraftDefinition : CraftDefinition` adds:

- `out_obj` — built object;
- `build_type`;
- `builder_ids` — build desks through which the blueprint is offered;
- `locked_builders_ids`;
- `sub_zone_id` — authored placement-area restriction when present;
- inherited `needs` — build materials.

`MainGame` uses `ObjectCraftDefinition.builder_ids` when constructing a build desk's visible recipe list. `BuildModeLogics` / build-grid code consumes `sub_zone_id` for placement restrictions.

**Important limit:** the raw data owners are verified, but a generic, correctly localized player-facing mapping from every `builder_id` / `sub_zone_id` to wording such as "Build at: Alchemy Lab" has **not yet been established**. Do not ship guessed ID-to-area naming rules.

## Solution-space checkpoint

### A. Enrich the existing Technology unlock tooltip

**Product shape**

Keep the vanilla Technology tree and add concrete native facts to the tooltip for the unlock currently hovered/focused.

Candidate facts, when the host data supports them safely:

- ordinary recipe: required materials and crafting station/location;
- blueprint: build materials and verified build-desk/area presentation;
- perk/work unlock: preserve/use the authored description, adding data only where a concrete omission is proven.

**Strengths**

- fixes the problem at the decision point before spending technology points;
- preserves vanilla navigation and visual model;
- reuses current native data rather than maintaining a second wiki database;
- `TechUnlock.GetTooltip` already covers mouse and gamepad semantics;
- naturally avoids a new screen, save state, or background system.

**Risks/open work**

- richer rows must remain readable when a technology has multiple unlocks;
- build-desk/area names need verified localization semantics;
- some practical dependencies are indirect and should not be guessed.

**Current direction:** preferred.

### B. Add a separate Technology details/encyclopedia window

Could show more information without tooltip-size limits.

Rejected for the initial scope because it introduces another UI/navigation surface and duplicates the tree's existing selection context before evidence shows the tooltip cannot carry the useful minimum.

Re-open only if representative runtime content proves the native tooltip fundamentally insufficient.

### C. Compute a full practical dependency graph

Example: warn that a blueprint requires an ingredient whose production recipe is gated by another technology, then highlight that technology.

Potentially useful, but not suitable for the first version:

- item acquisition is not synonymous with crafting; ingredients may be bought, found, quest-given, or otherwise obtainable;
- stations themselves can have progression dependencies;
- a recursive dependency engine can easily present false "required technology" claims;
- hidden/story gates increase spoiler risk and lifecycle complexity.

**Status:** separate research problem; do not infer these dependencies from recipe data alone.

### D. Add requirement pinning/planning

Real adjacent player need, but Graveyard Keeper 1 already has Recipe Pin for craft/build-menu requirements. A future Technology mod may integrate with or complement that workflow only if a concrete additional need remains.

Not part of the first Technology-information MVP.

## Recommended acceptance envelope for a future MVP

A future production mod should aim for this user-visible outcome:

> Before purchasing a visible technology, the player can inspect its visible unlocks in the existing Technology tree and see the concrete information needed to understand what the unlock gives and where/how it is used, when that information is authoritatively available from vanilla data.

Preserved invariants:

- no technology price or progression changes;
- no automatic unlocks;
- no save mutation;
- no exposure of authored invisible/hidden unlock content;
- no guessed practical-dependency claims;
- mouse and gamepad both receive equivalent information through their native focus model;
- localization should derive from native/localized game data where possible rather than a hand-maintained English-only catalog.

Likely first useful content:

1. **Blueprints:** build cost + verified player-facing build location/desk.
2. **Ordinary recipes:** ingredient requirements + crafting station/location.
3. **Existing descriptions:** retain vanilla effect/description text rather than replacing it wholesale.

Whether output quantities, energy/time costs, or secondary recipe metadata materially improve the Technology decision should be decided after representative examples are inspected; more data is not automatically better UX.

## Remaining research before creating a production repository

The product hypothesis is strong enough to continue, but the following questions should be closed first:

1. **Representative blueprint audit**
   - inspect several early/mid/late-game `ObjectCraftDefinition` unlocks;
   - confirm `needs`, `builder_ids`, `sub_zone_id`, and `out_obj` coverage;
   - include examples such as a common Workyard station, Church/Alchemy-Lab buildable, grave/church furniture, and at least one DLC blueprint where practical.

2. **Player-facing build-location mapping**
   - establish how each relevant builder/zone ID should be localized;
   - prefer native `ObjectDefinition` / zone labels or an existing host formatter;
   - identify exceptions instead of inventing a generic suffix/name heuristic.

3. **Representative ordinary-recipe audit**
   - verify how much useful data vanilla already contributes through `TechUnlock.GetTooltip`;
   - determine the smallest additional ingredient/station block that avoids duplication.

4. **Tooltip composition/layout**
   - inspect a technology with multiple visible unlocks and a long ingredient list;
   - reuse the accepted Technology width lifecycle where applicable;
   - verify mouse and combined gamepad tooltip presentation.

5. **Special unlock taxonomy**
   - inspect Work, Perk, Phrase, hidden/invisible, DLC and scripted cases;
   - explicitly define which categories are enriched and which remain vanilla.

6. **Purchase-dialog scope**
   - `TechUnlockDialogGUI.Open` renders unlock rows with child tooltip initialization disabled;
   - decide whether the tree hover/focus experience alone satisfies the MVP or whether purchase confirmation must also expose details. Do not broaden automatically.

## Decision

**Product hypothesis: SUPPORTED.**

There is repeated player evidence from multiple years that Graveyard Keeper's Technology UI forces external lookup, notes, trial-and-error purchases, or backtracking for information that is relevant at the moment of choosing a technology.

**Technical hypothesis: PROMISING, NOT YET PRODUCTION-READY.**

The key information owners and a shared mouse/gamepad tooltip seam are already present in GK 1.407, and a concrete vanilla blueprint-tooltip omission is statically verified. The remaining uncertainty is primarily about robust player-facing build-location semantics, representative coverage, and final information density rather than whether the concept is technically plausible.

**Repository decision:** keep work in `NikichMods/GraveyardKeeperResearch` for the next research step. Do not create the production mod repository until the remaining MVP data-mapping questions above are closed enough to define a narrow implementation and acceptance envelope.
