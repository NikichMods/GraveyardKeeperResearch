# Inventory Slot Pinning — Initial Research

**Target:** Graveyard Keeper 1.407 (Windows/Steam, BepInEx 5)
**Research date:** 2026-09-28
**Status:** initial product/community research + preliminary static feasibility evidence. Exact vanilla slot-order owner/final-writer remains **BLOCKED** pending direct host inspection.

## Problem / observable goal

Community-requested outcome:

- let the player keep chosen carried items in a stable, predictable place in the personal inventory;
- flagship example: the Teleport Stone should not keep shifting visually as other items are added/removed;
- reduce repeated visual search and preserve a stable inventory mental map.

The requirement is intentionally phrased as an observable UX outcome rather than prematurely requiring one mechanism such as physically reserving vanilla list indices.

## Community evidence

### Exact request prevalence

Targeted web searches on 2026-09-28 did **not** surface an existing Graveyard Keeper 1 mod or a large cluster of posts using the exact formulation "pin/lock an item to an inventory slot".

That is not evidence that nobody has requested it. It means the exact need appears under-served / poorly indexed rather than obviously saturated.

### Closely matching pain is persistent

The broader inventory-order problem is well supported across the game's lifetime:

- 2018 Steam discussion: players explicitly complain about spending a large share of play time sorting inventory and ask for more inventory capacity.
  - https://steamcommunity.com/app/599140/discussions/0/1734336452600362586/
- 2019 Steam discussion "Chest sorting": a player cannot understand the game's order and wants a predictable ordering scheme; taking items out and putting them back does not solve the problem.
  - https://steamcommunity.com/app/599140/discussions/0/1755772262915791901/
- 2019 Steam guide/discussion exists specifically to reduce "inventory and storage frustration", emphasizing the burden of keeping carried inventory light and remembering where items are.
  - https://steamcommunity.com/app/599140/discussions/5/1638675549014900044/
- 2021 Steam discussion on bag priority: players describe item destination/order as opaque and frustrating, with repeated manual shuffling needed when bag capacity changes.
  - https://steamcommunity.com/app/599140/discussions/0/3109142236158635479/
- 2023 Reddit discussion: inventory management is described as a recurring task because of the volume of item types; replies independently call sorting messy/tedious.
  - https://www.reddit.com/r/GraveyardKeeper/comments/10pcwr6
- 2025 Steam discussion "Inventory ruining experience": notably, a player explicitly complains that items cannot be manually dragged/rearranged into the desired place.
  - https://steamcommunity.com/app/599140/discussions/0/594029097882137147/

This supports the **category need** strongly: players repeatedly want less shuffling, more predictable placement, and lower search/management overhead. It does not establish that absolute slot pinning is the only or universally preferred solution.

### Teleport Stone is an especially strong first-use case

The Teleport Stone is a persistent, frequently used carried utility item.

Existing mods specifically improve access to it:

- Beam Me Up Gerry: https://www.nexusmods.com/graveyardkeeper/mods/61
- Teleport Stone Quick Slot: https://www.nexusmods.com/graveyardkeeper/mods/130

This makes the Teleport Stone a good representative acceptance case, while the product should remain generic rather than hard-code one item.

## Existing inventory-QoL ecosystem

Current/recent Graveyard Keeper 1 mods confirm substantial appetite for inventory/storage QoL:

- More Inventory Slots — configurable player/container capacity:
  - https://www.nexusmods.com/graveyardkeeper/mods/100
- Where's Ma' Storage — broad storage/inventory overhaul:
  - https://www.nexusmods.com/graveyardkeeper/mods/62
- Specialized Storage — themed storage stack specialization:
  - https://www.nexusmods.com/graveyardkeeper/mods/177
- Teleport Stone Quick Slot — direct quick-slot use of the Teleport Stone:
  - https://www.nexusmods.com/graveyardkeeper/mods/130
- Beam Me Up Gerry — direct teleport menu while retaining Teleport Stone ownership:
  - https://www.nexusmods.com/graveyardkeeper/mods/61

The proposed mod should avoid becoming another all-in-one inventory overhaul. A narrow "stable place for chosen items" contract is differentiated and easier to compose with existing QoL mods.

## Product-scope questions to close before implementation

The initial feature should be designed around **stable visual location**, but these edge cases need explicit semantics:

1. **Pin identity**
   - pin by item definition / item ID;
   - or pin one physical instance;
   - quality variants and non-stackable/unique items must be considered separately.

2. **Absent item**
   - reserve an empty visual slot while the pinned item is absent;
   - or collapse the inventory and restore the preferred slot only when the item returns.
   - Absolute "same place every time" semantics imply a reserved slot; that is a stronger requirement and may need UI/model support.

3. **Multiple stacks**
   - whether one pinned item type may occupy multiple stacks;
   - which stack owns the pinned slot when stack limits split the item.

4. **Mutation paths**
   - pickup/reward;
   - crafting output;
   - purchase;
   - use/consumption;
   - transfer to/from chest;
   - sale;
   - stack merge/split;
   - save/load;
   - inventory-capacity changes.

5. **Inventory families**
   - primary player inventory should be the initial scope;
   - bags, toolbelt/equipment, world storage and vendor inventories should remain unchanged unless separately justified.

6. **Input / UX**
   - mouse and gamepad need a discoverable pin/unpin action;
   - vanilla does not expose general drag-to-reorder as an obvious supported player workflow, so do not assume drag-and-drop is the safest control;
   - a small pin/lock marker is likely useful, but presentation should follow the verified NGUI lifecycle rather than introduce a parallel inventory UI.

7. **Compatibility**
   - More Inventory Slots changes capacity and has historically touched inventory logic;
   - Where's Ma' Storage explicitly overrides inventory logic deeply and warns about conflicts with other inventory-overhaul mods;
   - Quick Stack / transfer mods may exercise add/remove paths in unusual sequences;
   - compatibility must be based on shared/native seams rather than mod-specific hard dependencies where possible.

8. **Separate adjacent feature: item protection**
   - preventing a pinned item from being quick-stacked, sold, destroyed, or transferred is related but **not equivalent** to keeping it visually fixed;
   - do not bundle protection automatically unless player evidence/product choice justifies it.

## Existing shared host facts

From the accepted shared research in this repository:

- MainGame.me.player.GetMultiInventoryForInteraction() is **not** player-only; it may include nearby/world-zone inventories.
- player-carried inventory must therefore be identified through the player-owned inventory path rather than an interaction-wide MultiInventory.
- native chest transfers use ChestGUI / MultiInventory.MoveItemTo(...), with capacity/remove/add behavior owned by native inventory logic.
- stackable-item equivalence on verified vanilla paths is based on exact item ID.

See docs/CRAFTING_INVENTORY_AND_TRADING.md.

## Preliminary static feasibility evidence

A current public BepInEx mod source provides useful non-proprietary integration evidence:

**Source:** p1xel8ted/Graveyard-Keeper-Mods at commit/ref ebac55b3fe58402ae7cd5c061d9eb5b0c8e610eb.

Relevant observations:

- src/WheresMaStorage/Invents.cs
  - constructs new Inventory(MainGame.me.player);
  - directly observes MainGame.me.player.data.secondary_inventory;
  - iterates inventory contents via ...data.inventory;
  - reads BaseInventoryWidget.inventory_data.inventory.Count and inventory_size.
- src/WheresMaStorage/Patches.cs
  - patches InventoryPanelGUI.DoOpening;
  - receives and manipulates MultiInventory;
  - directly inspects inv.data.inventory.
- src/MaxButtonsRedux/Patches.cs
  - patches InventoryGUI.OnPressedSelect;
  - patches InventoryGUI.OnItemPressedItem;
  - patches InventoryGUI.OnItemPressedInBag;
  - patches ChestGUI.OnItemSelect.

Source paths:
- https://github.com/p1xel8ted/Graveyard-Keeper-Mods/blob/ebac55b3fe58402ae7cd5c061d9eb5b0c8e610eb/src/WheresMaStorage/Invents.cs
- https://github.com/p1xel8ted/Graveyard-Keeper-Mods/blob/ebac55b3fe58402ae7cd5c061d9eb5b0c8e610eb/src/WheresMaStorage/Patches.cs
- https://github.com/p1xel8ted/Graveyard-Keeper-Mods/blob/ebac55b3fe58402ae7cd5c061d9eb5b0c8e610eb/src/MaxButtonsRedux/Patches.cs

These facts establish that player inventory data and inventory UI interaction seams are reachable from BepInEx/Harmony. They do **not** yet prove the correct slot-order mutation seam.

## Solution-space checkpoint

The observable goal is stable visual position for selected carried items. At least three materially different mechanisms could satisfy it:

### A. Model-level absolute slots / reserved holes

Store preferred physical indices and make the underlying player inventory represent holes/reservations.

**Pros**
- strongest interpretation of "this item lives in this exact slot";
- stable even when unrelated items appear/disappear.

**Risks / unknowns**
- current evidence does not establish that vanilla inventory data supports empty interior slots;
- placeholders could leak into save, transfer, crafting, vendor, capacity, or bag logic;
- broadest blast radius.

### B. UI-only visual remapping

Leave vanilla item storage order untouched and render selected items in fixed visual cells.

**Pros**
- avoids mutating inventory data and save semantics.

**Risks / unknowns**
- every input path must map visual cell to underlying item correctly;
- gamepad focus/traversal, mouse selection, tooltip ownership, transfer actions and redraw lifecycle may all depend on native cell order;
- risks building a second inventory-order system on top of vanilla.

### C. Preference map + canonical post-mutation reorder

Persist a small preferred-order/slot map and, at a verified native mutation or redraw boundary, reorder the player inventory through the game's real data structure before vanilla draws/consumes it.

**Pros**
- potentially the narrowest mechanism if vanilla UI simply enumerates the player inventory list;
- keeps normal item objects and transfer logic;
- can compose with vanilla redraw instead of replacing the UI.

**Risks / unknowns**
- absolute slots may be impossible without holes; this may naturally provide a pinned region/stable relative order rather than true arbitrary coordinates;
- exact add/remove/final-writer path is not yet established;
- reorder timing must not race native compaction or serialization.

### Current direction

**Do not select a production mechanism yet.**

Option C is the leading low-complexity candidate **if** direct host inspection proves that list order is the canonical display order and remains stable after native mutation/save-load. Option A or B should only be paid for if absolute empty-slot reservation is an accepted product requirement and the host model requires it.

## Technical verdict

**Preliminary feasibility: high, but not yet proved for exact absolute-slot semantics.**

What is already established:
- inventory data is reachable and mutable enough for existing BepInEx mods to alter inventory behavior;
- inventory UI/open/select methods are patchable;
- player inventory can be distinguished from interaction/storage inventories.

What remains unknown and blocks production work:
1. canonical owner of primary-player item ordering;
2. whether the underlying collection is compact-only or can represent empty interior slots;
3. final writer/compactor after add/remove/merge/transfer;
4. save/load serialization order;
5. mapping from data order to BaseInventoryWidget cells;
6. gamepad focus/order ownership;
7. whether capacity-changing mods rebuild/rewrite the same collection.

**Production evidence gate: BLOCKED.**

The next research step is direct static inspection of GK 1.407 host code: Item, player inventory data, Inventory, MultiInventory, InventoryGUI, InventoryPanelGUI, BaseInventoryWidget, add/remove/move methods and save serialization. Only if static inspection cannot close a material uncertainty should a narrow diagnostic probe be built.

## Working product name

**Fixed Item Slots**

Rationale:
- immediately communicates the actual player-facing outcome;
- generic, not Teleport-Stone-specific;
- concise enough for Nexus and repository/DLL naming;
- natural technical identity: FixedItemSlots / FixedItemSlots.dll.

This is a working name until the product semantics are finalized.
