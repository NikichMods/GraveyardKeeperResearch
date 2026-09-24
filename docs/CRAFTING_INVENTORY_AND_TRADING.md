# Graveyard Keeper 1.407 — Crafting, Inventory and Trading Internals

This document consolidates reusable accepted facts promoted from Crafting Planner, Specialized Storage, Better Save Soul Rebalance, Food & Drink Rebalance, and Who Buys This?.

## CraftDefinition and builder recipe model

**Status:** verified static + accepted runtime for representative project families.

`CraftDefinition` owns the common recipe data used by many crafting/project paths, including:

- `needs`
- `needs_from_wgo`
- `output`
- `craft_type`
- `sub_type`

`ObjectCraftDefinition : CraftDefinition` is the building recipe subtype. `GameBalance` exposes separate `craft_data` and `craft_obj_data` collections.

`BaseCraftGUI.CommonOpen(...)` builds visible lists from actual `CraftDefinition` instances. `CraftItemGUI.Draw(CraftDefinition)` receives the exact current definition and renders ingredients from `current_craft.needs`.

Normal construction desks use:

`MainGame.OpenBuildObjectGUI(build_desk) -> CraftGUI.OpenAsBuild(build_desk, CraftsInventory)`

Representative runtime evidence proved that builder cards expose exact `ObjectCraftDefinition` objects and that repair/clearing world projects can expose ordinary `CraftDefinition` objects with concrete `needs` and `change_wgo`.

**Evidence provenance:** Crafting Planner Probe 0.2.0, source `f11c6b3c9e20443431aaf3eb25339c78e5e711e6`, runtime log SHA-256 `3bf9cb3782b623e40c38ef70189f21de751585d2ea568c0b09ec3c8db9b29fce`.

**Applicability limit:** not every recipe-like system shares plain `needs` semantics. Mixed alchemy, survey/research, body/autopsy, prayer, refugee/scripted craft paths and other special systems require separate evidence.

## Craft-card focus callbacks

**Status:** verified static + runtime for mouse build focus.

`CraftItemGUI.OnMouseOvered()` is the mouse-hover callback. `CraftItemGUI.OnOver()` is the gamepad-focus callback and returns immediately when the GUI is not in gamepad mode.

Crafting Planner Probe 0.2.0 runtime-confirmed mouse focus on native build cards. Its earlier 0.1.0 instrumentation of `OnOver` must not be misread as mouse evidence.

Reusable implication: do not conflate mouse hover and gamepad focus merely because both ultimately select a craft card.

## Player inventory is not interaction inventory

**Status:** accepted runtime fact.

`MainGame.me.player.GetMultiInventoryForInteraction()` can include nearby object/world-zone inventories. It answers what the current crafting interaction can access, not what the player physically carries.

Crafting Planner runtime evidence observed one exact case with:

- player-only `flitch` count = 0;
- interaction/crafting-access count = 18;
- current chest count = 9.

For player-carried counts, `Item.GetTotalCount(item_id, count_in_bags:true)` is the native player/bag-oriented path used by the accepted research.

**Evidence provenance:** Crafting Planner Probe 0.1.0, source `f94be35330d17f3dccdfbe927d4f847fb5905498`.

## Storage-local inventory and native transfer

**Status:** verified static + accepted runtime.

`WorldGameObject.GetMultiInventoryOfWGOWithoutWorldZone(true)` builds an inventory view for that specific WGO and its bags without adding the world-zone inventory.

Vanilla chest movement keeps host ownership:

- `ChestGUI.GetMaxMoveCount(...)` -> destination `MultiInventory.CanAddCount(...)`;
- `ChestGUI.MoveItem(...)` -> `MultiInventory.MoveItemTo(...)`;
- `MoveItemTo` checks destination capacity, removes from source, then adds to destination.

Crafting Planner runtime evidence confirmed a bounded native move of exactly one missing item and no surplus move once the requirement was satisfied.

Specialized Storage independently confirmed that the real transfer/add paths read `ItemDefinition.stack_count`; changing only the visible/max-move calculation would not alter the actual add constraint.

**Evidence provenance:** Crafting Planner Probe 0.1.0; Specialized Storage accepted post-audit of 1.2.0, source `dc8d3b1209510d0c57afa4d2bcd0382f56091332`.

## Vanilla stack equivalence

**Status:** verified static fact for stackable items.

In GK 1.407, vanilla stack merge/capacity paths compare stack identity by exact item `id` for stackable items:

- `Item.CanAddCount(Item,...)` delegates to the item ID form;
- capacity counts matching `item.id`;
- `Item.AddItem(Item,...)` locates stackable destinations by matching ID and increments `value`.

Durability, per-instance params and worker state are not part of vanilla stack equivalence when `stack_count > 1`.

**Applicability limit:** do not infer that every non-stackable/unique item can be normalized this way.

## Craft renderer parallel-list invariant

**Status:** accepted runtime-backed UI invariant.

`CraftItemGUI.Draw(...)` constructs `_multiquality_ids` with one entry per physical `CraftDefinition.needs` item. `CraftItemGUI.Redraw()` later passes the fixed ingredient cells, the current `needs`, `_multiquality_ids`, and amount together to `BaseItemCellGUI.DrawIngredients(...)`.

Temporarily appending a display-only pseudo ingredient to the shared `CraftDefinition.needs` without extending the parallel list broke the crafting UI and caused a `CraftItemGUI.OnOut()` exception.

Reusable implication: renderer-only augmentation should be performed at the renderer argument boundary using copied parallel lists, and shared recipe definitions should not be mutated for presentation.

**Evidence provenance:** Better Save Soul Rebalance 1.1.0 rejection and accepted 1.1.1 fix; accepted source `d684b783408d23de212ccc1d91ac51add72edf93`.

## CraftComponent.DoAction current actor and delta-time coupling

**Status:** accepted fact.

Exact method:

`CraftComponent.DoAction(WorldGameObject other_obj, float delta_time, bool for_gratitude_points)`

The original method checks the current `other_obj` argument before assigning it to the component field. A Harmony Prefix therefore must use the current method argument, not `CraftComponent.other_obj`, to identify the current actor.

The same `delta_time` feeds:

- `TrySpendPlayerEnergy(other_obj, delta_time)`;
- `SpendPlayerSanity(other_obj, delta_time)`;
- craft progress.

`TrySpendPlayerEnergy` charges the current slice proportionally to `delta_time / craft_time`. Accepted exact-energy testing showed that scaling `delta_time` accelerated real-time completion while preserving the same total craft Energy for the tested manual craft.

**Evidence provenance:** Food & Drink Rebalance accepted 1.2.1, source `c638e83cacb71a2f079e6acdc418e6064748a0fa`; diagnostic source `3b704ca7566361f2efe427ca263ab2a785c84456`.

**Applicability limit:** this proves the inspected manual CraftComponent path; direct world-resource gathering uses a different path.

## Buff duration/removal primitives

**Status:** verified runtime facts.

For normal Player buffs:

- `PlayerBuff.end_time` is authoritative active-duration state;
- `BuffsLogics.AddBuff(string, float?)` extends an existing additive-overlay buff's duration rather than multiplying its resource strength;
- `FindBuffByID` resolves the active buff;
- `RemoveBuff` removes it, subtracts its resources, and refreshes UI;
- `MainGame.game_time` is the time basis used with `end_time`.

Observed conversion in `AddBuff`: source length is converted by `length / 450 * 60`.

**Evidence provenance:** Food & Drink Rebalance accepted runtime data.

## Authoritative vendor/item data

**Status:** verified static + accepted runtime.

`GameBalance` owns the loaded `items_data` and `vendors_data` collections and builds ID caches during balance loading.

The native player-to-merchant sale filter is:

`Trading.BuyableItemsFilter -> Vendor.CanBuyItem(itemDefinition, true)`

`Vendor.CanBuyItem` applies product/item validity, vendor tier, intersection with `VendorDefinition.GetProductTypes()`, and vendor-specific `not_buying` exclusions. `Vendor.CanTradeItem` is weaker and is not an equivalent sale predicate.

Quality variants are exact item definitions/IDs; buyer/exclusion behavior can differ between quality variants, so collapsing to a base ID is unsafe.

**Evidence provenance:** Who Buys This? runtime research source `97375f2d352c5ac1ca22bb186b4249cae16e4141`; target GK 1.407.

## Dynamic vendor product types

**Status:** accepted fact.

`VendorDefinition.GetProductTypes()` returns base product types plus conditionally enabled `additional_types` evaluated against current player state.

The accepted runtime fixture contained one such dynamic vendor (Tress paints). A cache of only currently-active types would therefore become stale after the relevant progression change.

Reusable implication: structural indexing may include possible conditional types, but current eligibility should call the native `GetProductTypes()` when the candidate is conditional rather than polling/rebuilding the whole catalog.

## Vendor construction is not a read-only query

**Status:** accepted fact.

`WorldGameObject.vendor` lazily creates a `Vendor`. Construction can initialize money, tier, inventory and save-backed state.

Therefore forcing `wgo.vendor`, manufacturing Vendor instances, or invoking broad vendor-fill routines merely to answer a read-only question is not semantically neutral.

Accepted buyer research derived the full catalog without vendor construction or save mutation.

## Gameplay-start catalog lifecycle

**Status:** accepted lifecycle seam for the inspected balance/vendor catalog.

Static load order places balance loading before save/world restore and WGO rescan. Runtime evidence observed `WorldMap.RescanWGOsList()` before `MainGame.OnGameStartedPlaying()`, and the accepted Who Buys This? research produced a stable full catalog at that seam without later structural changes during the observed session.

Reusable implication: `MainGame.OnGameStartedPlaying()` is a proven one-time initialization boundary for data that requires loaded GameBalance plus restored world/save objects, when the data itself is structurally stable for the session.

**Applicability limit:** do not assume every subsystem is final by this event; prove the owner-specific lifecycle before using it as a universal initialization hook.

## Known NPC identity and staged vendor presence

**Status:** accepted fact for the inspected trading lifecycle.

The save stores known NPCs in `MainGame.me.save.known_npcs`. `Flow_Talk` records talked-to NPCs, with `ObjectDefinition.npc_alias` resolving the stored identity.

Ordinary merchant visibility can therefore be queried from native known-NPC state.

Game of Crone staged trade proxies are different: staged vendor WGOs are progression-created/replaced and do not map directly to ordinary KnownNPC identity. Their current WGO presence is the accepted native progression signal for those proxy merchants.

**Applicability limit:** this is a specific staged-proxy pattern, not a universal rule for every DLC/vendor family.

## Standard item-tooltip seam

**Status:** verified seam.

`ItemDefinition.GetTooltipData(Item item = null, bool full_detail = true)` returns the standard `List<BubbleWidgetData>` used by ordinary item cells.

This is a reusable event-driven seam for appending native-style item tooltip information without per-frame UI replacement.

## Known consumers

- `NikichMods/Crafting-Planner`
- `NikichMods/SpecializedStorage`
- `NikichMods/BetterSaveSoulRebalance`
- `NikichMods/FoodAndDrinkRebalance`
- `NikichMods/WhoBuysThis`

Project-specific balance, buyer-display policy, specialized-storage classification, and planner product scope remain canonical in their owning repositories.
