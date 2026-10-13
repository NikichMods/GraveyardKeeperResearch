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

### Native tooltip backing and NGUI geometry commit — 1.407

Direct installed level2/firstpass inspection for ResearchableAlchemyRecipes StudyTableWorkProbe 0.1.24, 2026-10-13:

- GUIElements native TooltipBubbleGUI 25244 has content widget 25242; optional inherited `back_spr` is null. The four `corners` GameObjects contain tail sprites and BubbleCornerPoint attachments, not the rectangular backing. The backing is widget 33075 at `back and points/back`, anchored to intermediary widget 24430, which surrounds the root content. Disabling all four corner groups preserves that rectangle. Earlier project descriptions calling corners frame groups were incomplete.
- Installed UIPanel.LateUpdate invokes panel UpdateSelf once per frame; UpdateSelf calls UpdateWidgets before filling draw calls. UpdateWidgets calls UIWidget.UpdateTransform, UpdateVisibility and UpdateGeometry. Moving a bubble only in its own LateUpdate does not establish that the change preceded that frame's mesh generation. A reference-scoped prefix at the backing's UIPanel.UpdateSelf can prepare the owned bubble immediately before that consumer while preserving native processing of all panel contents.
- UIWidget alpha invalidates cumulative/final-alpha state; geometry uses CalculateFinalAlpha at UpdateGeometry. Native collider-mode WidgetsBubbleGUI.Update raises a zero root alpha, so a withheld owned tooltip must contain that native exposure until its render-ready point.
- Installed UILabel.OnFill applies effectStyle/effectColor to the label's generated geometry, not individual rich-text colour spans. It returns before effects when bitmapFont.packedFontShader is true; dynamic-font paths do not take that bitmap-only guard. Selective outlined reagent headings therefore need separate text elements, and language-wide support cannot be inferred without checking the actual language-font configuration. This is an inspected mechanism limitation, not visual outline acceptance.

Inputs: level2 SHA-256 `1f75a0328dc4987f3020688a0feb0d310c2bf3f6e288c6e4a64ad09db1f39041`; Assembly-CSharp SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`; firstpass identity is recorded in the project's build manifest. Native BaseBubbleGUI/WidgetsBubbleGUI sources at Kupie/GYK_DECOMP `6abf79199d92482af1c7573870dd9a20ec2270b9` agree with inspected ownership. These are static lifecycle facts; no claim that one ordering defect is the sole cause of an observed flash, or that candidate 0.1.24 passes live rendering, follows from them. Private dumps stay outside tracked files.

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

## Graveyard Keeper 1.407 localization lookup ABI

**Status:** accepted host/runtime fact.

Direct metadata inspection of an installed Steam Graveyard Keeper 1.407 `Assembly-CSharp-firstpass.dll` establishes:

- module MVID: `7b81560f-fee5-4bdd-ac8c-058e486c8a3f`;
- inspected assembly SHA-256: `9dc6def3b7715dd27eeb168ddc0af47e31c6f38d3fbee24bf592899392026498`;
- `GJL` contains a public static exact overload `string L(string lng_id)`;
- the same type also contains multiple public static `L` overloads with 2–4 arguments and differing parameter types.

Reusable implication: code that only needs one-key localization should bind the exact `GJL.L(string)` signature. Do not select a merely compatible `L` overload by scanning methods or filling optional/default arguments.

**Evidence provenance:** direct metadata inspection of the user's installed Graveyard Keeper 1.407 runtime assembly during Compact Crafting Tooltips compatibility hardening. Only derived identifiers/signatures are retained; the proprietary game assembly is not stored.

**Applicability limit:** the MVID identifies the inspected firstpass module. It is separate from the verified 1.407 `Assembly-CSharp` MVID `6f50b8e7-156b-49ac-bbe8-7505894b2364`.

## Runtime language-font ownership for custom UILabels

**Status:** accepted host/runtime fact for Graveyard Keeper 1.407.

The game's live language-change path does more than replace localized strings. `GameSettings.ApplyLanguageChange()` loads the new language and then asks the GUI layer to update current labels. Native `LocalizedLabel.Localize()` assigns its localized text and calls `GJL.EnsureLabelHasCorrectFont(label, true)`; `BaseGUI.UpdateLocalizedLabels()` / `GUIElements.UpdateLanguageChangeForAllBaseGUI()` likewise delegate child-label font correction to GJL.

PrayerClarity runtime acceptance exposed the practical consequence for mod-owned NGUI labels: a raw persistent `UILabel` that merely copies `bitmapFont` / `trueTypeFont` once at creation can retain stale font assets or glyph metrics after live CJK/Latin/Cyrillic language switching. Re-running `GJL.EnsureLabelHasCorrectFont(label, true)` at the label's redraw/consumer boundary restored correct Japanese, Korean, English and Russian presentation without a mod-maintained language-to-font map.

Reusable implication: when a mod creates or persists raw NGUI `UILabel` objects outside the native `LocalizedLabel` lifecycle, treat GJL as the authoritative current-language font owner. At the natural redraw/localization boundary, explicitly rejoin `GJL.EnsureLabelHasCorrectFont(..., true)` rather than caching a font indefinitely or maintaining a parallel per-language font table.

**Evidence provenance:** Graveyard Keeper 1.407 static host inspection plus PrayerClarity: Rebalanced 0.2.37 runtime acceptance, exact source `d95eb760105fa0fdcb4922a1d0f270eb5dbc2c05`.

**Applicability limit:** this proves the inspected NGUI/current-language font lifecycle. It does not imply that every Unity text component or non-NGUI UI family uses GJL.

## Native font effects and independent tooltip text widgets (1.407 static inspection)

All 11 GJL languages route either to the ordinary atlas fonts (including tiny_font) or to custom ch_jp_12 (ja/zh_cn, size 12, spacingY 2) and korean (ko, size 13, spacingY 1). Direct resources.assets inspection finds all these UIFonts have mAtlas set and no mReplacement/dynamic font. UIFont.packedFontShader follows a replacement first, then returns false for atlas-backed fonts; otherwise it checks the material shader name for Packed. Thus none of these built-in language fonts takes UILabel.OnFill's packed-font outline exclusion. This establishes static eligibility, not visual quality or runtime acceptance in every language; unknown/replaced font paths still need capability-aware local fallback.

BubbleWidgetText.Draw applies the requested native style and GJL.EnsureLabelHasCorrectFont before text assignment. Separate BubbleWidgetTextData entries therefore provide separate native UILabel effect scopes. Tooltip prefab 25244 uses SimpleUITable 23831 with offset=0; native table positioning and WidgetsBubbleGUI sizing consume child widget dimensions. NGUIText.CalculatePrintedSize counts newline line heights, including trailing blank lines. A split renderer must preserve text/blank lines and colour scopes and still verify installed spacing.

NGUIText.ParseSymbol accepts two-digit alpha tags as byte/255; [AA] sets 170/255 and [FF] restores 1. Print initializes alpha to 1 per label. These tags can span a source's rich-text Study annotation and reset before the next row; they do not change glyph widths. UILabel Outline creates four diagonal effect copies and accepts fractional effectDistance, which applies to the entire label. Source-specific effects therefore must not be applied to a shared body label.

Evidence: installed 1.407 Assembly-CSharp/firstpass IL and resources.assets SHA-256 215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca (rechecked 2026-10-13), plus previously pinned level2 tooltip metadata. Private extraction/IL remains in ResearchableAlchemyRecipes work/palette; no native assets/source dumps are published. Consumer experiment: StudyTableWorkProbe 0.1.28; installed visual acceptance is separate.

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

## Native craft tabs and composed window input — 2026-10-12

**Status:** verified static source/scene ownership and bounded installed usability evidence; the returned 0.1.2 run confirms correction of the 0.1.1 key-map enumeration defect. Picker layer-warning ownership remains open.

Source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, CraftGUI, CraftTabGUI, MixedCraftGUI, ResourceBasedCraftGUI and BaseGUI. Direct installed level2 SHA-256: `1f75a0328dc4987f3020688a0feb0d310c2bf3f6e288c6e4a64ad09db1f39041`; Assembly-CSharp.dll matches the shared accepted 1.407 identity.

- GUIElements serializes distinct references to CraftGUI, ResourceBasedCraftGUI and the tabbed MixedCraftGUI. Both ordinary Survey and the tabbed mixing panel have `add_to_opened_stack=true`; the latter is a separate UI-root window, not simply a component parented inside CraftGUI's content transform.
- CraftGUI.RedrawCraftTabs builds CraftTabGUI instances and changes the header height from 50 to 82 UI units. The inspected native tab prefab is 66 by 32 units. CraftTabGUI.Draw provides icon presentation and a click delegate; it has no authored text caption field.
- Direct scene tab geometry: CraftTabGUI background and icon share a transform parent and Center pivot (4); background is 66x32 and icon widget 54x32/Simple/pixel-size 1. The icon has no anchor targets; native SetSelectedState changes tint and background sprite, not the icon transform. Custom art alignment must distinguish full-canvas center from opaque artwork center. Exact project-specific artwork offsets belong to the consuming project, not this host geometry fact.
- Study science header is native data/UI: mf_alchemy_survey.additional_header_items contains science; WorldGameObject.GetUniversalObjectInfo reads the station item count into right_items; UniversalObjectInfoGUI.Draw creates native BaseItemCellGUI rows using DrawItem. A consuming mod that merely uses this native header has not thereby authored or replaced the science sprite. This establishes ownership, not a diagnosis of perceived roughness or a guarantee that other mods leave rendering untouched.
- Ordinary tab selection filters craft cards by tab_id. A `?` tab is explicitly wired to MixedCraftGUI: it hides the card scroll view, opens the tabbed mixing window and changes button-tip/controller ownership. It is not a generic container for Survey or arbitrary custom content.
- BaseGUI.Open adds the window to the native stack and calls the previous top window's OnHiddenByAnotherGUI, which disables that window's controller. BaseGUI.Update consumes semantic keys only when is_shown_and_top is true, which means the window is exactly the stack's last entry. Merely setting add_to_opened_stack=false would therefore stop this BaseGUI key path; do not infer generic embedded-input support from that flag.
- MixedCraftGUI forwards PrevTab/NextTab to CraftGUI while acting as its subwindow, and closes the outer craft window on its close action. ResourceBasedCraftGUI has no equivalent tab bridge; its native sample-picker/craft behavior must remain owned by its own callbacks if composed elsewhere.
- BaseGUI.Hide removes the window from the stack, disables its controller and clears keys. Survey Hide also destroys extra ingredient widgets. Closing/switching composition must account for both owners; making a parent GameObject inactive without native Hide is not an equivalent lifecycle operation.

- CraftTabGUI.Draw resolves ordinary IDs as tab_<id> and ? IDs as tab_custom_<suffix>; SetSelectedState supplies selected/normal tint. Direct inspection of the accepted resources.assets identity confirms tab_book (33x25, quill/scroll) and tab_custom_alch (40x23, paired flasks). These are derived identifiers/dimensions; no extracted assets are retained publicly.
- Bounded tab-asset use mapping in the accepted 1.407 resources.assets: tab_bookcraft maps to ten authored craft rows at table_book_constr (church workbench, writing supplies/paper/covers); tab_alchemy_3_1 maps to eighteen rows at alchemy_table_zombie whose item outputs are all AlchemyType.Essence. tab_alchemy has no ordinary craft tab rows, but is used by GlobalCraftControlGUI.RedrawTabs: known/enabled WorldZone IDs are passed to CraftTabGUI.Draw, and alchemy is a world-zone ID in church_group. Thus absence from craft_data is not evidence of an unused tab asset. Source owner: GlobalCraftControlGUI in the same pinned decompilation; these are static bindings, not installed visibility observations.
- BaseGUI.Update enumerates the live gamekey_delegates.Keys and invokes delegates through that dictionary field. Do not mutate that dictionary from a callback, including restoration of existing values. ResearchableAlchemyRecipes 0.1.1 recorded eight Dictionary KeyCollection Enumerator exceptions immediately after composed-window closes; its source restores PrevTab/NextTab values there. A .NET Framework reproduction confirms that existing-value replacement invalidates this enumeration. Installing a separate map and restoring the original field reference completes the same enumeration pattern; the returned 0.1.2 run records eleven paired closes/layout restorations and zero enumeration exceptions, confirming the correction in that run.

Runtime limits: user observations for exact ResearchableAlchemyRecipes 0.1.1 cover keyboard/mouse/gamepad switching, picker select/cancel and reopening; log SHA-256 04aa127c681b8a3df008dc33324c27011d6eb64a577e682ca1d3f8c5d050b028 records ten paired closes/layout restorations alongside the eight exceptions, 20 missing custom tab icons and six NGUI layer warnings with unassigned cause. Do not infer a clean session, arbitrary-mod compatibility or final UI acceptance. The returned 0.1.2 log (SHA-256 efd6f0ac9cd7234723ada05e0137560c315d859b79bad7433184f13226942682) has no missing-tab-sprite or probe-error markers, but thirty NGUI layer warnings remain around picker interactions; cause unassigned. Startup identifies 0.1.2; installed binary digest was not available for independent recheck. Exact source/DLL identities, product-specific composition and its 0.1.2 correction are owned by ResearchableAlchemyRecipes docs/TEST_LOG.md and docs/DESIGN.md.

## Graveyard Keeper 1.407 current language selector codes

**Status:** accepted runtime fact for the inspected Steam/Windows 1.407 build.

A real in-game language-selector cycle loaded exactly these 11 locale codes:

- `en`
- `de`
- `fr`
- `pt-br`
- `es`
- `ru`
- `it`
- `pl`
- `ja`
- `zh_cn`
- `ko`

The same run exercised `GameSettings.ApplyLanguageChange()` / `GJL.LoadLanguageResource(...)` while the game was live. This list is suitable as the current 1.407 localization-completeness target for mods that author their own player-facing text.

Reusable implication: a 1.407 mod claiming support for every current in-game language should cover those 11 codes, while still treating unknown future codes as unverified rather than assuming the list is permanent.

**Evidence provenance:** Detailed Technology Tooltips 0.1.2 accepted runtime session, 2026-09-30; the BepInEx log records each loaded language resource during one selector cycle.

**Applicability limit:** this proves the language set exposed by the inspected 1.407 PC build. It is not a promise that later game versions or other platforms cannot add/remove/rename locale codes.


## Temporary native input containment for calibration — 2026-10-12

**Status:** static installed-assembly/source facts; returned exact 0.1.5 screenshot/log demonstrates the panel, chosen exports and four restoration paths. No general input compatibility guarantee. The tool is removed after selection in 0.1.6; details/identities remain in the consuming project TEST_LOG.md.

- Installed Assembly-CSharp-firstpass.dll SHA-256 9dc6def3b7715dd27eeb168ddc0af47e31c6f38d3fbee24bf592899392026498 contains public static bool UICamera.ignoreAllEvents. Exact UICamera.Update IL body (32 bytes) first reads this flag and returns when true, otherwise processing continues. A temporary tool may snapshot/restore it to suppress native NGUI event processing while a separate IMGUI panel owns pointer input; this is global event containment and must not be confused with a local icon property.
- Pinned ResourceBasedCraftGUI.Update processes keyboard Interaction before calling BaseGUI.Update; intercepting only the base update does not contain that Survey shortcut. A scoped calibration guard must account for the derived owner as well. Native key dictionaries and GUI stack need not be mutated for this containment.
- Flag restoration after mouse release and plugin destruction is a consuming-tool responsibility, not a native automatic guarantee. The current bounded implementation is research-only; installed success cannot be inferred merely from the existence of the flag or a successful build.

Bounded returned calibration evidence: log SHA-256 4a48481db590b8d0556e8c572167deabc4e3cc3502ff5907588f17bd088e88f8, installed 0.1.5 DLL digest matched its handed artifact. Four panel opens/restoration markers, six paired closes/layout restorations, 25 tab switches and no probe/enumeration errors; screenshot shows the panel and selected offsets. This observes the implemented restoration path and continued window interactions, without independently proving every input device/global UI combination. Chosen positions are product-specific and owned by ResearchableAlchemyRecipes docs/DESIGN.md.
