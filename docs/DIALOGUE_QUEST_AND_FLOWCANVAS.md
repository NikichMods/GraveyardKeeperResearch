# Graveyard Keeper 1.407 — Dialogue, Quest and FlowCanvas Internals

This document contains reusable cross-project facts promoted from accepted Day Wheel Quest Markers research. Product-specific reminder policy remains canonical in that mod; this file keeps only host/runtime behavior.

## Weekday HUD semantic ownership

**Target:** Graveyard Keeper 1.407  
**Status:** accepted fact.

The calendar root is:

`UI Root/HUD/hud left/hud spr/circle`

The six physical weekday-symbol objects remain in fixed positions while their semantic weekday changes as the wheel advances. The authoritative current semantic value is `HUDSinIcon._sin_type`; fixed child/index position is not a valid permanent weekday/NPC identity.

Normal menus hide/deactivate the HUD rather than destroying it. If the real HUD is recreated, mod-created child visuals must be recreated/rebound rather than assuming the original Unity objects survive forever.

**Evidence provenance:** `NikichMods/DayWheelQuestMarkers/docs/VERIFIED_RUNTIME_DATA.md`, accepted production lineage through 1.1.9.

**Applicability limit:** this proves the inspected weekday HUD path, not arbitrary HUD-widget recreation behavior.

## Native quest-marker resources

**Status:** accepted fact.

`GameSave.SetTaskState` selects native marker categories including:

- base/default -> `icon_quest_mark_small`;
- Stories DLC -> `dlc_quest_mrk`;
- Refugees/Event families -> `quest_marker_violet`;
- Better Save Soul -> `Icon_quest_mark_small_blue`.

Observed native dimensions are 10x10 for base/violet/Souls and 12x12 for Stories, with centered pivots.

Reusable implication: when a mod needs native-looking quest markers, prefer already-loaded game-owned Sprite objects and preserve their native pivots/art rather than redrawing equivalents.

## Task-state mutation and actionability

**Status:** accepted fact.

Normal authored task mutation flows through:

`FlowCanvas.Nodes.Flow_SetTaskState -> GameSave.SetTaskState`

`KnownNPC.TaskState` contains task ID/state, but a visible journal task is not sufficient evidence that an NPC interaction is currently actionable. Authored dialogue/resource/navigation gates can still block the relevant route.

Reusable implication: do not equate task visibility with immediate interaction availability when a mod needs current actionability.

## Phrase state and answer requirements

**Status:** accepted fact.

`GameSave.unlocked_phrases` and `black_list_of_phrases` participate in actual dialogue answer availability.

Direct `Flow_Answer` lock/price requirements are evaluated by the game through SmartRes sufficiency, ultimately using the game-owned `Player.IsEnough(SmartRes)` / equivalent WGO sufficiency path.

Reusable implication: preserve authored SmartRes and delegate sufficiency to the game instead of duplicating item, relation, quality, or resource rules.

## MultipleAnswerData compound semantics

**Status:** accepted fact.

Loaded-game IL for `MultipleAnswerData.FillVisualData` proves:

- `MultipleAnswerData` iterates all child `AnswerData` entries in `datas`;
- each child's `d_lock` is checked through game-owned sufficiency;
- only when that lock is sufficient is the child's `d_price` checked;
- any failed child lock clears the aggregate lock flag;
- any failed child price clears the aggregate price flag;
- the parent answer is pickable only when all child locks and prices are sufficient.

Therefore child requirements are an **AND set**, not alternative OR branches.

Accepted Day Wheel census found 7 relay-backed menu uses across four weekday NPCs, all resolving through the same structural family:

`RelayValueOutput<MultipleAnswerData> -> RelayValueInput<MultipleAnswerData> -> Flow_MultipleAnswer -> Flow_AnswersArray -> child Flow_Answer`

**Evidence provenance:** Day Wheel Quest Markers accepted 1.1.9; exact executable source `73b35a3bffcb440bf644dd03532fbf2cf6ce4b11`.

**Applicability limit:** this establishes the inspected 1.407 `MultipleAnswerData` behavior. Unknown/other graph node families still require evidence.

## Persistent dialogue consumption

**Status:** accepted fact.

Graveyard Keeper authors one-shot dialogue lifetime through phrase blacklisting:

- `FlowCanvas.Nodes.Flow_BlackListPhrase` -> `GameSave.AddPhraseToBlackList(phrase)`;
- `Flow_AddPhraseToBlacklist` does the same when `remove=false`;
- `remove=true` is reversible and must not be treated as permanent one-shot evidence.

Accepted path-local research established a reusable lifecycle rule:

> The persistent interaction owner is the nearest selectable entry on the concrete authored root-to-answer path whose lifetime is consumed by the progressing branch.

Exact self-consumption is the first-precedence case; otherwise the nearest persistently blacklisted selectable ancestor owns the interaction. Display text and the `@` prefix are not semantic ownership signals.

**Evidence provenance:** Day Wheel Quest Markers accepted 1.0.35/1.1.6/1.1.9 research, including complete six-weekday-NPC lifecycle census and player regression evidence.

## Root-to-answer navigation reachability

**Status:** accepted fact.

Checking only a final answer's own gate is insufficient for nested dialogue.

Reusable graph semantics established by the accepted 1.407 audit:

- predicates on one authored root-to-final-answer path combine with AND;
- alternative authored paths combine with OR;
- persisted ancestor phrase state can make an otherwise-valid child unreachable;
- supported ancestor AnswerData price/lock gates must also pass;
- unconditional plain ancestors add no runtime predicate and can compile away;
- ambiguous/unsupported ancestry should fail closed rather than be guessed.

This is a host-graph property useful to any mod that predicts current dialogue reachability.

## Zone-quality mirrors

**Status:** verified host fact.

A FlowCanvas graph can mirror live `WorldZone.GetTotalQuality()` into a player `GameRes` through an authored value chain:

`Flow_SetPlayerParam <- Flow_GetQualityOfZone`

The stored player parameter is therefore not necessarily the canonical owner of the underlying zone quality; when the graph explicitly establishes such a mirror, the live zone value remains the authoritative source.

**Evidence provenance:** accepted Day Wheel intermediate-progress research.

## Known consumers

- `NikichMods/DayWheelQuestMarkers`
- potentially any Graveyard Keeper mod that predicts dialogue availability, task completion routes, one-shot interactions, or weekday HUD semantics.

Do not generalize these findings to unrelated FlowCanvas node families or arbitrary UI surfaces without evidence.
