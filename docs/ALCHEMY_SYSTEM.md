# Graveyard Keeper 1.407 — Alchemy System

Target: **Graveyard Keeper 1.407 (PC)**.

## Status and evidence

**Status:** verified static host-mechanics baseline plus accepted loaded-balance runtime corpus; solver/information-gain analysis remains open.

Primary static reference:

- `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

Accepted 1.407 identities already used by the shared research corpus:

- `resources.assets` SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
- `Assembly-CSharp.dll` SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`.

This document intentionally does not publish the vanilla formula corpus. Aggregate recipe counts and structural statistics are now established from loaded-balance runtime evidence; exact formulas remain unpublished.

## Alchemy mixed-craft ownership

Alchemy combination crafting uses the game's special `MixedCraftGUI` / `CraftType.MixedCraft` path rather than the ordinary fixed-recipe craft-card UI.

`MixedCraftGUI.OpenAsAlchemy(...)` opens the station through the mixed-craft preset and applies `AlchemyItemPickerFilter`.

The native ingredient categories are:

- `AlchemyType.Powder`;
- `AlchemyType.Fluid`;
- `AlchemyType.Essence`;
- `AlchemyType.Universal`.

For a normal positional slot, `AlchemyItemPickerFilter` accepts either:

- the category corresponding to that slot position; or
- `Universal`.

The enum ordering is `Powder=1`, `Fluid=2`, `Essence=3`, so the current GUI filter derives the ordinary expected type from slot index + `Powder`.

**Applicability limit:** this proves category/position filtering in the inspected `MixedCraftGUI` path. Exact station preset arity and the authored recipe population still come from balance/object data and must be measured rather than inferred solely from the enum.

## Exact-mixture lookup

When the player presses Craft, `MixedCraftGUI.OnCraftPressed()` first asks for an exact mixed-craft definition.

`GetCraftDefinitionId(false,...)` constructs an ID from:

1. the current crafter object ID;
2. selected ingredient IDs in positional order;
3. empty trailing components up to the mixed-craft ID shape.

`GetCraftDefinition(...)` then resolves that exact ID against the currently visible mixed-craft definitions loaded by `BaseCraftGUI.CommonOpen(...)`.

Therefore positional ingredient identity is part of the native recipe key; mixed alchemy is not implemented as an unordered ingredient set.

## Failure / goo algorithm

If no exact recipe resolves, `MixedCraftGUI.OnCraftPressed()` asks `GetCraftDefinition(true,...)` for the failure craft.

The verified current algorithm is:

1. collect loaded `MixedCraft` definitions with the same `needs.Count` as the attempted mixture;
2. keep a definition when **at least one selected item equals the definition's ingredient in the same position**;
3. if multiple definitions qualify, choose one using `UnityEngine.Random.Range(0, list.Count)`;
4. locate a matching position between the attempted mixture and that randomly selected qualifying definition;
5. for the other positions of the selected definition, derive goo identities using `ItemDefinition.GetGooFromAlchemyIngridient(...)`;
6. construct and resolve the corresponding authored failure craft.

`GetGooFromAlchemyIngridient(...)` normalizes known alchemical/decomposition prefixes and converts the remaining semantic ingredient identity to its goo identity.

### Consequences

The native failure signal is **informative but stochastic**:

- a failed attempt may be related to a real same-arity recipe that shares at least one correctly positioned ingredient;
- the goo encodes the other position(s) of the randomly selected qualifying recipe;
- when multiple recipes qualify, repeating the same failed attempt is not guaranteed to select the same underlying candidate;
- the algorithm is driven by structural proximity to **some** valid recipe, not by a player-selected desired output.

This distinction is important for any mod that attempts to turn vanilla alchemy into a deduction puzzle. Teaching the goo rule can improve interpretation of failed experiments, but the verified native algorithm by itself does not establish a route from “I need unknown product X” to the relevant recipe branch.

**Open balance-data question:** exact candidate counts, ambiguity distributions and the probability/information value of failure outcomes depend on the authored 1.407 recipe corpus and are not established by static control flow alone.

## Runtime corpus facts — 2026-10-04

Accepted read-only capture from loaded Graveyard Keeper 1.407 balance:

- 509 alchemy mixed definitions: 44 success-classified and 465 auxiliary definitions.
- Two-slot success set: 24 formulas, 18 outputs; four outputs have alternatives, with at most three formulas for one output.
- Three-slot success set: 20 formulas, 17 outputs; three outputs have alternatives, with at most two formulas for one output.
- 43 of the 44 success-classified formulas conform to the standard positional alchemy picker. One three-slot definition violates the normal slot-category rule and remains an explicit exception.
- The ordinary picker-compatible set is therefore 43 formulas for 34 outputs.
- Loaded eligible item definitions: 16 Powder, 9 Fluid, 8 Essence, 19 Universal.
- Ordinary success formulas use 35 unique ingredients: 15 Powder, 8 Fluid, 8 Essence, 4 Universal.
- Those 35 ingredients map to 19 native goo identities; eight goo classes contain one Powder, one Fluid and one Essence participant, while eleven classes are singletons.
- The auxiliary key table is regular: 21 one-goo keys for the two-slot family; the three-slot family has the same 21 one-goo keys plus 420 ordered two-goo keys.

Limits: the capture does not establish progression-specific availability or the visible output multiplicity of auxiliary crafts. The exceptional picker-incompatible definition must be classified separately before treating it as player-solvable ordinary alchemy.

## Recipe discovery / persistence

`GameSave.OnFinishedCraft(CraftDefinition craft)` records newly completed craft IDs in `completed_one_time_crafts` unless the craft ID uses the native ignored failure-placeholder pattern.

For `CraftType.MixedCraft`, completion also derives and unlocks a simplified `mix_<ingredients...>` craft key after removing technology-point entries and quality suffix components where applicable.

`CraftDefinition.IsLocked()` checks `needs_unlock` against `GameSave.unlocked_crafts` and also honors `locked_crafts`.

`BaseCraftGUI.CommonOpen(...)` includes only crafts for which `GameSave.IsCraftVisible(...)` is true.

Separately, while a mixed craft is actively running, `CraftComponent.RefreshComponentBubbleData(...)` shows the unknown result until that exact craft ID is present in `completed_one_time_crafts`; once completed, the real output can be shown.

### Scripted recipe knowledge

`SmartExpression` provides both `UnlockRandomAlchemy` and `UnlockAlchemy`:

- they operate on real `mix:` craft definitions;
- they mark selected recipes in `completed_one_time_crafts`;
- they open a native technology-style discovery dialog for the selected result.

This is a separate authored/scripted discovery channel from player experimentation.

## Study / decomposition metadata

Alchemy decomposition is represented separately from mixed-product recipes through `CraftType.AlchemyDecompose`.

`GameSave.OnFinishedCraft(...)` unlocks the corresponding decomposition craft family after a decomposition result establishes its `AlchemyType`.

`BaseCraftGUI.CommonOpen(... AlchemyDecompose)` additionally gates visible decomposition crafts by `GameSave.IsSurveyComplete(...)`.

The native alchemy information widget, `BubbleWidgetAlchemyItem`, is attached from the standard item tooltip only after the item's Survey craft is complete **and** `ItemDefinition.GetItemDetails()` produced non-null alchemy details.

For the inspected 1.407 implementation, the generated path is narrower than the widget class superficially suggests:

- `ItemDefinition.GetItemDetails()` creates `ItemDetailsAlchemy` only for source items whose own `alch_type == None`;
- it populates `details_type = Decompose` and derives `decomposes` by scanning authored `AlchemyDecompose` crafts that consume that source item;
- if the item itself already has a non-`None` `AlchemyType` (Powder / Fluid / Essence / Universal), `GetItemDetails()` returns before creating an alchemy-details block;
- `BubbleWidgetAlchemyItem` contains a renderer branch for `DetailsType.Slots`, but a full search of the pinned 1.407 decompile finds no writer that assigns `DetailsType.Slots` or populates `ItemDetailsAlchemy.slots`.

Therefore the verified native Study signal is **decomposition-class information for studied source materials**. Do not treat the dormant `Slots` renderer as evidence that populated per-tier slot-compatibility metadata exists in 1.407 without new runtime/balance evidence.


### Player-facing item presentation relevant to alchemy-property research

For ordinary item presentation in the pinned 1.407 source:

- `ItemDefinition.GetItemName(true)` localizes the item ID through `GJL.L(...)`;
- `GetItemDescription(...)` looks up `<item_id>_d`, with a base-ID fallback for colon-suffixed variants, and the standard tooltip places that localized description directly below the item name;
- `ItemDefinition.GetIcon()` returns the explicit `icon` field when populated, otherwise the default key `i_<item_id>`; standard item cells resolve that key through `EasySpritesCollection.GetSprite(...)`;
- the standard `crafted_at` row comes from `GameBalance.GetItemCraftsIn(item_id)`, whose cache includes output-producing crafts only when the craft is not hidden, is not marked `dont_show_in_hint`, and has a resolvable `craft_in` object.

These are real presentation/ownership seams. However, an icon key or internal product/source tag is not by itself proof of what visual or semantic property a human player perceives; perceptual grouping still requires asset/UI inspection or direct runtime observation.


## Runtime property / provenance facts — 2026-10-04

Accepted bounded read-only capture from Graveyard Keeper 1.407 loaded balance (AlchemyCorpusProbe 0.3.0, source \`163f07939701cff70b3a90565e125c0107d245a9\`, CI run \`37207458475\`; installed-runtime log completed \`AR_PROPERTY_BEGIN\` -> \`AR_PROPERTY_DONE\` with no \`AR_PROPERTY_ERROR\`):

- the capture reproduced the accepted ordinary corpus envelope: **43** picker-compatible success formulas, **35** participating ingredients and **34** outputs;
- all **35/35** participating ingredient definitions have an empty localized base-description field in the tested Russian localization; ordinary item-description prose is therefore not a general native ingredient-property channel for this corpus;
- **29/35** participating ingredients have at least one authored \`AlchemyDecompose\` producer from a non-goo source item; these source relationships include plant/crop, anatomy, mineral, animal/insect and jelly/slime materials;
- the six participants without a non-goo \`AlchemyDecompose\` source are the basic substances alcohol, water, blood, oil, ash and salt; other native producer/provenance channels exist for blood, oil, ash and salt, while alcohol/water remain basic-substance cases in this capture;
- only **8/34** ordinary target outputs have non-empty localized base descriptions in the tested Russian localization, and those are consumable-effect descriptions rather than a uniform alchemy-discovery vocabulary.

Applicability / contamination control:
- the runtime had other BepInEx mods installed, so fields that unrelated mods can legitimately rebalance (for example arbitrary craft time/energy or food-recipe inputs) are not promoted here as pristine vanilla values;
- exact-source inspection of the installed Alchemy Research Redux 0.1.8 patch state (\`p1xel8ted/Graveyard-Keeper-Mods@85a88e96cd5c3864f772a03d461a9ac4935df310\`) shows it reads alchemy definitions for UI/recipe memory and does not mutate the relevant balance tables;
- exact historical Decomp Delight 0.1.9 source (\`p1xel8ted/Graveyard-Keeper-Mods@0e6aa4e67dda0c9eeaef8f0effea72253a327eb5\`) reads \`AlchemyDecompose\` records and appends tooltip presentation only; it does not rewrite the decomposition data;
- the accepted reusable facts above therefore rely on item/decomposition identities and bounded presentation data, not on mod-sensitive timing/economy values.

Product-specific grouping of these source relationships into puzzle properties belongs in AlchemyRiddle rather than this shared host document.

## Known current ecosystem overlap

Project-local competitor/product decisions do not belong here, but reusable host-level distinction is:

- decomposition-property presentation and mixed-recipe preview are separable concerns in the host;
- neither should be assumed to solve target-directed discovery of an unknown mixed product.

## Quantitative research still required

A current loaded-`GameBalance` corpus is still required to establish without guesswork:

- progression-specific reachable ingredient populations and player search-space sizes;
- exact role of the single picker-incompatible success-classified definition;
- visible/output multiplicity semantics of auxiliary failure definitions when a consumer needs them;
- candidate-set size distributions;
- expected information gain of vanilla goo outcomes;
- recipe/output classes that violate a common deduction model;
- target-directed answer-blind solver path lengths.

Do not infer those quantities from community recipe lists when exact runtime balance data can establish them.

## Known consumer

- `NikichMods/AlchemyRiddle` — design/research phase for a target-directed deductive alchemy-discovery layer. Product architecture and acceptance remain canonical in the owning repository.
