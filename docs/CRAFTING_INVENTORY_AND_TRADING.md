# Graveyard Keeper 1.407 — Crafting, Inventory and Trading Internals

This document consolidates reusable accepted facts promoted from Crafting Planner, Specialized Storage, Better Save Soul Rebalance, Food & Drink Rebalance, and Who Buys This?.

## CraftDefinition and builder recipe model

### Actual vendor tier commits versus temporary projections — 1.407

On the pinned Assembly-CSharp SHA-256 e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8, Vendor.TryLevelUpVendor() returns true after committing cur_tier + 1 and its native analytics event. Vendor.OnEndOfDay invokes it after TradeWithBank and before its success return. Vendor.cur_tier reads/writes vendor_data parameter vendor_tier. GetMoneyNeededForVendorLevelUp and GetTotalGoodsOnNextTier also temporarily increment that same property and restore it while projecting values. Thus observing the setter or every vendor_tier write would misclassify temporary projections as real upgrades. Observe the successful semantic method, verify actual before/after increase, and defer dependent work until native processing settles. Existing Vendor.vendor_data is readable without constructing/filling another Vendor. ResearchableAlchemyRecipes .54 consumes this seam for known-Clotho research discovery; its per-save policy/tests and installed event acceptance limits belong to that project.

### Station-input spend display and QueueEverything — 1.407

Verified static/focused executable evidence (2026-10-15): CraftItemGUI.Redraw assigns txt_spend from CraftDefinition.GetSpendTxt. Native ingredient rows use needs; native spend text handles only fire among needs_from_wgo, so another station currency requires its own scoped presentation. ResourceBasedCraftGUI uses the native (sci) science symbol. Installed QueueEverything 2.2.1 (owner p1xel8ted.gyk.queueeverything, DLL SHA-256 fc90e74404ac8b1ff3e540b414f0e2d5d69da10808b0b5a52ad382aa888442dd) additionally replaces GetSpendTxt's result with its own calculation, also filtering station inputs to fire. Earlier result extensions can therefore be erased despite correct native payment data.

ResearchableAlchemyRecipes 0.1.46 returns missing prices; its focused regression reproduces the loss using the actual installed QueueEverything postfix and real Harmony ordering, then passes after its own scoped science addition is ordered after that owner at Priority.Last. The executable fixture supplies empty vanilla text to avoid unrelated Unity calculations; competing postfixes are actual code. Sufficient/insufficient science, unrelated craft/station fallthrough and unpatch baseline are checked. This establishes the final-writer dependency, not universal compatibility or final UI rendering; consumer exact source/candidate/readability acceptance belongs to that project's VERIFIED_RUNTIME_DATA.md and TEST_LOG.md. No foreign binaries or bulk source are published.

### Ordinary source visibility and grouped cards — 1.407

Verified static ownership plus installed manual observation: CraftComponent.FillCraftsList reads GameBalance.GetCraftsForObject; BaseCraftGUI.CommonOpen filters the station list with GameSave.IsCraftVisible and allowed craft type. Ordinary CraftType.None display is separate from BaseCraftGUI.CanCraft material/condition sufficiency. CraftGUI can merge variants into one CraftItemGUI, retaining all definitions in private _possible_crafts; craft_definition alone is not the whole displayed source set. WorldMap.GetWorldGameObjectsByObjId reads the existing object list and returns all matching instances without spawning them.

ResearchableAlchemyRecipes Source Eligibility Probe 0.1.2, source `5020e89f71cef61704eb6cdc103aae3f12f87840`, installed DLL SHA-256 `3d8426ff867338fe84b7da2b0d2333b059129fcb555c20fbe34940b1d28df10f`, captures an actual opened idle table_book_constr on the accepted 1.407 host hashes. Current-launch log SHA-256 `84038132a9b0ccd2d6c59e35ad8ce8054b628a6ca80302b7114f13a1ac8be2eb` has READY/TRIGGER/RESULT/DONE, no source/probe errors and exact predicted/native-list/grouped-card ID agreement for all sixteen investigated alchemy-product source groups: eight positive groups (nine definitions) and eight zero-visible groups. The shown station is indexed and physically enabled, not removed/removing/tutorial-blocked, with native CanInteractCraft=true. No patches, native menu opening, unlocks or saved diagnostic state are used by the observer.

Limits: one real loaded save and source window, not all stations, all early/absent/busy states or a universal source-access policy. CraftComponent.CanInteractCraft also tests busy/removing/interaction-type inputs; choosing whether temporary work suppresses a mod's research-topic awareness is product policy, not established by the idle capture. Product coverage and implementation acceptance remain owned by ResearchableAlchemyRecipes docs/AWARENESS_SOURCE_AUDIT.md and docs/TEST_LOG.md.

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

## Move Objects 0.6.0 structural compatibility checkpoint — 2026-10-14

Read-only exact installed GYKMoveMod.dll inspection: BepInPlugin PandaModding.gyk.moveobjects / Move Objects Mod / 0.6.0; SHA-256 c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0. MoveModeController invokes MoveSelectionHelper.TryGetActiveRuntimeStateReason, which rejects CraftComponent.is_crafting with Status_CannotMoveActiveCraft. WGOMoveHelper.MoveExistingWGO moves the existing WGO transform and synchronizes grid/zone state; it is not a new WGO replacement on this selected path. These are static facts for this exact artifact, not installed table-move/save or universal movement-mod acceptance. Consumer ResearchableAlchemyRecipes 0.1.42 schedules idle inventory persistence, active vanilla/research refusal, same-table refund then idle move and saved position/settled knowledge; its RESEARCH_WORK_LIFECYCLE.md owns the actual return. No mod change or new runtime build follows from this checkpoint. Private bounded IL is retained in the consumer's ignored work/palette.

## Study-table sample boundary and native work ownership — 2026-10-12

**Installed cancellation/removal distinction, 2026-10-14:** ResearchableAlchemyRecipes exact 0.1.42 logs now confirm ordinary vanilla rope-study Cancel returns the sample and faith as collected drops but leaves station science spent (27->26). Real native removal assignment also returns rope/faith, while the later table DestroyMe sees science 17 and drop_inventory_after_remove=false. A source recipe work instead retains its custom unpaid-refund metadata after removal assignment in that candidate, revealing that Cancel is not this interruption owner. Source-specific acceptance/return hashes belong to that consumer's TEST_LOG.md.

Pinned-assembly follow-up resolves the owner: WorldGameObject.MarkForRemoval calls CraftComponent.StartRemovalCraft with the native removal definition. For an active craft this returns current definition.needs as inventory drops, sets craft_amount=0, replaces current_craft by its removal argument, resets work progress and queue; it does not return needs_from_wgo or invoke Cancel. Cancelling the removal mark uses CancelRemovalCraft -> End. Native End retains positive Item parameters, removes zero parameters, then clears current work when amount<=1. This explains the returned n=0/job=none sequence. A consumer refund for interrupted manual work must account for this distinct successful replacement, not merely patch Cancel or infer refund from final DestroyMe. Do not treat this as a general new-table science transfer rule or claim support for all direct object-deletion/modded paths.

2026-10-14 pinned-assembly lifecycle follow-up: CraftReally removes needs_from_wgo before setting is_crafting; ProcessFinishedCraft later calls GameSave.OnFinishedCraft and End, without deferring that resource debit. Cancel drops _cur_craft_items_used and calls End but does not refund needs_from_wgo. Direct native callers include CraftGUI.NeedToCancelCraftsDialog's callback and CraftQueueGUI.OnDeleteItemPressed; this is not proof that every study UI exposes a convenient cancel control. Native GetSerializedCraftComponent/DeserializeCraftComponent persist/restore active work, current craft ID and amount; SerializableWGO.item owns the table Item alongside native work state. Actual Item binary serialization retains namespaced fee metadata in the consumer's bounded fixture, not a new full installed save result.

Direct WorldGameObject.DestroyMe does not call Cancel, handles drop_inventory_after_remove and disables craft components. On the accepted resources hash mf_alchemy_survey has drop_inventory_after_remove=false, inventory_preset=surv and craft_preset=survey_table_back. Its direct removal path therefore does not drop table inventory, but actual player-visible demolition sequence, any preceding cancellation and consumed-sample/faith/science outcome require runtime observation. ResearchableAlchemyRecipes 0.1.42 separately refunds only its own ordinary cancellation and observes vanilla/removal; no destruction compensation or restriction is selected. Private bounded IL/data inspection is retained in that consumer's ignored work/palette; its RESEARCH_WORK_LIFECYCLE.md owns product decisions and acceptance limits.

**Status:** verified static control flow in the pinned 1.407 source; new mod integration has no runtime acceptance.

Source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, `ResourceBasedCraftGUI`, `BaseCraftGUI`, `CraftGUI`, `CraftComponent`, `GameBalance` and `CraftsInventory`. Matching accepted local resources.assets/Assembly-CSharp.dll identities were rechecked; direct balance inspection identifies `mf_alchemy_survey`, inventory preset `surv`, decoration preset `survey_table_back`, and additional header item `science`.

- `ResourceBasedCraftGUI.Open` derives selectable IDs from each craft's first `needs` item; `ChooseItem` opens an inventory resource picker. `OnResourcePickerClosed` resolves the selected item back to a craft by that first input. This is a sample-oriented selection path, not a product-topic catalog.
- `CraftComponent.FinishCurrentCraft` routes Survey, except `SurveySciencePoints`, through `ShowSurveyCompleteWindow`. That method dereferences `craft.needs[0]`, obtains its item details and continues completion through the dialog callback. `ProcessFinishedCraft` also returns the studied sample, including a fallback drop of the first input when `_current_item` is null. Removing the target input alone does not implement sample-free Survey safely.
- The common execution owner is `BaseCraftGUI.OnCraft -> CraftComponent.CraftAsPlayer -> Craft/CraftReally`. It consumes interaction-inventory `needs` and station `needs_from_wgo`; `DoAction` owns manual progress and energy. Completion applies native outputs/scripts/events, calls `GameSave.OnFinishedCraft`, then ends the work. These mechanisms are not restricted to Survey.
- `CraftGUI.OpenCraftList` uses `CraftType.None` through `ProceedOpen/CommonOpen`; craft cards support explicit icons and custom names even without a real item output. This supports investigating a separate native work definition rather than changing the material Survey contract; it does not yet prove a usable table-entry UI.
- Craft identity must survive reload: `GetSerializedCraftComponent` stores the current craft ID; `DeserializeCraftDefinition` resolves it through GameBalance. Queue entries also resolve IDs through GameBalance. A definition that exists only in a UI list is therefore insufficient for resumable work.
- `GameBalance.GetCraftsForObject` caches station lists; `FillCraftsList` clears and repopulates component lists from that owner. `CraftsInventory.additional_crafts` is consumed only by its object/build list, not `GetCraftsList`. Do not assume arbitrary transient additions automatically become registered, reloadable ordinary crafts.

### Active-station menu access and cancellation reachability — static 2026-10-14

Direct IL inspection of the accepted Assembly-CSharp.dll identity verifies that CraftComponent.CanInteractCraft returns false when is_crafting is true and the station cannot insert a zombie. CraftComponent.Interact checks this predicate before calling GUIElements.OpenCraftGUI. That GUI dispatcher independently returns for an enabled active non-zombie craft component before opening a station window. Same-hash resources inspection gives mf_alchemy_survey.can_insert_zombie=false. These upstream checks matter even though CraftGUI.Open can offer NeedToCancelCraftsDialog for non-enqueueable current work and CraftQueueGUI.OnDeleteItemPressed can cancel its current queue row: callback existence does not prove that the player can reach that UI from an active station.

Status: native static control flow, not an installed cancellation-access observation or universal modded-input claim. Consumer ResearchableAlchemyRecipes's current table-entry prefix also falls through on active work; its RESEARCH_WORK_LIFECYCLE.md owns the open product need/control decision and accepted diagnostic F9/removal-refund limits. No global busy-interaction change is implied. Private IL/tools remain in the consumer's ignored work/palette; no host source or assets are published.

### Registration, table routing and completion continuation — 2026-10-12

Further pinned-source inspection identifies a static integration path, still without installed-runtime acceptance:

- `GameBalance.LoadGameBalance` loads `game_data`, then invokes `CreateIDsCache`, item/tool caches and `CreateCraftsCache`. Adding idempotent ordinary CraftDefinition records to this GameBalance before `GameBalanceBase.CreateIDsCache` allows the following native construction to include both their IDs and station membership. Scope such a hook to GameBalance; the base cache method is a shared owner. `AddData`/`AddDataUniversal` alone do not refresh an already-created ID cache.
- Stable job definitions must be registered independently of which topics are currently offered, so a paid in-progress job remains resolvable after reload. UI eligibility and definition existence are different responsibilities. `MainGame.OnGameLoaded` assigns the save and restores the scene; `SerializableWGO.ToWGO` restores the craft component by saved ID. Exact live loader timing and repeated-load idempotence still require a representative runtime check.
- `CraftComponent.Interact` calls `GUIElements.OpenCraftGUI`. After special-station checks, the latter scans component definitions and routes Survey/ResourcesBasedCraft/AlchemyDecompose to ResourceBasedCraftGUI, returning immediately. A station retaining Survey records will therefore continue to open its sample picker even if None-type research jobs are appended. A table-local mode/entry is required to open ordinary research craft cards; do not replace all table interaction or reorder definitions to masquerade as integration.
- `DoAction` refuses manual work on hidden or automatic crafts. A hidden definition is consequently not a safe substitute for topic-eligibility filtering. Ordinary job definitions can have no target-product needs/output, an explicit icon/name, manual craft time/energy and no technology-point reward; the nearby Survey definitions remain separate.
- `CraftReally` removes inventory needs and station `needs_from_wgo`, sets current_craft and starts progress. `Cancel` refunds `_cur_craft_items_used` as drops and applies `set_when_cancelled`, but does not generically restore `needs_from_wgo`. Station science therefore has different native cancellation treatment from an ordinary item cost. Research refund semantics are not established merely by reusing Cancel.
- `ProcessFinishedCraft` reaches `GameSave.OnFinishedCraft` for the real job before `End` clears it. This is a narrow completion boundary for a namespaced research-only consumer. Native scripted `SmartExpression.UnlockAlchemy` writes actual formula IDs to `completed_one_time_crafts`; formula knowledge need not use a second save ledger. Calling OnFinishedCraft on fabricated mixed crafts would also invoke quest/achievement and mixed-key consequences and should not be treated as equivalent to authored knowledge disclosure.

These are static findings; the following installed test closes the bounded work path, without closing formula reward or final table UI.

### Installed sample-free work result — 2026-10-12

**Status:** confirmed native work lifecycle in the user's modded Graveyard Keeper 1.407 installation. Consumer: Researchable Alchemy Recipes, research-only Study Table Work Probe 0.1.0, exact source `6f4fd3bd21e6fee2247c0edfdc372bb1e1f988ef`, DLL SHA-256 `319a0a277c418587b71f0195cede9346ffd27d65774cb8d0c3cbdeef0b550612`. Returned log snapshot SHA-256 `c21fcb3a4c23589b85ac25b1ccddff00b9c72f1f7cf0280daf8f93f343f8c732`; detailed procedure and evidence are in that project's docs/TEST_LOG.md on research/study-table-work.

- Pre-ID-cache insertion resolves through the real native cache. Temporary table-local controller selection opens ordinary Survey or native None-type craft cards without removing Survey definitions.
- A real sample-free manual job with no output starts through CraftReally: faith 6 -> 5, station science 21 -> 20. Native work reaches progress 0.268 and spends energy before cancellation; no observer-written progress/completion.
- Native Cancel leaves no active craft and science remains 20. Faith is a world-drop refund: immediate inventory still reads 5, the user observes return, and the next launch reads 6. Inventory-only sampling at the cancel return boundary would incorrectly conclude no refund.
- After another start and a save/menu-load cycle, DeserializeCraftComponent reports saved active, definition resolved and current active. The user observes preserved partial progress and completes manual work. GameSave.OnFinishedCraft writes the native completed marker; cleanup removes it and stored last-job references.
- No probe error occurs. The loaded-state observer attached to MainGame.OnGameLoaded is not reached in this capture; Save Now! 2.5.14 participates in the loading environment. Do not generalize one private MainGame callback as a universal loaded-state boundary from its name alone.

Limits: this test used menu-load within one process, not a reported full process restart; numeric post-load progress is user-observed rather than logged. Controller selection is verified; mouse and pre-alchemy gate are not shown by this run. Installed Alchemy Research Redux 0.1.8/Decomp Delight! 0.1.9 coexistence is representative, not a general compatibility guarantee. Ordinary Survey visually completed but emitted a separate native sample-return warning of unknown cause before the probe job. No final UI, refund economics, formula disclosure/knowledge reload or general uninstall behavior is accepted. The modal entry is diagnostic; the consumer prefers two tabs.

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

## Craft technology-point output ownership

**Status:** verified static fact for Graveyard Keeper 1.407.

For ordinary crafts, technology-point rewards are represented inside the native `CraftDefinition.output` list alongside physical outputs.

`TechDefinition.TECH_POINTS` identifies the technology-point item IDs, including:

- `r` — red;
- `g` — green;
- `b` — blue;
- `v` and `gratitude_points` as other point-like resources handled by the same classification.

`CraftDefinition.GetFirstRealOutput()` explicitly skips entries whose IDs are in `TechDefinition.TECH_POINTS`, which distinguishes the physical craft result from these point outputs.

On the normal completion path, `CraftComponent.ProcessFinishedCraft()` processes `current_craft.output` through the host's existing output/drop machinery. Therefore a recipe's repeatable red/green/blue reward is native recipe data rather than a separate craft-specific technology-point formula.

Study/Survey follows the same data model for technology-point rewards. `ItemDefinition.GetSurveyCraft()` resolves the relevant `surv:...` `CraftDefinition`, and the standard item-tooltip Survey path reads technology-point entries from that Survey craft's `output`.

### Standard unstudied-item Survey hint semantics

Pinned Graveyard Keeper 1.407 static inspection establishes the exact vanilla hint behavior inside `ItemDefinition.GetTooltipData(...)`:

- when a Survey craft exists and is not yet complete, the tooltip iterates the Survey craft's processed `output`;
- it keeps only entries whose IDs are in `TechDefinition.TECH_POINTS`;
- it appends the technology-point **IDs/icons only** (for example `(b)`), not their numeric `Item.value`;
- therefore vanilla intentionally tells the player **which colors** an item can yield from Study while concealing the exact quantity;
- when the Survey is complete, the hint changes to the ordinary `survey_complete` state.

This is a reusable event-driven seam for a qualitative research-value cue: a mod can derive magnitude from the same native Survey `CraftDefinition.output` while preserving vanilla's decision not to expose the exact point count. Such a cue should remain presentation-only and should not duplicate Study eligibility or completion state outside the host data.


**Engineering implication:** when a mod needs to change only the technology-point reward of a verified recipe, modifying the relevant `r/g/b` entries in the host-owned `CraftDefinition.output` is a lower-level native seam to investigate before replacing the craft-completion reward path.

**Evidence provenance:** Graveyard Keeper 1.407 static reference `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, inspected `CraftDefinition`, `CraftComponent`, `TechDefinition`, `ItemDefinition`, and `TechPointsDrop`.

**Applicability limit:** this establishes ownership and consumption for the inspected normal craft/Survey paths. It does not establish the numeric reward values of any specific recipe, nor does it imply that every non-crafting source of technology points uses `CraftDefinition.output`.

## Known consumers

- `NikichMods/Crafting-Planner`
- `NikichMods/SpecializedStorage`
- `NikichMods/BetterSaveSoulRebalance`
- `NikichMods/FoodAndDrinkRebalance`
- `NikichMods/WhoBuysThis`

Project-specific balance, buyer-display policy, specialized-storage classification, and planner product scope remain canonical in their owning repositories.


### Red technology-point channels outside CraftDefinition

Pinned Graveyard Keeper 1.407 static/runtime inspection establishes that red technology points are not confined to `CraftDefinition.output`.

Additional native channels:

- `GameBalance.works_data` stores `WorkDefinition.reward`.
- `WorldGameObject.RewardForWork()` resolves `obj_def.work` through `GameBalance.GetData<WorkDefinition>()` and adds the work reward to the player.
- `ObjectDefinition.drop_items` can contain technology-point items; `WorldGameObject.DropItems()` recognizes r/g/b tech-point IDs and spawns them through the normal tech-point drop path.
- `ObjectDefinition.add_player_param_after_hp_0` is another direct player-resource channel applied during `DoZeroHPActivity()`.

Accepted runtime evidence from the grave-reward project found ordinary trees, stumps, stone nodes and quarry/ore objects with red entries in `drop_items`, confirming that physical gathering is a native repeatable red source independent of recipe crafting.

Research implication: whole-game red-economy analysis must inspect both craft outputs and world-object/work reward channels; a CraftDefinition-only inventory is incomplete.

## Item crafting-location tooltip ownership

**Status:** verified static fact for Graveyard Keeper 1.407.

Pinned source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

`ItemDefinition.GetTooltipData(Item, bool)` constructs the vanilla standard-item crafting-location row from `ItemDefinition.GetItemDetails().crafts_in`. It localizes each `ObjectDefinition.id` with `GJL.L(id)`, joins them using the localized comma token when available, and appends one `BubbleWidgetTextData` row headed by `GJL.L("crafted_at")`.

`GetItemDetails()` gets that station list from `GameBalance.GetItemCraftsIn(item_id)`. The backing cache is built by `GameBalance.CreateCraftsCache()` from native `craft_data -> CraftDefinition.craft_in`, excluding `grave_ground`, hidden recipes, and recipes marked `dont_show_in_hint`; duplicate station definitions are suppressed while first native encounter order is preserved.

`ItemDefinition.GetTooltipDataCraftAt(Item)` duplicates the same craft-location formatter as a dedicated helper.

Known direct consumers in the pinned source:
- `BaseItemCellGUI` calls `GetTooltipData(..., true)` for standard item cells;
- `TechUnlock` calls `GetTooltipData(..., false)` and, in its special multi-quality prayer-output branch, also calls `GetTooltipDataCraftAt(...)`.

`BubbleWidgetText.Draw(BubbleWidgetTextData)` is the verified final text writer on this path: it assigns `UILabel.text = data.text`; enclosing `WidgetsBubbleGUI` then owns normal bubble sizing/repositioning.

**Applicability limit:** this establishes ownership, source, ordering, direct callers and final text consumption. It does **not** establish a generic station-family/tier relationship. `ObjectDefinition` exposes no obvious universal tier/upgrade-parent field in the pinned class definition, so grouping tiered station variants requires separate balance/runtime evidence rather than assuming that every numeric ID suffix is a tier.

## Technology recipe exact-location semantics

**Status:** verified static + full-balance-data result for Graveyard Keeper 1.407.

A Technology craft unlock has two different native location concepts that answer different questions:

- the exact unlocked recipe owns `CraftDefinition.craft_in`;
- the output item's standard tooltip obtains `crafted_at` from `GameBalance.GetItemCraftsIn(output_item_id)`, which aggregates stations across all eligible recipes producing that item.

`TechUnlock.GetTooltip` currently delegates ordinary recipe detail to the output item's `ItemDefinition.GetTooltipData(item, false)`. Therefore its Technology-tooltip location is item-level, not necessarily exact-recipe-level.

A full audit of the user-supplied 1.407 `game_data` compared these two paths for the 229 authored-visible Technology recipe unlocks that resolve to a normal physical output item:

- 91 exact `craft_in` lists equal the item-level aggregate;
- 138 (60.3%) are strict subsets of the item-level aggregate;
- no compared case was disjoint or contradictory.

Example: `Iron -> ingot_metal` is authored for `mf_furnace_0`, while the item-level cache lists `mf_furnace_0`, `mf_furnace_1`, and `mf_furnace_2`.

Engineering implication: when presenting **which station owns this specific recipe**, use that recipe's `craft_in`. Use `GetItemCraftsIn` when the intended question is broader: **where can this output item be crafted in general**. Do not treat the two queries as interchangeable.

Evidence:
- pinned host source `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`;
- `resources.assets` SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
- matching `Assembly-CSharp.dll` SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`;
- detailed Technology audit in `docs/TECH_TREE_INFORMATION_RESEARCH.md`.

## ObjectCraftDefinition builder routing and placement scope

**Status:** verified static + full-balance-data result for the current 1.407 Technology-blueprint population.

`ObjectCraftDefinition.builder_ids` is consumed by `MainGame.OpenBuildObjectGUI(build_desk)` to decide which object crafts are offered through a given build desk, subject to the save's craft-visibility predicate.

`ObjectCraftDefinition.sub_zone_id` has a different role: build-grid logic consumes it as an authored placement/sub-zone restriction. It is not the owner of the build-menu routing identity.

For all 105 authored-visible Technology blueprint unlocks in the audited 1.407 balance data:

- exactly one `builder_id` is present;
- every builder ID resolves to a real `ObjectDefinition`;
- only 13 have a non-empty `sub_zone_id`.

The host already treats `ObjectDefinition.id` as a localization identity: standard item crafting-location formatting calls `GJL.L(ObjectDefinition.id)`, and `WorldGameObject.GetUniversalObjectInfo()` uses `GJL.L(this.obj_def.id)` for the object header.

Engineering implication: for a Technology blueprint's **owning build desk/menu**, derive the identity from `builder_ids` and localize that ObjectDefinition ID through the host. Do not substitute `sub_zone_id`, infer an area name from ID spelling, or maintain a parallel hard-coded builder-name map unless a future verified requirement demands it.

Applicability limit: the one-builder cardinality is an accepted fact for the current authored-visible 1.407 Technology blueprint population, not a universal schema invariant for every ObjectCraftDefinition in the game or future versions.

## Tavern and refugee cooking-station routing

**Status:** verified static host fact for Graveyard Keeper 1.407.

Pinned source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

The cooking-station IDs below are not one universal tier family:

- `tavern_kitchen` and `tavern_oven` have dedicated host branches in `CraftComponent` / `WorldGameObject` that route produced food through the tavern/Barman path.
- `refugee_camp_cooking_table` and `refugee_camp_cooking_table_2` have their own refugee-camp-specific handling in `CraftComponent`.
- ordinary `cooking_table`, `cooking_table_2`, and `oven` are distinct canonical IDs from both DLC-specific families.

Engineering implication: tooltip or recipe-presentation code must not infer that `tavern_kitchen` is a later tier of `cooking_table*`, or that `tavern_oven` is a later tier of `oven`, merely because they share recipe outputs or related display wording.

## Zombie mine production object identity

**Status:** verified static host fact for Graveyard Keeper 1.407.

Pinned source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

The zombie mining area uses several placed-object IDs that are interaction/production points rather than tiered workstation levels:

- `mine_zombie_bench` restarts `mine_zombie_bench_iron_production`.
- `zombie_mine_fence_left_front` restarts `zombie_mine_stone_production`.
- `zombie_mine_fence_front` is position-dependent:
  - the instance around `(-3932, 6622)` restarts `zombie_mine_stone_production`;
  - instances around `(-3732, 6616)` and `(-3382, 6622)` restart `zombie_mine_marble_production`.

`SmartExpression` contains the same distinction when a worker is attached.

Engineering implication: code that presents crafting locations must not interpret `left_front` / `front` zombie-mine IDs as level I/II. Multiple world instances can share the same canonical object ID, so `GameBalance.GetItemCraftsIn` may already collapse several physical interaction points into one object-definition entry. An exact duplicate player-facing label can therefore be a placed-object artifact rather than a tier family.

