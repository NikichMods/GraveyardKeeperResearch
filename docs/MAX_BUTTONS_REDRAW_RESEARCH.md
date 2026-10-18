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
