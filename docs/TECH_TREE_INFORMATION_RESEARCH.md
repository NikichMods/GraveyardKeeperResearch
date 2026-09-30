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

**Full-audit update:** build-location mapping is no longer blocked on a manual ID table. The exact 1.407 balance audit below shows that every authored-visible Technology blueprint has exactly one `builder_id`, every such ID resolves to a real `ObjectDefinition`, and the host already uses `GJL.L(ObjectDefinition.id)` as a player-facing object-name path in both `WorldGameObject.GetUniversalObjectInfo()` and standard item crafting-location presentation. `sub_zone_id` is a separate placement restriction and should not be exposed as a guessed area label.

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

## Full static balance audit

**Status:** accepted static research result for the user-supplied Graveyard Keeper 1.407 files.

Inputs:

- `resources.assets` SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
- `Assembly-CSharp.dll` SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`;
- pinned host-source cross-check: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

Because the shipped asset has `m_EnableTypeTree = false`, the serialized `game_data` schema was reconstructed from the matching managed assembly with a reproducible research toolchain. The proven compatible parser pair is `UnityPy 1.23.0 + TypeTreeGeneratorAPI 0.0.6`; the research repository now contains a short-lived-artifact workflow for reproducing that tooling without committing game files.

The full `game_data` object deserialized successfully. Relevant collection sizes:

| Collection | Count |
| --- | ---: |
| Technologies | 187 |
| Ordinary crafts | 2,101 |
| Object/build crafts | 533 |
| Object definitions | 1,318 |
| Item definitions | 1,157 |
| Perks | 44 |
| Works | 3 |
| Technology branches | 8 |

### Current Technology unlock population

Across all 187 technologies:

- 508 authored craft unlock references exist; 342 are authored-visible;
- 15 Work unlock references exist; all 15 are authored-visible and resolve;
- 39 Perk unlock references exist; 38 are authored-visible and resolve;
- no serialized Technology contains a Phrase unlock in the current 1.407 data;
- no technology has more than three authored-visible unlocks, so the vanilla `MAX_VISIBLE_UNLOCKS = 3` limit currently truncates **zero** Technology nodes;
- every Technology has 1–3 vanilla-visible unlocks: 53 have one, 60 have two, and 74 have three.

Of the 342 visible craft unlocks:

- **105 are ObjectCraftDefinition blueprints**;
- **237 are ordinary CraftDefinition recipes**.

The only unresolved authored craft reference in the entire dataset is `@stone_plate_3b` under `Stone carving`; it is explicitly authored invisible and therefore does not enter the vanilla-visible Technology UI.

### Blueprint coverage

For all **105/105 visible blueprints**:

- the blueprint ID contains the native blueprint-style `:` form used by `TechUnlock`;
- `out_obj` is non-empty;
- exactly **one** `builder_id` is present;
- that `builder_id` resolves to an existing `ObjectDefinition`.

There are only 13 unique builder objects for all visible Technology blueprints:

- `mf_wood_builddesk` — 21;
- `alchemy_builddesk` — 19;
- `souls_builddesk` — 14;
- `church_builddesk` — 12;
- `morgue_builddesk` — 9;
- `graveyard_builddesk` — 7;
- `mining_builddesk` — 6;
- `cellar_builddesk` — 6;
- `garden_builddesk` — 5;
- `tree_garden_builddesk` — 2;
- `vineyard_builddesk` — 2;
- `beegarden_table` — 1;
- `cremation_builddesk` — 1.

Build materials are present in **102/105 (97.1%)** visible blueprints. The three empty-`needs` exceptions are:

- `Rules of burning -> mining_builddesk::lantern_network`;
- `Rules of burning -> mining_builddesk:p:lantern_place`;
- `Garden beds -> garden_builddesk:p:garden_empty_place`.

These should naturally omit a requirements row rather than invent a zero-cost/material statement.

Ninety visible blueprints are `BuildType.Put`; 15 use `BuildType.None` for special systems/upgrades such as the lantern network, zombie mine/quarry structures, the well pump, and Better Save Soul structures/extensions. This argues for wording based on the authoritative **builder/menu owner** rather than assuming every ObjectCraftDefinition is an ordinary placeable construction.

Eleven `out_obj` IDs do not resolve to standalone `ObjectDefinition` records; all are in the special `BuildType.None` family. This does **not** block the MVP because the unlock already has its native title and its authoritative builder/requirements data. Do not make `out_obj -> ObjectDefinition` resolution a requirement for basic tooltip enrichment.

Only 13/105 blueprints have a non-empty `sub_zone_id`. The observed values are placement constraints such as `pray_stand`, `containers_sub_zone`, `lantern_sub_zone`, and `church_wall_obj_subzone`. Host code consumes this field in build-grid/sub-zone placement logic. It is not required to answer which build desk owns the blueprint and should stay out of the first user-facing tooltip.

### Ordinary recipe coverage

For the **237 visible ordinary recipe unlocks**:

- **221/237 (93.2%)** have non-empty `needs`;
- **236/237 (99.6%)** have non-empty `craft_in`;
- every one of the 33 unique visible `craft_in` station IDs resolves to an existing `ObjectDefinition`;
- **232/237 (97.9%)** have non-empty `output`.

The 16 empty-`needs` recipes are not one generic missing-data failure. They include body-part extraction crafts, automatic/growing production entries, and `fake_global_craft`. The correct generic policy is therefore: show requirements when `CraftDefinition.needs` is non-empty; otherwise do not invent ingredients.

Eight visible recipes do not reach a normal resolvable physical-output item on the current `TechUnlock.GetTooltip` path and therefore receive little/no native detail beyond their unlock title:

- `Zombie woodcuttering -> zombie_sawmill_unfinished_place`;
- `Browncastle -> honey_production`;
- `Grape farming -> garden_grapes_growing`;
- `Grape farming -> garden_hop_growing`;
- `Important parts -> ex:mf_preparation_1:brain`;
- `Important parts -> ex:mf_preparation_1:heart`;
- `Important parts -> ex:mf_preparation_1:intestine`;
- `soul_sins_2 -> fake_global_craft`.

Together with the 105 blueprint unlocks, this means **113/342 (33.0%) visible craft unlocks** currently hit a structurally sparse/title-only Technology-tooltip path.

### Exact recipe station versus vanilla output-item station list

For an ordinary craft unlock, vanilla `TechUnlock.GetTooltip` does not present that exact recipe's `CraftDefinition.craft_in`. It selects the first physical output item and delegates to `ItemDefinition.GetTooltipData(item, false)`.

That item tooltip obtains `crafted_at` from `GameBalance.GetItemCraftsIn(output_item_id)`, whose cache aggregates stations across every eligible recipe that produces the item.

The full data audit compared the exact Technology recipe's `craft_in` against the current item-level aggregate for the 229 visible recipe unlocks that reach a normal output item:

- **91/229** match exactly;
- **138/229 (60.3%)** have an exact recipe station set that is a strict subset of the output item's aggregate station list;
- there were no disjoint or contradictory exact-station cases.

Example:

- `Iron -> ingot_metal` is authored with `craft_in = [mf_furnace_0]`;
- the output-item crafting-location cache contains `mf_furnace_0, mf_furnace_1, mf_furnace_2`.

This is not necessarily wrong for a generic **item** tooltip—it answers where the item can be made in general. It is less precise for a **Technology unlock** tooltip, where the question is where this newly unlocked recipe belongs.

Engineering/product implication: a future Technology tooltip should prefer the unlock's exact `CraftDefinition.craft_in` for its recipe-location row rather than blindly inheriting the broader output-item `crafted_at` list.

### Localization / location mapping closure

The prior build-location blocker is closed enough for production design:

- all visible blueprint `builder_id` values resolve to `ObjectDefinition`;
- all visible ordinary-recipe `craft_in` station IDs resolve to `ObjectDefinition`;
- standard item crafting-location code already localizes station `ObjectDefinition.id` with `GJL.L(id)`;
- `WorldGameObject.GetUniversalObjectInfo()` independently uses `GJL.L(this.obj_def.id)` as the object's player-facing header.

Therefore a future mod can derive station/build-desk names from the same host localization identity rather than maintaining an English/Russian mapping table. Runtime acceptance still needs to verify the final strings and layout, but the data model itself is no longer BLOCKED.

### Tooltip-density stress cases

The full dataset also identifies representative worst-case Technology nodes for the first visual candidate. Useful stress cases include:

- `Vegetable set` — three recipes, nine ingredient rows, multiple station IDs;
- `Embalm 1` — one blueprint plus two recipes, nine ingredient rows;
- `The Beginning Of Alchemy` — three blueprints, nine build-material rows;
- `Illumination of faith` — two blueprints plus one recipe, eight material rows and mixed location ownership;
- `Clay` / `Stone carving` — mixed blueprint + recipe nodes;
- Better Save Soul nodes — special `BuildType.None` blueprint/upgrades.

These cases are preferable to a separate research DLL merely for data coverage. The remaining unknown is visual composition/ergonomics in the real Technology tooltip, which is best proved by the first narrow production candidate using the already-verified tooltip lifecycle.

## Research closure before production bootstrap

The research questions that originally blocked creation of a production repository are now closed:

1. **Product need:** supported by repeated player evidence.
2. **Canonical data owners:** verified for Technology, recipe and blueprint data.
3. **Mouse/gamepad tooltip seam:** verified through `TechUnlock.GetTooltip`.
4. **Blueprint data coverage:** complete enough for a generic data-driven rule.
5. **Recipe data coverage:** complete enough for a generic data-driven rule with explicit empty-list handling.
6. **Build/craft location identity:** can be derived from native ObjectDefinition IDs and GJL localization rather than a hand-written table.
7. **Special cases:** bounded and enumerable; no broad hidden dependency engine is needed.

A separate runtime **research** DLL is no longer justified before production bootstrap. Visual density and exact final wording should be tested through a narrow first production candidate because those are product/runtime acceptance questions, not unresolved balance-data ownership questions.

### Narrow MVP direction after the audit

The least-complex adequate first scope is now:

1. **Blueprint unlock**
   - keep the native unlock title/description;
   - add a requirements/materials block only when `needs` is non-empty;
   - add one localized builder/build-menu location derived from the single `builder_id`;
   - do not expose `sub_zone_id` or infer hidden practical dependencies.

2. **Ordinary recipe unlock**
   - keep the useful native output description;
   - add the exact recipe's `needs` when non-empty;
   - present the exact recipe `craft_in` station list rather than the broader output-item aggregate when composing Technology-specific location information;
   - handle special no-output/autopsy/production records conservatively from their authored data rather than forcing them through a normal-item assumption.

3. **Work / Perk**
   - leave vanilla behavior unchanged for the first version; all current visible references resolve structurally.

4. **Phrase**
   - no first-version path is needed for current 1.407 balance data because no Technology contains a Phrase unlock.

5. **Preserve visibility/progression**
   - operate only on the same visible unlocks vanilla presents;
   - do not expose `@`-hidden unlocks, hidden technologies, story state, or DLC-gated content prematurely.

The first production evidence gate must still make the exact content-composition/final-writer behavior reviewable before mutation, but the **project-level technical feasibility question is no longer blocked**.

## Runtime Work / Perk enrichment audit — 2026-09-30

**Status:** accepted runtime evidence for Graveyard Keeper 1.407.

Diagnostic identity:
- source: `64a3751a67e3a1950c2ccfb9970e7991fb304daf`;
- GitHub Actions run: `36714753757`;
- DLL SHA-256: `6b035058f73a30d270ba54d354b5048a7a7fdf0c0a54c90d4d510d92e57901df`.

The user ran the read-only diagnostic with the current mod set and Russian localization, opened the Technology tree once, and returned the complete BepInEx log. The audit completed normally:

- 15 authored-visible Work references;
- 38 authored-visible Perk references;
- 2 Work references have no authored description;
- 11 Perk references have no authored description.

### Sparse Work unlocks

The two visible Work unlocks without authored descriptions are:

- `t_diamond` — object group resolves to `dungeon_source_diamond`, which drops `faceted_diamond`;
- `t_marble` — object group resolves to `marble_heap_mid_1` / `marble_heap_mid_2`, which drop `marble`.

This is sufficient to support a minimal action-oriented description such as “can now be mined/collected” without exposing a guessed placement label.

### Sparse gathering-style Perks with proved native consumers

Nine visible `show=false` Perk unlocks with no authored description have direct current-data consumers:

- `p_t_gold_ore`: enables gold-nugget chance in both iron-to-small processing recipes and in `steep_iron` mining;
- `p_t_silver_ore`: enables silver-nugget chance in the same two iron-processing recipes and in `steep_iron` mining;
- `p_t_lifestone`: gates limestone/lifestone drops from `steep_coal`;
- `p_t_sulfur`: gates sulfur drops from `steep_coal`;
- `p_t_beeswax`: gates beeswax drops from the bee house and bee-tree harvest objects;
- `p_t_bee`: gates bee drops from the bee house and bee-tree harvest objects;
- `p_t_butterfly`: gates butterfly drops from small flowers during `IsDay()`;
- `p_t_moth`: gates moth drops from small flowers during `IsNight()`;
- `p_t_maggot`: gates maggot output from `peat_from_waste`.

Product implication: these unlocks can receive short source/action explanations derived from the proved native consumer rather than a wiki-style use catalog. Do not include exact chance percentages in the first tooltip wording unless separately accepted.

### Explicit exclusions / unresolved sparse Perks

Two sparse visible Perks must **not** be given the same inferred wording yet:

- `p_t_old_books`: the audit found no consumer in the inspected native craft/object/drop expression seams. Its exact effect remains unresolved.
- `p_t_pyrite`: current runtime data is internally inconsistent. The Perk ID is `p_t_pyrite`, the coal-drop expression reads `Ppar("p_t_pyrite")`, but `PerkDefinition.output_res` is `p_t_pirit=1`. Static 1.407 host code proves `GameSave.UnlockPerk` adds `output_res` to player parameters. Therefore the Technology's direct unlock path does not prove that the parameter consumed by the coal-drop expression becomes set. An informational mod must not claim “pyrite can now drop” from this evidence, and must not silently fix the gameplay mismatch.

The diagnostic's broad string search also reports `p_t_bee` against `p_t_beeswax` expressions because `p_t_bee` is a substring of `p_t_beeswax`. Those lines are diagnostic-search false positives; the exact `p_t_bee` consumer set is the bee-drop expressions.

### Ordinary visible Perks

The remaining visible Perks already have authored descriptions. The audit additionally confirms normal `stars`, `output_res`, craft `linked_perks`, and direct output/drop consumers for many of them. The first enrichment pass should preserve those vanilla descriptions rather than replacing them with a second generated explanation.

### Production implication

The least-complex Work/Perk enrichment policy is now:

1. preserve authored Work/Perk descriptions when present;
2. add a short action/source description only to sparse visible unlocks whose current 1.407 native consumer is proved;
3. leave `p_t_old_books` and `p_t_pyrite` vanilla/sparse until their exact behavior is separately resolved;
4. do not expose hidden `@` unlocks, future uses, dependency graphs, or guessed story/location information.

## Decision

**Product hypothesis: SUPPORTED.**

There is repeated player evidence from multiple years that Graveyard Keeper's Technology UI forces external lookup, notes, trial-and-error purchases, or backtracking for information that is relevant at the moment of choosing a technology.

**Technical hypothesis: SUPPORTED FOR PRODUCTION BOOTSTRAP.**

The full 1.407 balance audit verifies sufficient generic data coverage for a narrow, data-driven Technology-tooltip mod. Blueprint and ordinary-recipe location ownership can be derived from native IDs/localization; the important sparse/special cases are bounded; and no separate wiki-style database or runtime research harness is required for the initial scope.

**Repository decision:** the research-only hold is lifted. A dedicated production repository is now justified once the user chooses to proceed. The first production candidate must still follow the normal per-change READY/BLOCKED gate and real-game visual acceptance; this conclusion authorizes project bootstrap, not unreviewed production mutation.


## Static asset inspection checkpoint — resources.assets

**Input:** user-supplied Graveyard Keeper 1.407 `resources.assets`  
**SHA-256:** `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`  
**Observed Unity serialized-file version:** `2020.3.17f1` / serialized-file format 22.

Direct serialized-file inspection established:

- the file is a valid Unity serialized asset file;
- its serialized metadata reports `m_EnableTypeTree = false`;
- the named `game_data` MonoBehaviour is present;
- `game_data` path ID: `150254`;
- serialized object byte range begins at file offset `65329800`;
- serialized object size: `4352660` bytes;
- the MonoBehaviour name `game_data` appears at the expected base-object offset, confirming object identification;
- its script pointer is external rather than a locally embedded MonoScript object.

**Research implication:** the balance payload is present, but the custom field schema is intentionally absent from this asset because TypeTree data is disabled. Correct deserialization should therefore use the managed type schema rather than reverse-engineering field boundaries from the 4.35 MB payload by heuristics.

**Checkpoint closure:** the matching `Assembly-CSharp.dll` was subsequently supplied (SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`). No `Assembly-CSharp-firstpass.dll` or runtime research DLL was required. The managed schema plus CLR/Unity reference assemblies allowed the full `game_data` object to deserialize successfully; the accepted results are recorded in the full static balance audit above.
