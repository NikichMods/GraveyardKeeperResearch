# Graveyard Keeper 1.407 — UI, Input, Timing and Environment Internals

This document consolidates reusable host/runtime facts promoted from PrayerClarity, Gamepad Tooltip Position Fix, Meditation Speed, Rain & Wind Volume Controls, Keeper's Lantern, Crafting Planner, and Bite Countdown.

## Technology tooltip width lifecycle

The canonical detailed entry remains in `docs/GAME_INTERNALS.md#technology-tooltip-width-lifecycle`.

Key accepted result: on the inspected Technology tooltip path, the live NGUI label uses `ResizeFreely`; finite `UILabel.overflowWidth` is the effective expansion/wrap ceiling, and the outer bubble sizes from child geometry after the label has reprocessed its text.

Do not generalize that exact width owner to unrelated tooltip families without evidence.

## Technology-tree gamepad focus and unlock-tooltip ownership

**Status:** verified static host-path fact for Graveyard Keeper 1.407.

Exact 1.407 decompilation was inspected from `Kupie/GYK_DECOMP`, commit `6abf79199d92482af1c7573870dd9a20ec2270b9`; that source reports `LazyConsts.VERSION = 1.407f`.

On the Technology-tree path:

- `TechTreeGUI` / `BaseGUI` own gamepad navigation through `GamepadNavigationController`;
- the parent `TechTreeGUIItem` receives the `GamepadNavigationItem` focus/select callbacks;
- `TechTreeGUIItem.Draw` creates/draws its child `TechTreeGUIUnlockItem` objects, but passes `init_tooltip = false` for those children in gamepad mode;
- when `init_tooltip` is false, `TechTreeGUIUnlockItem.Draw` disables the child `BoxCollider2D`;
- `TechTreeGUIItem.InitGamepadTooltip` instead appends every visible child `TechUnlock.GetTooltip` result into the parent Technology tooltip;
- in mouse mode, the same child renderer initializes each child Tooltip independently by clearing it and invoking that child's `TechUnlock.GetTooltip`;
- ordinary directional input is already owned by `BaseGUI.OnPressedLeft/Right/Up/Down -> GamepadNavigationController.Navigate`.

Reusable implications:

- the inspected 1.407 Technology path has no native **used** child-`TechUnlock` gamepad-focus lifecycle to reuse directly; gamepad presentation is intentionally parent-focused and combined;
- a UI extension that needs one child unlock at a time can reuse the native single-unlock writer `TechUnlock.GetTooltip` without creating new Technology/save state;
- taking ordinary D-pad directions for an internal child selector would conflict with native tree navigation unless the extension introduces and clearly owns a separate sub-mode or uses otherwise-unused semantic actions.

**Applicability limit:** this establishes the code path and ownership used by GK 1.407. It does not claim that no serialized prefab could contain additional dormant components, and it does not generalize to non-Technology UI families.

**Evidence provenance:** PrayerClarity Better Save Soul Technology navigation research, 2026-09-24; exact host source identity above.

## Gamepad tooltip placement lifecycle

**Status:** accepted host-lifecycle fact for the inspected bubble path.

Relevant native seams include:

- `Tooltip.Show(bool for_gamepad)` / `TooltipBubbleGUI.Show(...)`;
- `Tooltip.SetData(...)` and related data mutation;
- `TooltipsManager.Redraw()`;
- `WidgetsBubbleGUI.Update()`;
- `BaseBubbleGUI.UpdateBubble`.

Exact GK 1.407 lifecycle inspection established that vanilla bubble code invokes `WidgetsBubbleGUI.Update()` after layout/content work and recomputes bubble position from current screen/collider/content inputs. `BaseBubbleGUI.offset` is an additive translation inside that dynamic calculation, not a persistent absolute-position mode.

Reusable implications:

- show/data-change callbacks can be too early when final geometry matters;
- a one-time offset can be insufficient when vanilla recomputes position later;
- if placement depends on final width/height and current screen/collider state, the final-writer/lifecycle must be proved before replacing a late recurring correction with earlier event hooks.

**Evidence provenance:** Gamepad Tooltip Position Fix accepted 1.3.0, source `fefe71d3492221d879efd51d3cafaabceab8c115`.

**Applicability limit:** this proves the inspected WidgetsBubbleGUI/BaseBubbleGUI family and target screens, not every UI widget.

## Native screen-size / NGUI scaling lifecycle

**Status:** verified static + accepted runtime for the inspected HUD context.

Graveyard Keeper owns GUI resolution changes through:

`MainGame.OnScreenSizeChanged(int w, int h)`

The path updates the configured pixel size, sets `MainGame.ui_root.manualHeight = screenHeight / gui_pixel_zoom`, calls `GUIElements.RecalcScreenResolution(w, h)`, and then lets the NGUI hierarchy recalculate layout.

Crafting Planner runtime evidence showed that direct screen/root-derived positioning can produce active UI outside the viewport, while content cloned/kept under the live `HUD.zone_name` parent coordinate context remained visible and followed resolution changes.

Reusable implication: attach gameplay HUD additions to a verified native parent/anchor context and express only small local offsets there; do not use raw `Screen.width/height` arithmetic as a substitute for understanding NGUI ownership.

**Applicability limit:** the `HUD.zone_name` parent is accepted for the Planner HUD use case, not a universal anchor for unrelated UI.

## WaitingGUI timing ownership

**Status:** accepted fact.

Meditation waiting is owned by `WaitingGUI`.

Vanilla waiting installs:

- `Time.timeScale = 10f`;
- `Time.fixedDeltaTime = 0.083333336f`.

`WaitingGUI.StopWaiting()` restores:

- `Time.timeScale = 1f`;
- `Time.fixedDeltaTime = 0.016666668f`.

`WaitingGUI.Update()` restores player energy/HP from scaled `Time.deltaTime`. `EnvironmentEngine.Update()` advances world time from scaled `Time.deltaTime`.

Reusable implication: changing the timing inputs composes with native recovery/world progression; do not reimplement those consumers unless necessary.

**Evidence provenance:** Meditation Speed accepted 1.0.0, source `37964f2e17d52d2d81df9265778ff2b5ef21fe80`; research harness source `3e5fd476584eb8749a98ca58b1b3ffa575b1d9bc`.

## Fixed timestep during accelerated meditation

**Status:** accepted result for the tested meditation-speed range.

Accepted mapping:

| Relative meditation speed | timeScale | fixedDeltaTime |
| --- | ---: | ---: |
| 1x | 10 | 0.083333336 |
| 2x | 20 | 0.16666667 |
| 4x | 40 | 0.33333334 |

This preserves roughly the vanilla-meditation real-time fixed-callback rate (~120/s). The highest 4x boundary was runtime-tested without a reported simulation anomaly.

Static inspection found fixed-step consumers including `CustomUpdateManager.FixedUpdate()`, WGO custom fixed updates, movement/kick components, and `DropsList.FixedUpdate()`; some behavior is per-call rather than perfectly delta-normalized.

**Applicability limit:** this is accepted for the tested meditation range/environment, not a general recommendation to scale Unity fixedDeltaTime for arbitrary game-speed mods.

## Slider input and host hold-repeat

**Status:** accepted fact.

`BaseGUI.Init()` routes:

- `GameKey.SliderDec -> OnPressedSliderDec`;
- `GameKey.SliderInc -> OnPressedSliderInc`.

The same native semantic path receives keyboard and gamepad D-pad input.

`LazyInput` treats SliderDec/SliderInc as hold-repeat keys: repeat begins after 0.3 and continues about every 0.07, while its held-update timing uses scaled `Time.deltaTime`. Under accelerated meditation this can turn one physical hold/press into rapid repeats in real time.

Accepted mitigation used native `LazyInput.WaitForRelease` instead of adding a custom polling/debounce timer.

Reusable implication: before inventing input polling/debounce, inspect the game's semantic action handlers and release lifecycle, especially when global timeScale is altered.

## WaitingGUI button-tip presentation seam

**Status:** accepted fact for WaitingGUI.

The WaitingGUI single-tip `ButtonTipsStr.Print(GameKeyTip)` event occurs after vanilla enters Waiting and installs its timing values.

A UI extension can therefore use that event, gated to the active WaitingGUI and its exact button-tips instance, as an event-driven presentation seam. Broadly patching inherited `BaseGUI.Open` by method name can accidentally observe unrelated GUIs.

## Localization reload seam

**Status:** accepted fact.

`GameSettings._cur_lng` exposes the current language code. `GJL.LoadLanguageResource` is an event boundary after which custom localization can be re-injected into the active `GJL.cur_lng.dict`.

Reusable implication: localization extensions can be event-bound to language loading/changing rather than polling language state.

**Evidence provenance:** Food & Drink Rebalance accepted production/runtime data.

## Runtime language-font ownership for custom UILabels

**Status:** accepted host/runtime fact for Graveyard Keeper 1.407.

The game's live language-change path does more than replace localized strings. `GameSettings.ApplyLanguageChange()` loads the new language and then asks the GUI layer to update current labels. Native `LocalizedLabel.Localize()` assigns its localized text and calls `GJL.EnsureLabelHasCorrectFont(label, true)`; `BaseGUI.UpdateLocalizedLabels()` / `GUIElements.UpdateLanguageChangeForAllBaseGUI()` likewise delegate child-label font correction to GJL.

PrayerClarity runtime acceptance exposed the practical consequence for mod-owned NGUI labels: a raw persistent `UILabel` that merely copies `bitmapFont` / `trueTypeFont` once at creation can retain stale font assets or glyph metrics after live CJK/Latin/Cyrillic language switching. Re-running `GJL.EnsureLabelHasCorrectFont(label, true)` at the label's redraw/consumer boundary restored correct Japanese, Korean, English and Russian presentation without a mod-maintained language-to-font map.

Reusable implication: when a mod creates or persists raw NGUI `UILabel` objects outside the native `LocalizedLabel` lifecycle, treat GJL as the authoritative current-language font owner. At the natural redraw/localization boundary, explicitly rejoin `GJL.EnsureLabelHasCorrectFont(..., true)` rather than caching a font indefinitely or maintaining a parallel per-language font table.

**Evidence provenance:** Graveyard Keeper 1.407 static host inspection plus PrayerClarity: Rebalanced 0.2.37 runtime acceptance, exact source `d95eb760105fa0fdcb4922a1d0f270eb5dbc2c05`.

**Applicability limit:** this proves the inspected NGUI/current-language font lifecycle. It does not imply that every Unity text component or non-NGUI UI family uses GJL.

## Weather audio ownership

**Status:** accepted host-path fact.

Established GK 1.407 weather/audio path:

`SmartWeatherState.Update`
-> `SmartWeatherState.UpdateWeatherVolume(float weather_value)`
-> `GetWeatherMusicId()`
-> native Rain/Wind normalization
-> `SmartAudioEngine.me.SetSoundVolume(weatherMusicId, volume)`

Rain/Wind resolve to the stable audio groups:

- `rain_environment`
- `wind_environment`

Installed-game reflection also confirmed live `SmartWeatherState` methods `GetWeatherMusicId()`, `SetWeatherSoundEnable(Boolean)`, and `UpdateWeatherVolume(Single)`.

**Evidence provenance:** Rain & Wind Volume Controls accepted 1.1.0, source `f27c489075b7091d41a04609303b91995fdcfcef`.

**Applicability limit:** this proves the rain/wind path; do not infer every environment sound uses the same state class.

## Environment preset reapply after late location restoration

**Status:** accepted compatibility result, host seam reusable with caution.

Keeper's Lantern testing established that `EnvironmentEngine.ApplyEnvironmentPreset` can safely reapply the already-selected current vanilla environment preset after a third-party late location restoration, provided the world/location has settled and the exact current preset is used.

The accepted Save Now integration schedules the reapply for the next Unity frame after `SaveNow.Plugin.RestoreLocation()` returns; earlier time-based/too-early attempts were rejected.

Reusable implication: when another system moves the player/location late, refresh environment state at the actual post-owner seam rather than guessing elapsed time from spawn.

**Evidence provenance:** Keeper's Lantern accepted 1.0.12, source `8df0a848aa8e8bfb5748f6937731db3b01d4430a`.

**Applicability limit:** this is not a blanket instruction to reapply environment presets after every teleport.

## Final ambient-light observation

**Status:** accepted cross-mod integration fact.

Unity's global `RenderSettings.ambientLight` represents the final ambient colour seen by downstream code after ordinary scene lighting updates. Keeper's Lantern writes its transformed ambient result there in `LateUpdate`.

Bite Countdown successfully sampled `RenderSettings.ambientLight.grayscale` once per bite wait after `WaitForEndOfFrame`, allowing a presentation-only overlay to adapt to the final scene ambient without depending on Keeper's Lantern assemblies/configuration.

Reusable implication: when a mod only needs the final ambient presentation state, sampling the final Unity ambient output after late-frame lighting work can be less coupled than integrating with a specific lighting mod.

**Evidence provenance:** Bite Countdown accepted 1.0.3, source `2b5c3cfec638098328eaf5707d0e79effb9a1bf6`.


## Pray GUI craft-button anchor ownership

**Status:** accepted runtime fact for Graveyard Keeper 1.407 `PrayCraftGUI`.

PrayerClarity pulpit-layout research established the following native NGUI ownership chain for the sermon action button:

- `UI Root/Pray GUI/window/craft button` carries the root `UILabel`;
- that root `UILabel` is vertically anchored directly to `UI Root/Pray GUI/window`;
- its anchor update mode is `OnUpdate`;
- both vertical anchors use the window's bottom edge (`relative = 0`) with bottom/top absolute offsets `+28/+44`;
- the visible `craft button back` `UI2DSprite` is anchored to the root craft-button widget.

Runtime evidence also showed that writing `craft button.transform.localPosition` is not a durable final placement mechanism: NGUI anchor resolution later restores the button from the root-window anchor contract.

Reusable implication: for this exact Pray GUI button family, change the verified upstream owner (for example the root-window geometry) or the anchor data itself when placement must change. Do not assume a late-looking transform write owns final button placement while the widget remains anchored with `OnUpdate`.

**Evidence provenance:** PrayerClarity Pulpit Geometry Probe 0.1.1, runtime on Rebalanced 0.2.35; probe source `d33c141f132b430d79140b96c683886851b9cc0a`.

**Applicability limit:** this proves the inspected `PrayCraftGUI` craft-button hierarchy in Graveyard Keeper 1.407. Do not generalize the exact anchor targets/offsets to unrelated NGUI buttons without inspection.

## Known consumers

- PrayerClarity
- Gamepad Tooltip Position Fix
- Meditation Speed
- Rain & Wind Volume Controls
- Keeper's Lantern
- Crafting Planner
- Bite Countdown
- Food & Drink Rebalance
