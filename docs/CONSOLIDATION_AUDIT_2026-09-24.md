# Cross-Project Knowledge Consolidation Audit — 2026-09-24

## Purpose

One-time retrospective consolidation of accepted Graveyard Keeper 1.407 research that had accumulated across project docs, test-build logs, frozen/research branches, and repository history.

The goal was not to copy every experiment into one place. Raw logs/branches remain evidence archives. This pass promoted only durable facts likely to be reused by more than one project.

## Repositories reviewed

- PrayerClarity
- DayWheelQuestMarkers
- SpecializedStorage
- FoodAndDrinkRebalance
- GamepadTooltipPositionFix
- RainWindVolumeControls
- KeepersLantern
- BetterSaveSoulRebalance
- Graveyard-Keeper-Meditation-Speed
- BiteCountdown
- WhoBuysThis
- Crafting-Planner
- VisibleBodyArmour
- GraveyardKeeperResearch

The review used current canonical docs first, then accepted test/research evidence and relevant frozen/research history where the main docs pointed to it.

## Promotions completed

### Already canonical before this pass

- Technology tooltip width lifecycle from PrayerClarity was already promoted into `docs/GAME_INTERNALS.md`.
- Performance-diagnostics facts already lived in `docs/PERFORMANCE_EVIDENCE.md` / `docs/TEST_LOG.md`.

### Newly promoted shared knowledge

- Dialogue/task/FlowCanvas semantics -> `docs/DIALOGUE_QUEST_AND_FLOWCANVAS.md`
- Crafting/inventory/trading semantics -> `docs/CRAFTING_INVENTORY_AND_TRADING.md`
- UI/input/timing/environment seams -> `docs/UI_INPUT_TIME_AND_ENVIRONMENT.md`
- Fishing lifecycle/anchor semantics -> `docs/FISHING_RUNTIME.md`

## Important project-local findings intentionally not duplicated

The following remain canonical in owning repositories because they are product-specific rather than reusable host facts:

- PrayerClarity prayer formulas, balance, UX, candidate/release state and acceptance identities;
- Day Wheel Quest Markers reminder-product policy, manifest schema/counts, exact reminder coverage and release identities;
- Specialized Storage storage suitability rules, x4/cap-200 balance and marker UX;
- Food & Drink Rebalance food/alcohol balance and custom-buff product behavior;
- Keeper's Lantern lighting balance values, lantern art/placement and Darker Nights policy;
- Better Save Soul Rebalance balance contract and Gratitude surcharge design;
- Bite Countdown ring visual tuning values;
- Who Buys This? buyer-display product policy and price-scope decisions;
- Crafting Planner P0/P1 product scope and unresolved UI/input decisions.

## Findings reviewed but not promoted as accepted shared facts

### VisibleBodyArmour

The paused project contains valuable reverse-engineering notes on Keeper render layers, equipment lookup, CharacterSkin, NPC clothing and sprite geometry. However its own research document explicitly describes the current implementation direction as provisional and the production project is paused.

This pass therefore did **not** reclassify those findings as accepted shared knowledge. They remain available in `NikichMods/VisibleBodyArmour/docs/RESEARCH.md` and can be promoted later after the relevant evidence state is closed.

### Rejected/superseded candidates

Rejected candidates were used only when they proved a host invariant later confirmed by an accepted fix. Their rejected behavior itself was not promoted.

Examples:
- Better Save Soul Rebalance 1.1.0's broken shared-`needs` mutation is retained only as evidence for the renderer parallel-list invariant proven by accepted 1.1.1.
- Bite Countdown rejected visual prototypes are not canonical; only accepted lifecycle/geometry conclusions are.
- PrayerClarity rejected 0.2.17-0.2.19 presentation candidates remain project evidence, not shared facts.

## Coverage conclusion

The largest historical knowledge concentrations were DayWheelQuestMarkers, PrayerClarity, Crafting Planner, Who Buys This?, Bite Countdown, Meditation Speed, and the post-audits of Specialized Storage/Gamepad Tooltip/Rain-Wind/Food & Drink. Their reusable accepted findings are now reachable from `docs/RESEARCH_INDEX.md` without archaeological branch/history search.

This does **not** mean every old branch can be deleted. Frozen candidates, probes and historical commits remain evidence archives for provenance and exact build identity.

## Ongoing rule

Future accepted reusable findings should be promoted as part of the iteration that accepts them:

`project canonical docs -> shared research when cross-project -> RESEARCH_INDEX`

A future large retrospective pass should be unnecessary unless this promotion discipline lapses or a large pre-policy repository is imported.
