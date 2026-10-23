# Graveyard Keeper 1.407 Research Index

This is the canonical entry point for reusable cross-project research.

Local installation/log collection across Graveyard Keeper mods follows [DevRules/CODEX_WORKFLOW.md — Local installed-runtime loop](https://github.com/NikichMods/DevRules/blob/main/CODEX_WORKFLOW.md#local-installed-runtime-loop): execute authorized DLL replacement and accessible log capture directly, preserving exact identity, previous payload and unrelated mods. This is an operating-workflow pointer; runtime findings and project acceptance remain in their owning evidence documents.

Before starting a new host-internals probe in a Graveyard Keeper mod:

1. read the owning project's canonical verified-data / architecture docs;
2. search this index and the linked shared research docs;
3. search accepted test logs and relevant repository history if the fact has not yet been promoted;
4. perform fresh static/runtime research only when the question remains open.

Active-station cancellation access: CRAFTING_INVENTORY_AND_TRADING.md records native CanInteractCraft/GUIElements.OpenCraftGUI busy non-zombie entry blockers and why existing cancel-dialog/queue callbacks do not establish reachable UI. Static host evidence; consumer-specific cancellation necessity/control remains open in ResearchableAlchemyRecipes.

## Shared game/runtime internals

- [Graveyard objects and rating](GRAVEYARD_RUNTIME.md): GK 1.407 prayer-station definitions, native body/decoration rating boundary and bounded sarcophagus/candle asset evidence. Static evidence only; placement and new mechanics unverified. [Player-needs research](GRAVEYARD_PLAYER_NEEDS.md) separates testimony about body-work friction and aesthetic freedom from unresolved buried-organ-inspection demand. [Late-game design candidates](ECONOMY_DESIGN_CANDIDATES.md) preserve historical exploration and unadopted side ideas. The adopted prayer-station product design now belongs to [Better Prayer Stations](https://github.com/NikichMods/BetterPrayerStations/blob/main/docs/DESIGN.md); reusable host facts remain here.
- [Rating inputs and diagram/zone consumers](GRAVEYARD_RUNTIME.md#rating-inputs-native-diagram-and-zone-consumers--2026-10-23): native active-craft exclusions, generic-modifier branch limits, separate raw-body/central-grave UI readers, decay thresholds and live zone-quality summation. Native skull oracle passes160 offline cases; subsequent one-grave input projection passes installed rating/diagram, excess-capacity, explicit activation and cleanup with unchanged measured raw/physical data. [Headless native getter and grave-carrier checks](GRAVEYARD_RUNTIME.md#headless-native-rating-and-grave-carrier-lifecycle-checks--2026-10-23) add2700 native calculation cases and binary serialization/synthetic body-relocation evidence. [Bounded live empty/replacement recovery](GRAVEYARD_RUNTIME.md#bounded-installed-emptyreplacement-grave-recovery) now confirms retained grave ID/investment across native exhumation and replacement menu-loads plus real low-durability calculation. Destination non-export was not instrumented; payment and real coverage remain unverified.
- [Grave/item persistence](GRAVEYARD_RUNTIME.md#grave-and-decoration-persistence--2026-10-23): native map/WGO saved Item and ID, same-object grave state transitions, all 42 checked gravestone/fence definitions non-stackable, copy-preserving inventory paths, template-generated removal outputs losing arbitrary metadata, and reduced active-craft current-item serialization. Offline native map/item serialization fixture passes; the subsequent bounded gravestone menu-load/render observation is linked below. Broader live craft/movement and independent-slot lifecycles remain unverified.
- [Grave visual descriptors](GRAVEYARD_RUNTIME.md#native-grave-visual-descriptor-pipeline--2026-10-23): CustomDrawers inventory-to-WOP-name builder, construction/durability/vegetation descriptors, cache comparison, stage fallback and native part-instantiation owner; bounded stage/component corpus and GraveGUI close seam. The [bounded installed observations](GRAVEYARD_RUNTIME.md#bounded-installed-grave-descriptor-observation--2026-10-23) confirm one-grave close-cancel plus two-grave independent top/fence choices through native removal/reinstallation, empty decoration slots, distinguishable gravestone exchange, menu-load and cleanup. Identical physical fence provenance, movement/exhumation and broader acceptance remain open; native-log watch items are retained without inferred attribution.

- [Economy progression and resource saturation](ECONOMY_PROGRESSION_RESEARCH.md): GK 1.407 money, faith, station science and separate red/green/blue economies; conditional deficit-to-surplus transitions, DLC/automation differences, major costs and remaining-goal demand. [Scenario calculations](ECONOMY_SCENARIOS.md) distinguish explicit assumptions from measured playthroughs; [sources and contradictions](ECONOMY_SOURCES.md) record exact input identities, bounded native checks, player testimony and limits. Includes a small reproducible static extractor/derived evidence; no new game mechanics or population timing claims.

- [Current-save player ownership after scene restoration](GAME_INTERNALS.md#current-save-player-ownership-after-scene-restoration): verified GK 1.407 startup inventory writer, stale prior-scene window on same/cross-save loads, and accepted read-only readiness boundary; consumer-specific checks remain required.

Native window/overlay coordination: UI_INPUT_TIME_AND_ENVIRONMENT.md records BaseGUI stack/event ordering, separate tooltip/speech/context/corner overlays, native player-control guard and three-row/no-scroll information-window limits. Static owners for later safe-notification integration; no installed popup-coordination acceptance is implied.

### UI / NGUI / input / timing / environment

- [Requested height versus actual hidden-widget geometry](UI_INPUT_TIME_AND_ENVIRONMENT.md#ngui-requested-height-and-hidden-widget-observation--2026-10-23): native height setters respect virtual sprite minimums/anchors; grave red bar is hidden atzero. Exact requested height is not a valid universal assertion for an invisible widget. The particular Faith Probe0.1.0 failed bar conjunct was not separately logged and remains unobserved.

- Native picker warning attribution in UI_INPUT_TIME_AND_ENVIRONMENT.md: InventoryPanelGUI.DoOpening creates bare separator widgets without assigning their layer; bounded .56 runtime identifies five layer0 separators against panel13, corrected by native CheckLayer. Initial image/text asset scans miss these generated widgets. No general layer patch or clean-game compatibility claim follows. Consumer-specific cleanup/acceptance stays in ResearchableAlchemyRecipes.


- `docs/GAME_INTERNALS.md` — BepInEx plugin-metadata constraints, detailed Technology-tooltip width lifecycle, and accepted standard item-tooltip child-alignment/native-span behavior.
- `docs/UI_INPUT_TIME_AND_ENVIRONMENT.md`, native craft tabs section — static CraftGUI/CraftTabGUI tab ownership, separate top-of-stack MixedCraftGUI composition, Survey's distinct picker owner and semantic-key/close lifecycle; bounded study-table two-tab usability is observed in 0.1.1, but live key-map mutation on close produces enumeration errors. Native icon IDs and the field-replacement correction are documented and observed in the returned 0.1.2 run; the later .56 observation attributes the specific picker warnings to native generated separators (see the inventory separator section).
- `docs/UI_INPUT_TIME_AND_ENVIRONMENT.md` — Technology-tree gamepad focus/unlock-tooltip ownership, gamepad bubble placement lifecycle, NGUI screen-size ownership, Pray GUI craft-button anchor ownership, WaitingGUI timing/fixed-step ownership, SliderDec/SliderInc input/hold-repeat, WaitingGUI button tips, localization reload, the verified 1.407 `GJL.L(string)` localization ABI / firstpass identity, current-language UILabel font ownership, weather-audio ownership, environment-preset refresh, and final ambient-light observation.
- `docs/TECH_TREE_INFORMATION_RESEARCH.md` — community/product evidence for richer Technology-tree information, full 1.407 balance-data audit, verified unlock/tooltip ownership, blueprint/recipe coverage and special cases, exact location semantics, stress cases, and the closed research gate for production bootstrap.

Key established limits:
- Native font-effects section in UI_INPUT_TIME_AND_ENVIRONMENT.md: all 11 built-in language routes use atlas-backed UIFonts and do not take the packed-shader outline exclusion; separate native tooltip text widgets scope effects, and rich-text alpha tags can subdue source rows. Static eligibility is not multilingual visual acceptance.
- BepInEx 5 `[BepInPlugin]` version metadata must be numeric/System.Version-parseable; keep research/RC labels outside that metadata.
- Technology-tooltip `UILabel.overflowWidth` evidence applies to the inspected Technology path, not every tooltip.
- Standard item-tooltip alignment evidence distinguishes child-label text alignment from centered child-widget placement; it applies to the inspected `WidgetsBubbleGUI` item-tooltip family, not arbitrary NGUI tables.
- Technology-tree gamepad navigation focuses the parent tech node and combines visible child `TechUnlock` tooltips on the verified 1.407 path; mouse child tooltips are independent.
- Technology-information research supports enriching the existing `TechUnlock.GetTooltip` path as the least-complex current product direction; each visible blueprint record has a resolvable native builder, but five 1.407 visible blueprint unlocks also have same-Technology `@`-hidden sibling records that unlock additional builders. `sub_zone_id` remains a separate placement restriction and must not be repurposed as a guessed area label; global same-`out_obj` aliases can belong to separately gated progression.
- `WidgetsBubbleGUI.Update()` is a late/native placement lifecycle for the inspected gamepad bubble family; do not replace it with an earlier event merely for elegance without proving final geometry/overwrite order.
- WaitingGUI time/fixed-step results are accepted for the tested meditation range, not arbitrary global speed mods.

### Dialogue / quests / FlowCanvas

- Exact alchemy-request task owners in DIALOGUE_QUEST_AND_FLOWCANVAS.md: initial Clotho healing and Astrologer acid assignment/completion, pure persisted task readers versus mutating GetOrCreateNPC, and a bounded 764-graph Item/Flow_Answer census. Task mention policy and pending installed evidence belong to ResearchableAlchemyRecipes.

- `docs/DIALOGUE_QUEST_AND_FLOWCANVAS.md` — weekday HUD semantic ownership, native quest-marker resources, task mutation, phrase state, SmartRes answer gates, `MultipleAnswerData` AND semantics, persistent blacklist ownership, root-to-answer reachability, and zone-quality mirrors.

Key established result:
- current actionability is not equivalent to a visible journal task or a final answer's own gate; authored parent-route state and resource gates can matter.

### Church / confessionals

- `docs/CONFESSION_NOTIFICATION_RESEARCH.md` — confession-availability mechanics established by PrayerClarity, player UX evidence, notification solution-space history, and accepted state-owner/lifecycle/audio/presentation closure used by Keeper's Alerts.

Key established direction:
- observe the native resulting confession-availability state; do not duplicate the daily RNG/probability mechanics;
- reuse the existing prayer/confessional visual semantics where practical;
- the owner/lifecycle/audio/presentation questions are now closed for the current Keeper's Alerts production consumer; the owning repository remains authoritative for product-specific implementation and release state.

### Crafting / inventory / trading / buffs

- Repeatable builder placement in CRAFTING_INVENTORY_AND_TRADING.md: exact DoPlace material/preview/just_built ordering, capture-before-next-preview identity, and bounded passive observation of two commits, busy rejection, cancellations and upstream material refusal. Common craft completion and upgrade/repair/clearing remain separate; planner decrement acceptance belongs to Building Planner.

- `docs/STUDY_RESOURCE_ECONOMY.md` — bounded static 1.407 paper/science yields, technology unlock owners and material-study/payment limits. Science is a station resource; authored paper yields do not establish player income rates or final mod prices.

- Ordinary source/card observation in CRAFTING_INVENTORY_AND_TRADING.md: installed manual 0.1.2 confirms current-save church-workbench visibility against exact native/grouped-card ID sets for sixteen source groups. Preserve _possible_crafts membership when variants share one card; affordability and temporary busy-state policy are distinct from visibility. Bounds exclude universal physical/early/busy access claims.

- `docs/CRAFTING_INVENTORY_AND_TRADING.md` — CraftDefinition/build ownership, native craft/Survey technology-point output ownership, player-vs-interaction inventory, storage-local inventory, native move/capacity path, stack equivalence, craft renderer list invariants, `CraftComponent.DoAction` actor/timing semantics, PlayerBuff duration/removal primitives, vendor sale rules, dynamic product types, lazy Vendor construction, KnownNPC/staged-vendor lifecycle, the standard item-tooltip seam, and vanilla crafting-location tooltip ownership/source/order.
- `docs/FARMING_AND_FERTILIZER.md` — exact 1.407 manual fertilizer state/writer/reset semantics, Boost duration formula, Quality yield and next-tier seed/crop behavior, Farmer interaction boundary, and the distinct permanent Quality-fertilizer upgrades used by zombie farms/vineyards and refugee garden beds.
- `docs/ALCHEMY_SYSTEM.md` — mixed-alchemy slot categories and exact-mixture lookup, stochastic failure/goo selection, recipe-discovery persistence/scripted unlock channels, Study/decomposition metadata ownership, accepted 1.407 runtime corpus structure/counts, and loaded presentation/provenance facts for ordinary alchemy ingredients. Static 1.407 verification also establishes zero overlap between successful mixed-product outputs and mixed ingredients, including cross-tier checks. Ingredient producers beyond decomposition include native inventory use, body extraction and hidden automatic cremation; their availability/yield owners and static limits are documented separately from live UI acceptance.
- Sample-free study integration: `docs/CRAFTING_INVENTORY_AND_TRADING.md`, Study-table sample boundary section, separates Survey from common work, pre-cache definition registration, save-ID restoration, native UI dispatch and cancellation resource behavior. The installed 0.1.0 probe confirms start/cancel/menu-load resumption/completion/cleanup, with world-drop faith refund and retained science; final tabs and formula reward remain open. `docs/ALCHEMY_SYSTEM.md`, residual paint awareness section, identifies the normal tech_alchemy_begins graph, persistent technology/craft states and old-save migration writers; current builder/dialogue availability remains open.
- `docs/PERK_MECHANICS.md` — exact 1.407 perk quality-score ownership, linked craft consumers, surgery mistake probabilities, combat-stat effects, Persistence energy regeneration, Miner drop changes, Wine Master secondary energy effects, and Cultist skull-display semantics.
- `docs/INVENTORY_SLOT_PINNING_RESEARCH.md` — initial community/product research for stable/pinned player-inventory item positions, existing mod landscape, preliminary BepInEx feasibility evidence, solution-space comparison, and the remaining BLOCKED slot-order/final-writer questions.

Key established limits:
- `GetMultiInventoryForInteraction()` is not player-only inventory.
- read-only trade queries should not force lazy Vendor construction.
- renderer-only craft augmentation must not mutate shared recipe definitions.
- vanilla item-tooltip crafting locations come from ordered native `GameBalance.GetItemCraftsIn(...)` data, but no generic station-family/tier relationship is yet established from static `ObjectDefinition` metadata.
- a Technology recipe's exact `CraftDefinition.craft_in` and an output item's aggregate `GetItemCraftsIn(...)` answer different questions; in the audited visible 1.407 recipe population the exact station set is a strict subset in 138/229 comparable cases.
- manual fertilizer uses independent `grow_qual` / `grow_time` plot parameters and resets after the crop cycle; automatic zombie/refugee farm Quality-fertilizer upgrades are persistent `lvl` changes and must not be conflated with manual fertilizer application.
- mixed-alchemy failure goo is a stochastic clue derived from a randomly chosen same-arity valid recipe sharing at least one correctly positioned attempted ingredient; this improves forward structural discovery but does not by itself target a requested unknown output.
- accepted loaded-balance evidence finds 43 ordinary picker-compatible formulas for 34 outputs, plus one success-classified three-slot definition outside the standard picker contract; keep that exception separate until its role is established.

### Fishing

- `docs/FISHING_RUNTIME.md` — native bite-wait ownership, missed-hook re-entry, bobber transform/sprite geometry, and the accepted read-only overlay integration pattern.

### One-time consolidation audit

- `docs/CONSOLIDATION_AUDIT_2026-09-24.md` — records the repositories reviewed, what was promoted, what intentionally stayed project-local, and which provisional/rejected findings were not promoted.

## Performance diagnostics

- `docs/PERFORMANCE_EVIDENCE.md` — accepted performance findings, active hypotheses, ruled-out causes, and root causes.
- `docs/TEST_LOG.md` — controlled A/B and runtime diagnostic history.
- `docs/GC_COUNTER_NOTE.md` — GC counter interpretation note.
- `docs/MOD_ISOLATION_2026-09-13.md` — mod-isolation evidence.
- `docs/MERCHANTS_PROMISE_OWNER_2026-09-13.md` — owner attribution research for The Merchant's Promise.

## Promotion rule

Do not leave a reusable accepted result discoverable only through a chat, old commit, candidate note, raw log, or frozen research branch. Distill it into the appropriate canonical shared document and update this index.

Project-specific mechanics, balance, UX decisions, release state, and accepted build identity remain canonical in the owning mod repository.

## Research-material policy

The canonical knowledge base is public, but research inputs do not have to be public or committed. Local/temporary inspection of game binaries, decompiled code, resources, runtime state, dumps, and extracted metadata is allowed as research input when the working environment permits it. Promote only the durable derived facts/evidence needed for reuse.

Do not create a private scratch repository by default. Use one only when a concrete operational need appears; accepted reusable conclusions still return to this index and the linked canonical documents.

- Native formula final consumer: ALCHEMY_SYSTEM.md, Native known-formula list section, records exact completed_one_time_crafts -> station-filtered CraftGUI.OpenAsAlchemy, specific authored disclosure vs manufactured completion, grouped pseudotech dialog capacity and native save preparation/serialization boundaries. Installed reward/full-restart evidence belongs to the consuming project.

- Native outbound transfer invalidation: CRAFTING_INVENTORY_AND_TRADING.md, Successful native inventory transfer section, explains why MultiInventory.MoveItemTo bypasses Item.RemoveItem observers and records the bounded Quick Stack refresh acceptance from Building Planner0.1.14. Observe the successful semantic transfer, preserve native capacity/ownership and avoid polling.
- Native action-tip palette/artwork: UI_INPUT_TIME_AND_ENVIRONMENT.md, Native action-tip gradient section, records public UILabel gradient properties, inspected cream-to-brown tip styling and native round direction sprites. Native copy/layout acceptance remains consumer-specific.

- World repair/clearing completion in CRAFTING_INVENTORY_AND_TRADING.md: bounded passive observation separates successful start/spend from later ProcessFinishedCraft/OnFinishedCraft/End; original-owner capture survives replacement, and clearing target0 remains Unity-alive at return despite native DestroyMe. Builder upgrade and negative paths remain separate; consumer planner removal is not established.

- One-time yard builder upgrade in CRAFTING_INVENTORY_AND_TRADING.md: authored auto/zero-time None craft completes through a nested ProcessFinishedCraft/OnFinishedCraft/End before CraftReally returns, without replacement; the builder/component owner matches. Explicit synthetic resources prepared a real native action, not a fabricated completion. Script termination is later, and other scripted upgrades remain outside the bounded observation.

- Authored one-time builder audit in CRAFTING_INVENTORY_AND_TRADING.md:57 definitions split into39 instant scripted None upgrades,17 Put placements and1 empty-needs action. A separate45 non-one-time None population includes callback-driven home furniture; do not equate native craft start with its confirmed placement. Static corpus/route coverage is distinct from individual runtime acceptance.

- [Native positions and WGO registration](GRAVEYARD_RUNTIME.md#native-positions-and-wgo-registration-for-coverage-consumers--2026-10-23): native pos/grid96 scale, position-cache writer, static registration deduplication and removed-state boundary; exact Move Objects0.6.0 selected coordinate-commit path. Static input evidence plus [bounded coverage return and world-badge distinction](GRAVEYARD_RUNTIME.md#coverage-consumer-runtime-return-and-separate-world-badge-consumer): numeric coverage and loaded wooden station pass, but native registered previews and stale world badges remain separate integration questions.
