# Max Buttons Redux redraw attribution —2026-10-19

## Question and known baseline

Determine why Max Buttons Redux1.4.0 gamepad maximum commands throw in CraftItemGUI.Redraw, and whether Building Planner is necessary/contributes. User reopens this deferred investigation after accepting planner0.1.25 persistence. Production planner stays unchanged.

Exact installed Max Buttons DLL SHA256e811fc228ece102e389759d278bb66569884898b827369f99e11c984ffc2af30; planner0.1.25 SHA2560922e8d7c27699cbe8483c2ee6c158034c105916df256070011609c49f31b1e2. GK1.407 PC/BepInEx/Harmony, modded installation. Earlier planner0.1.21 return snapshot SHA2563972a147f116c7fc1d3a735a71c84894084ab2e2298b94ad762d970659b7fc26 contains9 NullReferenceExceptions through dynamic Redraw -> MaxButtonCrafting.SetAmount -> SetMaximumAmount -> HandleCraftGuiInput -> MainGame.Update while TechTreeGUI is open. The previous explicit planner-prefix GetComponent failure was guarded in0.1.21; remaining dynamic frames do not identify the failing member or exonerate patches. No pre-planner error-free control establishes onset. Latest persistence session did not exercise the same technology/gamepad trigger.

## Verified-static observations

- Max Buttons owns cached _craftGuiOpen, _craftItemGui and _crafteryWgo. CraftGUI_Open sets its open flag; CraftGUI_OnClosePressed clears it. HandleGamepadInput uses that flag and _unsafeInteraction, rather than actual CraftGUI top-window state.
- Its CraftItemGUI_OnOver postfix copies static native current_overed into the cached row and reads craftery from GUIElements.me.craft. This is not proof the cached row remains valid after menu changes.
- IsCraftGuiInputValid reads cached current_craft.needs/one_time_craft; despite its name, a true return causes the caller to skip processing. SetAmount changes _amount then calls native Redraw and OnOver. This is already a native-row mutation before rendering can fail.
- Native CraftGUI.Hide(bool) calls ClearList and BaseGUI.Hide independently of its OnClosePressed handler. Thus not all hide routes necessarily reset Max Buttons' flag. Need the actual runtime route, cache identity and patch ownership before assigning causality.
- Native current_craft dereferences _possible_crafts and index; Redraw also consumes normal row/UI fields. CLR/Unity-null, cleared lists and missing child fields are different failure candidates. Planner's current Redraw prefix guards Unity-null and only restores attached PlannerRowControls; its postfix calls Ensure. Planner and all other patches remain potential contributors until observed/isolated.

Inputs: preserved project logs/current source plus private exact-DLL IL/signature inspection (maxbuttons-*.txt); no third-party implementation is copied here.

## Research-method checkpoint

Direct inspection proves the suspicious cached-flag path but cannot locate the null reference inside a Harmony dynamic method or prove patch/row identity. A simple Planner-on/off control could test necessity, but a non-reproduction without context would remain ambiguous. Choose a passive observer first: real Max Buttons SetAmount and native Redraw/Hide, input scope and public window events; log before-state and propagate the exact original exception. This observes fields that the user cannot read visually and does not recreate a baseline or fix the result. If attribution remains uncertain, repeat the same short sequence with the actual planner DLL temporarily absent, keeping everything else identical; do not call partial unpatching equivalent to an absent mod.

## Probe0.1.0 contract and user actions

Source: research/MaxButtonsRedrawProbe. Independent diagnostic GUID nikich.gyk.maxbuttonsredrawprobe. No planner/Max binary replacement, result substitution, exception suppression, synthetic row mutation, save/inventory edits or production fix. Real SetAmount calls naturally retain Max Buttons' normal quantity changes. Thread-local input scope has constant overhead without per-frame snapshots/logs; detailed observations are limited to64 actual amount changes,48 window events and12 input/amount errors. Native Redraw before-state includes row null kind, real parent path, current-definition list/index, child references and attached planner controls. Startup lists exact loaded hashes and Redraw/OnOver/Hide/OnClosePressed patch owners. No recurring object/world/inventory scan.

With a gamepad:
1. Open an ordinary crafting station (e.g. sawmill), select an ordinary repeatable material recipe; use Max Buttons maximum then minimum without starting work.
2. Close the crafting menu normally. Open technologies and change4–5 branches left/right with the usual gamepad controls that triggered old errors. Close technologies.
3. Open/close a building menu with planner counters; repeat the technology branch changes, then reopen ordinary crafting and verify maximum/minimum still responds.
4. Report completion and visible symptoms. No craft/build/save is required. Leave diagnostics installed until the log is collected; the agent removes it after the investigation.

A success without the old exception establishes only a non-reproduction in that tested sequence, not a fixed issue or mod independence. If necessary a second start with the actual planner absent isolates its necessity; record loader hashes and the same trigger before comparing. A separate production gate/new owning-mod candidate is required for any fix.

## Observer0.1.0 return —2026-10-19

Private consumer log .local/evidence/max-redraw-observer-return/LogOutput.log SHA256366aee5696c89f2e034ee09671a54b83c3a7cbee6c55ea8004d4b973670fe991 loads exact observer0.1.0/planner0.1.25/Max1.4.0 and matches both baseline hashes.11 SetAmount calls and11 normal redraw results occur on ordinary skroll_skin_pig/skroll_skin_pig_2/paper_bad rows; all have craft_top=True, intact current definitions and planner_controls=clr_null.4 technology openings and multiple branch changes occur with zero logged exceptions/observer errors. This is a real non-reproduction, not an attribution/control result or fix acceptance. Current patch inventory additionally identifies QueueEverything's two Redraw prefixes/finalizer and MoveObjects' postfix alongside planner; dynamic frames alone cannot assign ownership.

Observer defect: all48 window observations were consumed by initial hide events before actual interaction; only1 startup craft_hide is retained. Thus this run proves successful actual amount changes but cannot determine cached-flag transitions during the user sequence. Fix the observer rather than infer missing state.

Narrowed hypothesis from prior exact log: before the old9 errors, ordinary wooden_plank was actually started (OnCraftPressed -> Craft -> Hide CraftGUI -> EnqueueCraft). The new instructions only exercised normal Close. Static BaseCraftGUI.OnCraft includes a direct Hide(bool) path, unlike OnClosePressed. Automatic craft-start hiding may leave Max Buttons' cached flag set, then technology inputs reach the old row. This is a hypothesis until observed, not a newly proven third-party-only root cause.

Observer0.1.1 starts a bounded relevant CraftGUI/TechTree window budget only after an actual shown craft window, separates24 Hide observations from the window counter and records cached-row state across Hide. No error/result/gameplay mutation is introduced. Next short native check: at an ordinary station with resources for1 inexpensive recipe (plank/paper), use max/min then choose1 and actually start/enqueue that1. Let the game close the crafting menu itself; after work finishes open technologies through the ordinary inventory/menu and change4–5 branches. No repeat building-menu pass is required. If the discrepancy is reproduced but planner causality remains open, the following comparison must remove the actual planner payload for one controlled start.
