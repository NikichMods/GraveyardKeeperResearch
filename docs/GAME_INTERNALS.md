# Graveyard Keeper 1.407 — Shared Game Internals

This document stores distilled host/runtime facts that are reusable across multiple mods.

Each entry should state its evidence status, owner/path, verified behavior, supporting evidence, and applicability limits. Project-specific product/mechanics decisions belong in the owning project rather than here.

Broader accepted internals are split by domain for maintainability: `DIALOGUE_QUEST_AND_FLOWCANVAS.md`, `CRAFTING_INVENTORY_AND_TRADING.md`, `UI_INPUT_TIME_AND_ENVIRONMENT.md`, and `FISHING_RUNTIME.md`. Start from `RESEARCH_INDEX.md` rather than assuming this single file is exhaustive.

## BepInEx plugin metadata

### BepInPlugin version strings must be numeric/System.Version-parseable

**Target:** BepInEx 5.x used by Graveyard Keeper 1.407  
**Status:** **verified framework/runtime fact**.

A plugin whose `[BepInPlugin]` version contains a prerelease suffix such as `0.0.0-research` is rejected by the BepInEx chainloader before the plugin's `Awake()` executes. The observed runtime diagnostic is:

`Skipping type [...] because its version is invalid.`

BepInEx's `BepInPlugin` constructor parses the supplied string through `TryParseLongVersion`, which uses `System.Version.TryParse` / `System.Version` semantics; unparseable metadata yields a null `Version`, and `BaseChainloader` skips the plugin.

Engineering implication: keep `[BepInPlugin]` metadata numeric (for example `0.0.0` or `1.2.3`). Track research/candidate labels such as `research`, `rc`, or handoff IDs separately in logs, filenames, docs, or other display metadata rather than appending them to the BepInEx plugin version string.

Evidence:
- CompactCraftingTooltips taxonomy probe r1, source `a56bec010603b5224ad42cb36a244a90abfc00a9`, was compiled successfully but rejected at runtime on BepInEx 5.4.23.5 because its metadata version was `0.0.0-research`.
- Upstream BepInEx source: `BepInEx.Core/Contract/Attributes.cs` (`BepInPlugin.TryParseLongVersion`) and `BepInEx.Core/Bootstrap/BaseChainloader.cs` (skip when `metadata.Version == null`).

Applicability limit: this is about the BepInEx plugin metadata version supplied to `[BepInPlugin]`; it does not forbid richer human-facing semantic labels elsewhere.

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


### Standard item-tooltip child alignment inside centered bubbles

**Target:** Graveyard Keeper 1.407  
**Status:** **accepted fact** for the inspected standard item-tooltip `WidgetsBubbleGUI` path.

#### Verified behavior

The standard item tooltip is populated through `ItemDefinition.GetTooltipData(Item, bool)` and rendered as child bubble widgets.

For text rows on the inspected path:

1. `BubbleWidgetText.Draw(BubbleWidgetTextData)` copies `data.alignment` to the child `UILabel.alignment`.
2. That alignment controls text **inside the child label**; it does not by itself choose where the child widget sits inside the enclosing bubble.
3. A centered `WidgetsBubbleGUI` centers each child widget as a whole.
4. `WidgetsBubbleGUI.UpdateSize()` computes the enclosing bubble width from the **maximum current child-widget width**, then normal repositioning lays out the children.
5. Consequently, a short one-line child whose text alignment is `Left` can still look visually centered when the child widget itself shrinks to roughly the text width.
6. On PrayerClarity 0.2.47, runtime acceptance confirmed that expanding only selected left-aligned content children to the **already-existing maximum native child width** before stock size/reposition preserves the outer parchment width while making the intended left edge visible.

#### Engineering implications

- Distinguish **text alignment within a child** from **child placement within the bubble** before treating an apparent alignment defect as an enum/value error.
- Do not change the entire bubble/container alignment merely to left-align one semantic subset if other rows must remain centered.
- If the desired content span already exists naturally in another child, reusing that current maximum can change internal alignment without introducing a new fixed outer width.
- Keep this separate from the accepted Technology `overflowWidth` lifecycle above; the two facts answer different UI questions.

#### Evidence provenance

- Graveyard Keeper 1.407 host inspection of `BubbleWidgetText.Draw` and `WidgetsBubbleGUI.UpdateSize/Reposition`.
- PrayerClarity Rebalanced 0.2.47, exact accepted runtime source `6b3aa5399c8913d368f2b09bab963326db17e7f3`; user runtime acceptance on 2026-09-26.

#### Applicability limits

This establishes the inspected **standard item-tooltip bubble family** and the accepted 0.2.47 use of that lifecycle. It does not imply that every NGUI table, dialogue bubble, pulpit panel, or unrelated custom tooltip uses the same parent alignment or child-sizing policy.

#### Known consumers

- `NikichMods/PrayerClarity` — prayer-item Base Result / On Success content alignment.
