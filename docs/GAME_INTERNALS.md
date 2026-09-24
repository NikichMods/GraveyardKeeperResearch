# Graveyard Keeper 1.407 — Shared Game Internals

This document stores distilled host/runtime facts that are reusable across multiple mods.

Each entry should state its evidence status, owner/path, verified behavior, supporting evidence, and applicability limits. Project-specific product/mechanics decisions belong in the owning project rather than here.

Broader accepted internals are split by domain for maintainability: `DIALOGUE_QUEST_AND_FLOWCANVAS.md`, `CRAFTING_INVENTORY_AND_TRADING.md`, `UI_INPUT_TIME_AND_ENVIRONMENT.md`, and `FISHING_RUNTIME.md`. Start from `RESEARCH_INDEX.md` rather than assuming this single file is exhaustive.

## UI / NGUI

### Technology tooltip width lifecycle

**Target:** Graveyard Keeper 1.407  
**Status:** **accepted fact** for the inspected Technology tooltip path.

#### Verified behavior

PrayerClarity width research established the following live lifecycle for the Technology tooltip mechanics text:

1. PrayerClarity-owned `BubbleWidgetTextData` reached the game with a large finite `max_width` marker/ceiling, but changing that value alone did not make the live label wide enough.
2. Read-only runtime probe 0.1.2 observed the corresponding live `UILabel` with:
   - `overflowMethod = ResizeFreely`;
   - `width = 148`;
   - `lineWidth = 148`;
   - `overflowWidth = 150`;
   - backing `mOverflowWidth = 150`;
   - `processedText` already wrapped at that approximately 150-unit ceiling even though the raw text still contained the intended unbroken tier rows.
3. The effective native width control is therefore `UILabel.overflowWidth`, not the earlier `BubbleWidgetTextData.max_width` value by itself and not a manually forced `UILabel.width`.
4. After vanilla `BubbleWidgetText.Draw(BubbleWidgetTextData)` has assigned the live label text/style, setting a finite `UILabel.overflowWidth` and forcing `processedText` reprocessing allows native NGUI `ResizeFreely` behavior to determine the natural label geometry.
5. The enclosing bubble then uses its normal child-size path (`BubbleWidgetText.GetSize()` / `WidgetsBubbleGUI.UpdateSize()`) to size the outer tooltip/parchment from that geometry.
6. User runtime verification of PrayerClarity 1.0.13 confirmed that this seam made Technology prayer tooltips expand with content and removed the old narrow-prefab wrapping failure.

#### Engineering implications

- Treat `overflowWidth` as a **finite maximum expansion/wrap ceiling**, not as a minimum or forced final width.
- Let real content drive the natural width under `ResizeFreely`; a larger ceiling does not itself force the label to occupy that width.
- Do not manually assign final label width/height merely to widen this verified Technology path unless new evidence proves the native seam insufficient.
- Keep viewport/clamp behavior as a separate downstream concern.
- When changing a shared Technology-tooltip width helper, enumerate all tooltip surfaces/rows that consume it before broadening the policy.

#### Evidence provenance

- PrayerClarity commit `82101688f3a723a70b90e5c5a9189470299e77b8` — recorded the 1.0.10 runtime failure and established that changing the earlier width cap alone was insufficient.
- PrayerClarity candidate note commit `cbda71157b00271417b5c246436326e9312da37c` — recorded runtime probe 0.1.2 geometry and defined the 1.0.13 `overflowWidth` fix.
- PrayerClarity implementation commit `9e8e669e4c592dce5361960d708e6492b4dd496b` — changed the owned Technology mechanics label through native `UILabel.overflowWidth` and `processedText`.
- PrayerClarity 1.0.14 candidate commit `0c18bc3c7af86ec2dea6a168d7c389da05115201` — recorded that the user had runtime-verified 1.0.13 widening successfully.

#### Applicability limits

This evidence proves the inspected **Technology tooltip** path in Graveyard Keeper 1.407. It does not by itself prove that inventory/item bubbles, pulpit UI, dialogue bubbles, or every other NGUI tooltip use the same width owner/lifecycle. Re-verify before generalizing to a different UI family.

#### Known consumers

- `NikichMods/PrayerClarity` — Technology prayer tooltip width/layout.
- Potentially useful to other Graveyard Keeper UI mods only after confirming they are on the same `WidgetsBubbleGUI` / `BubbleWidgetText` lifecycle.
