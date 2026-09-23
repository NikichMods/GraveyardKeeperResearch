# Graveyard Keeper 1.407 Research Index

This is the canonical entry point for reusable cross-project research.

Before starting a new host-internals probe in a Graveyard Keeper mod:

1. read the owning project's canonical verified-data / architecture docs;
2. search this index and the linked shared research docs;
3. search accepted test logs and relevant repository history if the fact has not yet been promoted;
4. perform fresh static/runtime research only when the question remains open.

## Shared game/runtime internals

### UI / NGUI

- **Technology tooltip content width lifecycle** — accepted fact for the inspected Graveyard Keeper 1.407 Technology tooltip path. See `docs/GAME_INTERNALS.md#technology-tooltip-width-lifecycle`.
  - Key result: the live text label uses NGUI `ResizeFreely`; `UILabel.overflowWidth` is the effective finite wrap/expansion ceiling, and the enclosing `WidgetsBubbleGUI.UpdateSize()` derives final bubble/parchment size from child widget geometry.
  - Important limit: this evidence applies to the inspected Technology tooltip path; do not automatically generalize it to unrelated bubble types or UI surfaces.

## Performance diagnostics

- `docs/PERFORMANCE_EVIDENCE.md` — accepted performance findings, active hypotheses, ruled-out causes, and root causes.
- `docs/TEST_LOG.md` — controlled A/B and runtime diagnostic history.
- `docs/GC_COUNTER_NOTE.md` — GC counter interpretation note.
- `docs/MOD_ISOLATION_2026-09-13.md` — mod-isolation evidence.
- `docs/MERCHANTS_PROMISE_OWNER_2026-09-13.md` — owner attribution research for The Merchant's Promise.

## Promotion rule

Do not leave a reusable accepted result discoverable only through a chat, old commit, candidate note, or raw log. Distill it into the appropriate canonical shared document and update this index.

Project-specific mechanics, balance, UX decisions, release state, and accepted build identity remain canonical in the owning mod repository.
