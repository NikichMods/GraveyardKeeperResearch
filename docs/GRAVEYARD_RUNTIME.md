# Graveyard objects and rating — GK 1.407

Status: **bounded static evidence**, checked 2026-10-22. This document records host facts; proposed product mechanics live separately in [the candidate notes](ECONOMY_DESIGN_CANDIDATES.md).

## Input identity and method

- `resources.assets` SHA-256: `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`.
- `Assembly-CSharp.dll` SHA-256: `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`.
- Read selected `game_data.objs_data`, `craft_obj_data`, `craft_data`, `items_data`, `techs_data` and Russian localization with UnityPy/TypeTreeGenerator, using the same input identity and loading method as [the economy extractor](../research/economy/audit_economy.py).
- Read IL of the installed assembly through reflection, without running gameplay methods: `WorldGameObject.get_quality`, `Item.GetBodySkulls`; inspected `ItemDefinition.type`'s nested enum.
- Recovered the earlier bounded unused-asset audit (`definitions.json`, `targeted.json`, `verified.json`, `closure.json`, dated 2026-10-22). Those temporary local inputs are not public game payloads in this repository. This was a targeted recovery, not a fresh bulk asset scan.

No runtime placement, save compatibility or proposed mechanic has been accepted by these checks.

## Prayer stations

| Definition | Russian localization | Authored quality |
| --- | --- | ---: |
| `pray_stand_wd` | Деревянная молельня | 2 |
| `pray_stand_stn` | Каменная молельня | 4 |

Construction definitions:

- `graveyard_builddesk:p:pray_stand_wd_place`: 7 `flitch`, 4 `nails`, 1 `book:book_hard:1`.
- `graveyard_builddesk:p:pray_stand_stn_place`: 4 `stone_plate_2`, 1 `stone_plate_3`, 1 `book:book_hard:2`.
- Technology `Stone praying stand` includes the stone station construction alongside `road_marble` and `lantern_3` construction.

These are existing quality-producing decorations, not literally valueless objects. The selected definitions do **not** establish an existing radius, grave ownership, skull conversion, active prayer interface or paid upgrade system. Those would be new mechanics requiring separate verification and implementation.

## Native grave rating boundary

For the body-present `QualityType` branch in `WorldGameObject.get_quality`, the inspected IL returns:

`min(floor(inventory decoration quality) - displayed red skulls, available white skulls)`.

The owner obtains skulls through `Item.GetBodySkulls(..., false)`. That method combines the body's own values and its contained items, clamps negative red/white totals to zero, and calculates available white skulls from durability. Its durability percentage is rounded up; values above 90 are treated as 100 before flooring the available-white result.

Consequences:

- Red-skull removal, white-skull addition and direct grave-quality addition are different interventions.
- For an unspoiled body, an actual one-red-to-one-white conversion increases this rating by exactly 1 at unchanged decoration: both arguments of `min` rise by 1. It is not +2. With decay/rounding or a different intervention, evaluate the actual available-white result instead of promising the same gain.
- Adding only white skulls is different: decoration may already be the limiting argument, so the rating gain can be zero.
- Changing the physical body could also matter after exhumation or resurrection. A proposed grave-only effect must not be described as a permanent organ/body modification.
- Empty graves, transient installation/removal state and generic object quality follow additional branches; the formula above is not a universal object-quality formula.

## Bounded unused-asset candidates

| Candidate | Confirmed evidence | Applicability limit |
| --- | --- | --- |
| `grave_top_sarcofag_mrb_1` | Item; quality 10; type 102, resolved as `ItemDefinition.ItemType.GraveFence`; locked manufacturing recipe; set/remove/fix/study references; three stage sprites | No technology craft unlock mention found in the bounded audit. Installation recipe presence does not prove compatible placement, visual composition or save/load. The name does not make it a gravestone-slot item |
| `grave_top_sarcofag_stn_1` | Inventory icon `i_grave_top_sarcofag_stn_1` | No corresponding item or recipe in the earlier exact-ID check; no complete placeable object established |
| `candle_6` | Item, icon and locked recipe | No use established in the selected object/construction/vendor/technology references; not proven to be a usable church candle or placeable decoration |
| `skull_candle` | Item, icon and recipe | No input use found in the earlier recipe check; complete placeable object not established |
| `candle_5` | Icon | No exact item established |

The marble sarcophagus manufacturing recipe consumes 4 `marble_plate_2`, 1 `marble_plate_3:3`, 3 `detail_2` at `mf_hammer_1`. Its quality 10 is a data fact, not proof of superiority to current DLC decorations. A cosmetic reward role remains possible without stronger numerical stats.

Existing columbaria, lanterns, paths and flowerbeds may be candidates for additional systemic roles, but are already used objects. No missing-content or new-function claim for them is established here.

## Additional bounded item/UI checks

2026-10-22: targeted `items_data`/Russian-localization inspection identifies `communion_cookies` as «Печенье для причастия». The selected `craft_data` has no recipe producing this exact item. Seller/buyer eligibility, a new recipe and relationship effects are not established. Its base-price field is not a guaranteed sale quote.

`GraveGUI.Redraw` in pinned [GraveGUI.cs](https://github.com/Kupie/GYK_DECOMP/blob/6abf79199d92482af1c7573870dd9a20ec2270b9/Assembly-CSharp/GraveGUI.cs) calls `body_panel.Draw(_body)` and displays grave quality. The buried-body skull panel is not a new proposed feature. Showing organ inventory/contributions would be an additional interface; usability/reachability is untested. See [Cultist semantics](PERK_MECHANICS.md#cultist) before changing visibility.

## Appearance identity seams — 2026-10-23

Status: **bounded static findings**, not a proven cosmetic implementation. Same pinned input hashes as above. Read these files at `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`: `Assembly-CSharp/Item.cs`, `GameRes.cs`, `BaseItemCellGUI.cs`, `ItemDefinition.cs`, `GraveGUI.cs`, `CraftComponent.cs`. Direct installed-assembly IL corroborates `Item.GetIcon` and `Item.OnAfterDeserialize`; selected native balance definitions were read locally.

- `Item` has `[SerializeField] GameRes _params`; `GameRes` contains serialized string-key/float-value lists and its setter accepts new keys. `Item(Item)` copies the parameter values, while the item JSON path includes serialized instance fields and recursively reconstructs inventory. This supplies a plausible namespaced numeric cosmetic-state carrier, not proof that every save/transfer/craft path preserves it.
- Sampled `grave_bot_stn_1`, `grave_top_stn_plate_2`, `grave_top_sarcofag_mrb_1`, `grave_top_sculpt_mrb_5` all have native `stack_count=1`. Do not generalize to all decorations or installations with stack-changing mods. For stackable items, [native stack equivalence](CRAFTING_INVENTORY_AND_TRADING.md#vanilla-stack-equivalence) ignores instance parameters.
- `Item.GetIcon` resolves definition/base-name icon paths rather than arbitrary cosmetic parameters. More importantly, `BaseItemCellGUI.DrawItem` directly assigns `_item_definition.GetIcon()`, resets native icon color from durability state, initializes tooltip data via `GetTooltipData(i, true)`, and redraws quality/additional icons. Patching only `Item.GetIcon` cannot establish all inventory presentation.
- `ItemDefinition.GetTooltipData(Item, bool)` receives the actual instance, providing a candidate per-item finish-description seam. This does not prove every tooltip family uses that route. Keep native quality, durability and price semantics separate from appearance.
- `GraveGUI.OnResourcePickerClosed` passes the selected instance to `CraftAsPlayer`; `FindGravePartPutCraft` matches exact input IDs. The selected `set_grave_top_stn_plate_2` recipe has `transfer_needs_to_wgo=true`; `CraftComponent` has a branch placing a supplied particular item into the grave inventory. The matching removal recipe outputs the base item definition, and generic completion generates results and has explicit durability handling. Consequently appearance preservation on removal must be traced/handled; it is not guaranteed by storing a parameter alone.
- Namespaced instance parameters and separate cosmetic item IDs are different architecture candidates. Separate IDs must be registered for loading and connected to exact-ID installation/removal paths; they are not a free workaround. No route is selected or runtime-tested here.

The hypothesis that an excavated grave is impossible to remove, and preservation under external movement mods, were not verified in this check. They must not be used as identity guarantees.
