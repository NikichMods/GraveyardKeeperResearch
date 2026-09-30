# Farming and Fertilizer Mechanics

Target: **Graveyard Keeper 1.407 (PC)**.

## Status and evidence

**Status:** verified static/data result, supported by existing accepted runtime evidence.

Primary evidence:

- pinned host source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`;
- accepted 1.407 balance inputs used by NikichMods research:
  - `resources.assets` SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
  - `Assembly-CSharp.dll` SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`;
- previously captured 1.407 balance/runtime dumps that expose the exact fertilizer crafts, crop growth formulas, ready-crop drop expressions, Farmer-perk consumers, zombie/refugee farm recipes and upgrade definitions;
- accepted PrayerClarity runtime evidence that independently exercised the native `buff_plant` term in the crop-growth formula.

No new runtime harness is required for the facts below: the relevant writers, consumers, formulas, reset path and automatic-farm definitions are directly visible in the current 1.407 data/source.

Community/wiki material may corroborate user-facing behavior, but is not the canonical mechanics source.

## Manual farming: canonical state

Manual fertilizer behavior is represented by two independent WGO parameters on the farming plot:

- `grow_qual` — crop/yield quality axis;
- `grow_time` — growth-time axis.

Fertilizer application is implemented as an instantaneous `CraftDefinition` on the empty plot. `CraftComponent.ProcessFinishedCraft()` applies `CraftDefinition.output_set_res_wgo`, and the underlying parameter setter writes each resource key independently.

Current manual-plot families include:

- ordinary empty garden bed: `garden_empty`;
- garden bed with sticks: `garden_empty_stick`;
- vineyard vine trellis: `vineyard_grapes_stick`.

The relevant fertilizer definitions write the same state model across these manual plot families.

### Fertilizer tier mapping

The game's internal numeric tiers include peat as tier 1:

| Player-facing input | Internal state written |
| --- | --- |
| Peat | `grow_qual=1`, `grow_time=1` |
| Quality fertilizer I (`sack_star_silver`) | `grow_qual=2` |
| Quality fertilizer II (`sack_star_gold`) | `grow_qual=3` |
| Boost fertilizer I (`sack_clock_silver`) | `grow_time=2` |
| Boost fertilizer II (`sack_clock_gold`) | `grow_time=3` |

The vineyard peat craft is named `vineyard_stick_fertilize_time_1`, but its actual `output_set_res_wgo` writes **both** `grow_qual=1` and `grow_time=1`. Do not infer semantics from the craft ID alone.

## Combination and overwrite semantics

Quality and Boost are independent axes, so a Quality fertilizer and a Boost fertilizer can coexist on the same plot.

They do **not** add together within one axis. Applying another fertilizer that writes the same key replaces the previous value because the native setter performs a direct keyed set; the fertilizer crafts contain no monotonic `max()` guard.

Consequences in the current 1.407 data:

- Quality II followed by Quality I changes `grow_qual` from 3 to 2;
- Boost II followed by Boost I changes `grow_time` from 3 to 2;
- peat writes both axes to 1, so applying peat **after** a stronger Quality or Boost fertilizer downgrades the corresponding axis;
- peat may be applied first and then one or both specialized fertilizers may overwrite only their own axis;
- reapplying the same or a weaker fertilizer can therefore waste the item.

This is a data/lifecycle fact, not merely a UI recommendation.

## Effect lifetime

Manual fertilizer is **one crop cycle**, not a permanent plot upgrade.

The ready-crop ObjectDefinitions reset the growth state when the crop is harvested/cleared:

- `growing=0`;
- `grow_qual=0`;
- `grow_time=0`.

The reset is present on the ordinary ready-crop family, including garden vegetables and vineyard crops. The next planting therefore needs fresh fertilizer if the player wants the effect again.

## Boost / growth-time mechanics

The manual crop growth crafts use the form:

```
base_time * (1 - 0.2 * WGOpar("grow_time") - 0.2 * WGOpar("buff_plant"))
```

Representative current base expressions:

- most garden crops: `1440 * (...)`;
- grapes: `2160 * (...)`.

Therefore, isolating fertilizer and treating `buff_plant=0`:

| Growth state | Remaining duration | Reduction from base |
| --- | ---: | ---: |
| none, `grow_time=0` | 100% | 0% |
| peat, `grow_time=1` | 80% | 20% |
| Boost I, `grow_time=2` | 60% | 40% |
| Boost II, `grow_time=3` | 40% | 60% |

Prefer the wording **“reduces growth time by X%”**. Saying “X% faster” is not mathematically equivalent.

`grow_time` is not part of the inspected ready-crop drop formulas. Boost fertilizer therefore changes maturation time, not crop/seed yield.

### Separate `buff_plant` modifier

`buff_plant` is a separate native term in the same growth-time formula. Accepted PrayerClarity runtime evidence has exercised it independently of `grow_time`.

Do not conflate the prayer/buff effect with fertilizer ownership merely because they are summed in one duration expression.

## Quality / yield mechanics

The exact ready-crop drops are authored as expressions that read `grow_qual`, the planted `seed_qual` where applicable, and in many cases the Farmer perk `p_farmer`.

The tables below isolate fertilizer by assuming `p_farmer=0`.

### Basic crops without star quality

For the common basic-crop family such as carrot, wheat, beet and cabbage:

- crop amount: `6 + 2q` through `8 + 2q`;
- returned seeds: `2 + q` through `3 + q`;
- crop waste: `2 + q` through `5 + q`;

where `q = grow_qual`.

That yields:

| Quality state | Crop | Seeds returned | Crop waste |
| --- | ---: | ---: | ---: |
| none, `q=0` | 6–8 | 2–3 | 2–5 |
| peat, `q=1` | 8–10 | 3–4 | 3–6 |
| Quality I, `q=2` | 10–12 | 4–5 | 4–7 |
| Quality II, `q=3` | 12–14 | 5–6 | 5–8 |

Manual planting consumes four seeds for these plots. Without Farmer, Quality I is therefore the first fertilizer tier in this common basic-crop family whose authored return cannot fall below the four-seed replacement cost.

Hemp/cannabis uses different base ranges, so the table above must not be generalized as a universal numeric table for every non-star crop. The semantic result remains the same: `grow_qual` raises yield.

### Crops with bronze / silver / gold quality

Planting records the source seed quality in `seed_qual`:

- bronze = 1;
- silver = 2;
- gold = 3.

For the inspected quality-crop family (onion, pumpkin, lentils, grapes and hops), `grow_qual` affects both same-tier yield and next-tier output.

Ignoring Farmer:

#### Bronze source seed

| Fertilizer | Bronze crop | Bronze seeds | Additional silver crop | Additional silver seed |
| --- | ---: | ---: | ---: | ---: |
| none | 3–5 | 2–4 | 0 | 0 |
| peat | 4–6 | 3–5 | 0 | 0 |
| Quality I | 4–6 | 3–5 | 1 | 1 |
| Quality II | 5–7 | 4–6 | 2 | 2 |

#### Silver source seed

The same pattern applies one tier higher:

- Quality I guarantees 1 additional gold crop and 1 gold seed;
- Quality II guarantees 2 additional gold crops and 2 gold seeds.

#### Gold source seed

There is no higher quality tier. The quality-fertilizer benefit remains at gold quality:

| Fertilizer | Gold crop | Gold seeds |
| --- | ---: | ---: |
| none | 3–5 | 2–4 |
| peat | 4–6 | 3–5 |
| Quality I | 5–7 | 4–6 |
| Quality II | 7–9 | 6–8 |

### Semantic conclusions

For current 1.407 manual crops:

- peat increases yield and gives the tier-1 growth-time reduction, but **does not produce a higher seed/crop quality tier**;
- Quality I guarantees **1** next-tier crop and **1** next-tier seed when a next tier exists;
- Quality II guarantees **2** next-tier crops and **2** next-tier seeds when a next tier exists;
- Quality fertilizer therefore affects **produce as well as seeds**;
- gold-quality planting has no lower-quality output path in the inspected formulas, so fertilizer does not cause gold seeds/crops to “degrade”;
- when no higher tier exists, the quality benefit becomes additional same-tier gold output.

These are stronger and more precise statements than “Quality fertilizer improves seed quality.”

## Farmer perk interaction

The `Gardening` technology grants `p_farmer`.

Ready-crop expressions consume `p_farmer` alongside `grow_qual`; depending on crop/output family it increases crop multipliers and/or seed/waste ranges. Therefore exact harvest counts observed by a player with Farmer can exceed the fertilizer-only tables above.

Product/UI text that only needs to explain fertilizer semantics should normally avoid publishing a single universal harvest-count table unless it deliberately accounts for Farmer and crop-family differences.

## Automatic farms are a different fertilizer mechanic

Quality fertilizer also appears in permanent upgrade recipes for automatic farms. These uses must not be described as though they were manual `grow_qual` application.

### Zombie farm / zombie vineyard

Current upgrade definitions:

- base -> silver tier: consume **12 Quality fertilizer I**, set `lvl=1`;
- silver -> gold tier: consume **12 Quality fertilizer II**, set `lvl=2`.

The object changes to the corresponding higher-tier desk and the upgrade persists.

The tier controls which seed-quality recipes the automatic station offers:

- tier 0: base/bronze recipes;
- tier 1: adds silver;
- tier 2: adds gold.

Automatic recipes consume and return the **same seed quality**. The upgrade does not turn bronze seeds into silver or silver into gold. Higher-quality seed stock must come from elsewhere, such as manual farming/trading, before the corresponding automatic recipe can run.

Their time formulas use `lvl`, not manual-plot `grow_time`:

- zombie garden: `300 * (1 - 0.2*lvl - 0.2*buff_plant)`;
- zombie vineyard: `200 * (1 - 0.2*lvl - 0.2*buff_plant)`.

Thus higher permanent tiers also reduce the automatic craft duration.

### Refugee camp garden beds

Game of Crone refugee garden beds use the same permanent quality-upgrade pattern:

- `refugee_garden_desk_upgr_to_silver`: 12 Quality fertilizer I, tier 1 -> tier 2 / `lvl=1`;
- `refugee_garden_desk_upgr_to_gold`: 12 Quality fertilizer II, tier 2 -> tier 3 / `lvl=2`.

Their production recipes likewise gate bronze/silver/gold seed-quality recipes by bed tier and use a `lvl`-based time formula.

Boost fertilizer is not part of these permanent automatic-farm upgrade definitions.

## Applicability limits

- The numeric manual-harvest tables above intentionally isolate fertilizer from Farmer and other modifiers.
- Basic and quality crop families have different authored output expressions; do not collapse every crop into one exact yield formula.
- The verified manual plot families are ordinary beds, beds with sticks and vineyard trellises. Automatic zombie/refugee farms are separate systems.
- Do not infer fertilizer semantics from item names, icons or craft-ID spelling when exact `output_set_res_wgo` / consumer expressions are available.
- These facts target Graveyard Keeper 1.407 and should not be silently generalized to future game versions.

## Known consumer

- `NikichMods/DetailedTechnologyTooltips` — candidate product research for whether the Farming & Nature Technology tree should briefly explain fertilizer semantics. Product placement, wording, localization and inclusion remain separate decisions in the owning repository.
