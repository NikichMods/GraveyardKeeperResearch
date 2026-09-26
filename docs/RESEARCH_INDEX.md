# Graveyard Keeper 1.407 Research Index

This is the canonical entry point for reusable cross-project research.

Before starting a new host-internals probe in a Graveyard Keeper mod:

1. read the owning project's canonical verified-data / architecture docs;
2. search this index and the linked shared research docs;
3. search accepted test logs and relevant repository history if the fact has not yet been promoted;
4. perform fresh static/runtime research only when the question remains open.

## Shared game/runtime internals

### UI / NGUI / input / timing / environment

- `docs/GAME_INTERNALS.md` — detailed Technology-tooltip width lifecycle.
- `docs/UI_INPUT_TIME_AND_ENVIRONMENT.md` — Technology-tree gamepad focus/unlock-tooltip ownership, gamepad bubble placement lifecycle, NGUI screen-size ownership, Pray GUI craft-button anchor ownership, WaitingGUI timing/fixed-step ownership, SliderDec/SliderInc input/hold-repeat, WaitingGUI button tips, localization reload and current-language UILabel font ownership, weather-audio ownership, environment-preset refresh, and final ambient-light observation.

Key established limits:
- Technology-tooltip `UILabel.overflowWidth` evidence applies to the inspected Technology path, not every tooltip.
- Technology-tree gamepad navigation focuses the parent tech node and combines visible child `TechUnlock` tooltips on the verified 1.407 path; mouse child tooltips are independent.
- `WidgetsBubbleGUI.Update()` is a late/native placement lifecycle for the inspected gamepad bubble family; do not replace it with an earlier event merely for elegance without proving final geometry/overwrite order.
- WaitingGUI time/fixed-step results are accepted for the tested meditation range, not arbitrary global speed mods.

### Dialogue / quests / FlowCanvas

- `docs/DIALOGUE_QUEST_AND_FLOWCANVAS.md` — weekday HUD semantic ownership, native quest-marker resources, task mutation, phrase state, SmartRes answer gates, `MultipleAnswerData` AND semantics, persistent blacklist ownership, root-to-answer reachability, and zone-quality mirrors.

Key established result:
- current actionability is not equivalent to a visible journal task or a final answer's own gate; authored parent-route state and resource gates can matter.

### Crafting / inventory / trading / buffs

- `docs/CRAFTING_INVENTORY_AND_TRADING.md` — CraftDefinition/build ownership, player-vs-interaction inventory, storage-local inventory, native move/capacity path, stack equivalence, craft renderer list invariants, `CraftComponent.DoAction` actor/timing semantics, PlayerBuff duration/removal primitives, vendor sale rules, dynamic product types, lazy Vendor construction, KnownNPC/staged-vendor lifecycle, and the standard item-tooltip seam.

Key established limits:
- `GetMultiInventoryForInteraction()` is not player-only inventory.
- read-only trade queries should not force lazy Vendor construction.
- renderer-only craft augmentation must not mutate shared recipe definitions.

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
