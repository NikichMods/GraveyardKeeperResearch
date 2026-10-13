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


## Runtime presentation / provenance facts — 2026-10-04

Accepted read-only loaded-balance capture from the AlchemyRiddle property/provenance probe adds the following reusable 1.407 facts for the **35 ingredients that participate in ordinary picker-compatible successful mixed-alchemy formulas**:

- all 35 have player-facing names and icon keys, but **none has non-empty base item-description text** in the captured Russian localization;
- the regular semantic reagent population contains eight Powder / Fluid / Essence triplets whose names correspond to the same native goo/material family across forms; the other eleven participants are singleton/special material identities;
- **29 / 35** have at least one authored non-goo `AlchemyDecompose` source material in addition to any same-family goo source;
- source materials span concrete world categories such as crops/plants, creature/insect materials, anatomical remains and minerals;
- some special ingredients instead expose provenance through ordinary producer paths such as autopsy/press/pyre-style production, so decomposition sources alone are not a complete acquisition graph.

For the 34 ordinary target outputs, only **8 / 34** have non-empty base description text in this capture; the populated descriptions are concentrated in consumable effect-bearing elixirs.

Applicability limits:
- these counts describe the ordinary success-corpus participants, not every globally eligible alchemy-picker item definition;
- broad labels such as “plant-derived” or “mineral-derived” are **not native fields** and remain product-layer interpretations of authored source relationships;
- icon keys do not by themselves establish human-perceived color/shape properties.

## Known current ecosystem overlap

Project-local competitor/product decisions do not belong here, but reusable host-level distinction is:

- decomposition-property presentation and mixed-recipe preview are separable concerns in the host;
- neither should be assumed to solve target-directed discovery of an unknown mixed product.

## Ingredient producers beyond decomposition — static 1.407

### Source-material quality audit

One-time raw-data audit on the same resources.assets SHA-256 215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca: all 123 visible manual producer records have needs_quality=0 and no input ID ending in an exact numeric quality suffix. All 508 non-hidden authored records referencing either manual alchemy-table ID likewise have no needs_quality or exact-quality input suffix; this broad set includes failure records and is not a count of known success formulas. Including the nonstandard memory-potion recipe gives 44 success formulas/36 distinct inputs; the same manual producer set remains 123, so this does not establish full acquisition coverage of the extra input.

The producer inputs onion_crop/hop_crop/hamp_crop are unsuffixed family IDs. Native item definitions contain :1/:2/:3 members for each; onion/hops members have quality 1/2/3, hemp members have quality 0 despite the suffix variants. Pinned Item.InitMultiqualityItems resolves a missing exact definition by collecting every matching id + ':' prefix. Item.IsEnoughItems accepts the family's member list when no concrete variant is selected. These are actual crop families, not generic crop waste. Material naming should resolve native localized family members (onion/hops/hemp) without asserting a specific quality. This bounded static finding does not promise that future host/modded producer data never adds quality requirements; no runtime quality-policy implementation is implied.

Bounded static inspection for ResearchableAlchemyRecipes 0.1.19 rechecks the accepted resources.assets / Assembly-CSharp identities and selects the same 43 ordinary manual-table formulas, 34 outputs and 35 inputs by native slot types (Powder/Fluid/Essence or Universal). Filtering visible authored None/AlchemyDecompose producers with real input/equipment, excluding zombie mirrors, yields 123 producer records covering all 35 ingredients. This coverage includes goo conversions and must not be mistaken for complete world acquisition/provenance.

Additional native channels are important for the six basic substances discussed above:

- ItemDefinition.can_be_used/drop_on_use and Item.UseItem supply inventory-use channels for water/alcohol; the inspected simple-use definitions have no on_use_script. Source material acquisition itself is a separate question.
- Native ex:-prefixed body extraction is identified by CraftDefinition.IsBodyPartExtractionCraft. CraftComponent.CraftReally captures the selected body part and End processes authored output; these records have no ordinary needs array. A fixed-input-recipe filter therefore loses blood extraction.
- corp_burning/crematorium_corp_burning are hidden automatic output records, not manually selectable recipes. mf_pyre_put_body changes the object to mf_pyre_corp; its ObjectDefinition.craft_after_hp_0 starts corp_burning. The crematorium's object/script transitions lead to mf_crematorium_burning and its Flow_StartCraftByName. Construction records own their equipment access; ordinary IsCraftVisible on the hidden burning recipe would be the wrong availability predicate for guidance.
- Authored techs_data.crafts can identify named unlock prerequisites for some extraction/construction records. No match is not proof of unrestricted access: dialogue/script grants and parent/context gates remain distinct owners.
- ResModificator.ProcessItemsListBeforeDrop evaluates min/max counts and can use RNG or p_scientist-dependent expressions. A quantity UI should not evaluate that drop path merely to describe acquisition. Two equipment tiers can share input and product while having different yield expressions.

Source pointer: Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9, Item, ItemDefinition, CraftDefinition, CraftComponent, ResModificator; matched local native metadata. Private graph serialized-text SHA-256: mf_pyre_put_body e4cd68a499d2a7d714796925673592382de388945e60358dee0368c656ebbcb4; mf_crematorium 46f3257fcea9c9f21de47e6f5e646a6786517f0410ed74fdaa1196baca0c56ab; mf_crematorium_burning f25c6b6234aacc2df9d96178ba75b2a57a621c11ce26c6de5d7aba4ef63196b3. Extracted assets/graphs/source lists stay private. This is static/read-only dependency evidence; current live guidance acceptance belongs to the consuming project's TEST_LOG.md. Shops, quest rewards, arbitrary world drops and every DLC/mod-altered progression case are not enumerated by this source audit.

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

## Residual paint awareness channels — 2026-10-12

**Status:** verified static data/owner classification, not a first-exposure chronology or complete current-save availability proof.

Direct balance inspection on the accepted 1.407 input hashes finds 26 `craft_obj_data` records consuming one or more of the seven formerly blueprint-only ordinary outputs: `yellow_paint`, `green_paint`, `brown_paint`, `red_paint`, `d_green_paint`, `d_violet_paint`, `violet_paint`. Every record belongs to `keeper_room_builddesk`, has `needs_unlock=true`, `hidden=false`, and an empty condition expression. No `techs_data.crafts` entry directly names these exact 26 records.

Pinned source `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, `GameSave` save-migration path, contains explicit unlocks for the eight colored carpet and eight bed variants (including blue), plus one picture, in the `num <= 1314` migration when `The Beginning Of Alchemy` is already unlocked. The `num <= 1316` migration likewise unlocks colored wall variants. This establishes an actual persistent `unlocked_crafts` owner and an old-save repair channel, not a technology replay requirement.

The normal unlock writer is now also statically established. The Beginning Of Alchemy has `hidden=true`, `invisible=false`, no populated parent and `flowscript=tech_alchemy_begins`. `GameSave.UnlockTech` records the ID in `unlocked_techs` and calls `TechDefinition.ApplyTech`, which runs that graph through GS. Direct resources.assets inspection finds the named graph at path ID 150987, serialized graph text SHA-256 `d2d05199954752514686b1619a6daccd62798d78b8ae3a1e9bbc22af81e6dea6`. Its 34 nodes are connected from StartEvent and contain only StartEvent, Flow_UnlockCraft, Flow_RevealCrafth and Flow_EndScript. The 31 unlock nodes include 22 of the 26 gap-consumer blueprints and cover all seven paint outputs. The four other consumers are picture variants 4–7. All seven paint colors already have carpet unlocks before the graph's picture-reveal node. `Flow_UnlockCraft` calls the native `UnlockCraft` writer. Therefore these channels are introduced by the alchemy technology path, not proved to be initial-game unlocks simply because the room builder exists early.

For a mod reconstructing current source eligibility, inspect actual native craft unlock/visibility and builder availability. `GameSave.IsCraftVisible` checks hidden/locked/one-time completion state; it alone does not prove that the relevant builder is currently accessible. Do not use the migration condition as the general rule for every current save, or invoke migration to manufacture eligibility. Exact builder reachability, other possible source channels and representative loaded-save evidence remain open.

The technology's unlocked state is directly readable from the serialized `GameSave.unlocked_techs` list. Revealed/visible states are separate; `Flow_UnlockTech` in its non-silent path commits the actual unlock through the forced TechUnlockDialogGUI OK callback. Consumers gating on completed introduction should read the resulting unlock state, including on old saves, rather than rely only on future events or create a duplicate perk flag.

The former eighth structural/vendor gap, Spices, already has an accepted Merchant curse task/hand-in source in `AlchemyRiddle/docs/PROGRESSION_RESEARCH.md`; this closes channel existence only. Its currently available root-to-dialogue path must still be established under the shared dialogue reachability rules.

No formula inputs are included. The 26/34 prior figure remains historical coverage of measured channels; it must not be relabeled a verified 34/34 save-availability result.

## Known consumer

- `NikichMods/AlchemyRiddle` — design/research phase for a target-directed deductive alchemy-discovery layer. Product architecture and acceptance remain canonical in the owning repository.


## Mixed-output / ingredient disjointness — 2026-10-11

**Status:** directly verified static balance result on the same 1.407 resources.assets and Assembly-CSharp.dll SHA-256 identities listed above. Read-only local inspection; no game/save mutation.

Question: can revealing acquisition routes for an ingredient of one ordinary mixed-alchemy formula disclose another mixed formula, especially a two-slot formula used inside a three-slot formula?

Method: deserialize game_data with UnityPy 1.23.0 + TypeTreeGeneratorAPI 0.0.6, reproduce the accepted AlchemyCorpusProbe success classification (`mix:mf_alchemy` family, excluding `goo` and `:_:` auxiliary keys), exclude technology-point entries from needs/output, and apply the native positional picker rule. AlchemyType enum constants were read from the matching assembly metadata (Powder=1, Fluid=2, Essence=3, Universal=9). All non-technology output entries were included in the intersection, not just the primary output.

Reproduced baseline: 2,101 craft_data records, 509 alchemy mixed definitions, 44 success-classified formulas, 43 picker-compatible formulas (24 two-slot and 19 three-slot), 34 ordinary outputs and 35 ordinary ingredients.

Results:
- ordinary success-output IDs intersect ordinary ingredient IDs: **0**;
- two-slot ordinary outputs intersect three-slot ordinary ingredients: **0**;
- three-slot ordinary outputs intersect two-slot ordinary ingredients: **0**;
- including the single picker-incompatible success-classified definition: output/ingredient intersection still **0**;
- success-classified outputs intersect inputs of all 509 mixed definitions, including auxiliary definitions: **0**.

Therefore this fixed vanilla corpus contains no nested mixed-product dependency that would disclose another mixed formula when tracing an ordinary reagent's acquisition. This does not establish that reagents lack other non-mixed crafting or gameplay uses. Mod-altered balance and broader host versions are outside this static result. No formula rows or concrete product/ingredient names are published.

### Native known-formula list, disclosure popup and save boundary — static 2026-10-12

Fact from the pinned 1.407 source above, in CraftGUI, SmartExpression, GameSave, TechUnlockDialogGUI and TechDefinition:

- CraftGUI.OpenAsAlchemy reads completed_one_time_crafts, resolves exact CraftDefinitions, includes only definitions whose craft_in contains the current station, then opens a MixedCraft inventory. CommonOpen still applies native visibility/type filtering. Neutral registered craft markers with empty craft_in do not enter this list.
- Specific authored UnlockAlchemy adds the exact recipe ID and uses a temporary TechDefinition with a forced pseudotech dialog. It does not invoke OnFinishedCraft or simplified automation UnlockCraft; random disclosure has distinct achievement effects.
- TechUnlockDialogGUI.Open(forced_unlock=true, pseudotech=true) displays unlocks without buying/revealing/unlocking a technology on OK. Native visible-unlock capacity is three. This establishes the static presentation seam, not installed grouped-reward usability.
- GameSave.ToBinary captures scene state then calls SmartSerializer.Serialize<GameSave>; FromBinary deserializes the save and PrepareAfterLoad prepares loaded state. completed_one_time_crafts is a public list without NonSerialized. These paths establish a serialization candidate; full process-restart knowledge retention must be observed in the actual runtime.

Consumer: ResearchableAlchemyRecipes research-only 0.1.7. Its synthetic setup and cleanup markers are diagnostic state, not a second production formula ledger. No native recipe ingredients are published. Actual reward/popup/list/persistence acceptance remains in the owning project.

### Installed native knowledge retention — accepted bounded 0.1.7 result

ResearchableAlchemyRecipes Study Table Work Probe 0.1.7 (source b9721dd1ad169f7561a6ac31e47ecd0477d631c3, DLL SHA-256 439ab0329b37298d4efe932dee4a087fab5a991ce8bc0f1766a3ce1993d1bdfe) now has two-launch installed evidence. Actual sample-free research completion adds missing native exact formula knowledge; native tier-I OpenAsAlchemy renders two cards; saving and fully restarting retains both IDs and cards. A partially known synthetic input adds only one missing formula. User screenshots/report accept the bounded functionality. Owning TEST_LOG.md records returned log hashes and restrictions.

Applicability: one ordinary two-variant product, the verified 1.407 host and current mod set. Cleanup restores the originally unknown baseline, with cleaned state passed through ToBinary. Nonzero baseline restoration is automated state evidence only. This does not prove actual manufacturing, all products/stations, source availability, final tooltip guidance or general compatibility. Native pseudotech presentation has generic technology wording and a missing TechDefinition lookup warning; the formula-knowledge path is confirmed, while presentation refinement remains separate.

### Room-source filter and Spices native disclosure — static 2026-10-12

Pinned 1.407 source remains the same. MainGame.OpenBuildObjectGUI includes craft_obj_data records whose builder_ids contain the current desk and whose GameSave.IsCraftVisible passes, then calls CraftGUI.OpenAsBuild. There is an explicit tutorial early return (in_tutorial=1 and tut_shown_tut_1=0). Builder interaction and affordability are separate: displayed resource needs do not require already owning those materials. Keeper-room attached graph handles first-use speech, not blueprint filtering; actual desk presence/reachability must be established per save. GameSave's <=1314 migration explicitly spawns this desk, a historical repair writer rather than a universal availability predicate.

Direct verified resources.assets graph inspection: npc_merchant path 150541, serialized graph SHA-256 fb8815cf25d381e3e3a7bc112cd93e45aea481a7c90338e8d3aea854dd6f88b2; npc_witch path 150562, 3b1254898077562cffc9adba7cb339144dd6d5842ca2c522d522c72de8610108; keeper_room_builddesk path 150502, b47867e06a6ce983b5549632d6063d465c5b516f7ea95880d69743e46a6785cf. Raw graphs/formulas remain private.

Important refinement of the Spices anchor: merchant_curse Visible unlocks the Clotho-directed branch but is not itself proof of a displayed Spices need. Witch node 140 unlocks the Merchant cure answer and node 142 immediately reveals the one ordinary Spices formula through Flow_RevealCrafth (silent=false, not_add_to_one_time=false). Thus this introduction normally leaves the sole formula natively known; a recipe-learning consumer must suppress a fully known topic. Prior findings about absence of ordinary mixed targets in Flow_UnlockTech/Flow_UnlockCraft do not cover Flow_RevealCrafth and must not be read as absence of scripted disclosure.

Merchant cure answer is root menu #7 with one Spices as price and no populated separate lock; successful hand-in blacklists that answer and completes merchant_curse. MultiAnswerGUI.ShowAnswers filters unlocked/blacklisted phrase state, then renders even can_be_picked=false answers. A shown need and affordable hand-in are distinct properties; product ownership cannot become a requirement merely to learn that a source mentions it. KnownNPCList.GetNPC is a noncreating read; GetOrCreateNPC changes meeting/achievement state.

Consumer: ResearchableAlchemyRecipes Source Eligibility Probe 0.1.0, a bounded read-only loaded-input/final-UI observer. Live builder availability and catalog-wide source eligibility remain pending; no 34/34 save coverage is claimed.

### Loaded room source reconstruction — bounded runtime 0.1.1

ResearchableAlchemyRecipes Source Eligibility Probe 0.1.1 (source 25e3f834d04b1c1b07b218496c23328766298e1f, DLL SHA-256 067fb2a69daf0328abb351dca6cbe20c37176052b302dba03c2322048e8b0d4d) returned an exact installed one-shot manual snapshot after successful load. Native visibility counts agree with actual shown keeper-room OpenAsBuild cards for all 26 paint-consuming definitions: per-product 4,3,3,6,3,3,4 over the seven previously enumerated paint outputs. Shown native builder supplies the exercised positive access boundary; technology is unlocked and no tutorial/removed/disabled block is present. Existing formula knowledge leaves five unknown paint products; two are known. Spices sole formula is known, Merchant curse completed, cure phrase unlocked and blacklisted.

Therefore this mature save supports recovery of current positive blueprint sources from native builder/visibility state without a new encounter/unlock event or duplicate source history. Limits: one save/current mod set, positive room-source state only; no early/locked/absent-source checks, active Merchant dialogue, catalog-wide eligibility or playable topic registration. Known Spices is consistent with its authored reveal but does not prove the exact historical learning route in this save. Exact returned log hash and the separately observed Study/MaxButtonsRedux gamepad null-reference stacks belong to the consuming project's TEST_LOG.md; no clean-log compatibility claim is made.
