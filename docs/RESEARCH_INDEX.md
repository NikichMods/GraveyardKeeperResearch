# Graveyard Keeper 1.407 Research Index

This is the canonical entry point for reusable cross-project research.

Before starting a new host-internals probe in a Graveyard Keeper mod:

1. read the owning project's canonical verified-data / architecture docs;
2. search this index and the linked shared research docs;
3. search accepted test logs and relevant repository history if the fact has not yet been promoted;
4. perform fresh static/runtime research only when the question remains open.

## Shared game/runtime internals

### UI / NGUI / input / timing / environment

- `docs/GAME_INTERNALS.md` — BepInEx plugin-metadata constraints, detailed Technology-tooltip width lifecycle, and accepted standard item-tooltip child-alignment/native-span behavior.
- `docs/UI_INPUT_TIME_AND_ENVIRONMENT.md` — Technology-tree gamepad focus/unlock-tooltip ownership, gamepad bubble placement lifecycle, NGUI screen-size ownership, Pray GUI craft-button anchor ownership, WaitingGUI timing/fixed-step ownership, SliderDec/SliderInc input/hold-repeat, WaitingGUI button tips, localization reload, the verified 1.407 `GJL.L(string)` localization ABI / firstpass identity, current-language UILabel font ownership, weather-audio ownership, environment-preset refresh, and final ambient-light observation.
- `docs/TECH_TREE_INFORMATION_RESEARCH.md` — community/product evidence for richer Technology-tree information, full 1.407 balance-data audit, verified unlock/tooltip ownership, blueprint/recipe coverage and special cases, exact location semantics, stress cases, and the closed research gate for production bootstrap.

Key established limits:
- BepInEx 5 `[BepInPlugin]` version metadata must be numeric/System.Version-parseable; keep research/RC labels outside that metadata.
- Technology-tooltip `UILabel.overflowWidth` evidence applies to the inspected Technology path, not every tooltip.
- Standard item-tooltip alignment evidence distinguishes child-label text alignment from centered child-widget placement; it applies to the inspected `WidgetsBubbleGUI` item-tooltip family, not arbitrary NGUI tables.
- Technology-tree gamepad navigation focuses the parent tech node and combines visible child `TechUnlock` tooltips on the verified 1.407 path; mouse child tooltips are independent.
- Technology-information research supports enriching the existing `TechUnlock.GetTooltip` path as the least-complex current product direction; each visible blueprint record has a resolvable native builder, but five 1.407 visible blueprint unlocks also have same-Technology `@`-hidden sibling records that unlock additional builders. `sub_zone_id` remains a separate placement restriction and must not be repurposed as a guessed area label; global same-`out_obj` aliases can belong to separately gated progression.
- `WidgetsBubbleGUI.Update()` is a late/native placement lifecycle for the inspected gamepad bubble family; do not replace it with an earlier event merely for elegance without proving final geometry/overwrite order.
- WaitingGUI time/fixed-step results are accepted for the tested meditation range, not arbitrary global speed mods.

### Dialogue / quests / FlowCanvas

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

- `docs/CRAFTING_INVENTORY_AND_TRADING.md` — CraftDefinition/build ownership, native craft/Survey technology-point output ownership, player-vs-interaction inventory, storage-local inventory, native move/capacity path, stack equivalence, craft renderer list invariants, `CraftComponent.DoAction` actor/timing semantics, PlayerBuff duration/removal primitives, vendor sale rules, dynamic product types, lazy Vendor construction, KnownNPC/staged-vendor lifecycle, the standard item-tooltip seam, and vanilla crafting-location tooltip ownership/source/order.
- `docs/FARMING_AND_FERTILIZER.md` — exact 1.407 manual fertilizer state/writer/reset semantics, Boost duration formula, Quality yield and next-tier seed/crop behavior, Farmer interaction boundary, and the distinct permanent Quality-fertilizer upgrades used by zombie farms/vineyards and refugee garden beds.
- `docs/ALCHEMY_SYSTEM.md` — mixed-alchemy slot categories and exact-mixture lookup, stochastic failure/goo selection, recipe-discovery persistence/scripted unlock channels, Study/decomposition metadata ownership, and accepted 1.407 runtime corpus structure/counts.
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
