# Technology Work / Perk Audit

Status: **research-only diagnostic**, 2026-09-30.

## Question

For authored-visible Graveyard Keeper 1.407 Technology **Work** and **Perk** unlocks, what native data actually explains the user-visible effect well enough to support Detailed Technology Tooltips without a hand-maintained wiki table?

In particular, this audit must resolve sparse `p_t_*` gathering perks such as Gold ore / Silver ore / Sulfur by locating their real native consumers rather than inferring behavior from names or external guides.

## Existing evidence before the probe

Static 1.407 host inspection already proves:

- `TechDefinition.GetUnlocksList()` distinguishes Work and Perk unlocks;
- Work unlocks resolve `ObjectGroupDefinition`;
- `GameSave.IsWorkAvailible(ObjectDefinition)` gates `need_unlock_work` objects through the first object group and `unlocked_works`;
- Perk unlocks resolve `PerkDefinition`;
- `GameSave.UnlockPerk` adds `PerkDefinition.output_res` to player parameters;
- `CraftDefinition.GetPerkValue` consumes unlocked normal perks through `stars`;
- `SmartExpression.Ppar(name)` reads a player parameter;
- `ResModificator.ProcessItemsListBeforeDrop` evaluates `Item.self_chance` / `common_chance` before craft/object drops;
- `SmartExpression.GetRawExpressionString()` exposes the authored expression without evaluating or mutating it.

The missing evidence is the **serialized 1.407 content** tying each visible Work/Perk to those mechanisms. The prior full `resources.assets` audit established population counts but did not persist the per-unlock Work/Perk consumer map.

## Why a runtime audit is justified

The original full `resources.assets` input is not currently available in the working environment. Public historical data dumps are incomplete for SmartExpression fields and are not authoritative for 1.407.

A one-shot read-only runtime audit is therefore lower-risk and more direct than reconstructing current behavior from old dumps or Wiki prose.

## Diagnostic behavior

The diagnostic:

- patches `TechTreeGUI.Open()` only to choose a deterministic point after localization and game balance are available;
- runs once per process when the Technology tree is opened;
- does not mutate balance, save data, progression, drops, recipes, perks, Work availability, UI content, or localization;
- enumerates only authored-visible Work/Perk unlocks;
- for Work, records authored description presence plus the native gated object-group members;
- for Perk, records `show`, authored description presence, `stars`, `output_res`, and references from:
  - craft/object-craft `linked_perks`;
  - craft/object-craft output item chance expressions;
  - object drop item chance expressions;
  - the inspected object/craft SmartExpression seams;
- logs raw SmartExpression text but never evaluates those expressions for the audit.

## User procedure

1. Install `GKTechnologyUnlockAudit.dll` alongside the normal mods.
2. Start the same Graveyard Keeper 1.407 save.
3. Open the Technology tree once.
4. Exit the game and return `LogOutput.log`.
5. Remove the diagnostic DLL afterward.

No save cleanup is required because the diagnostic is read-only.
