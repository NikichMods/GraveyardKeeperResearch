# Late-game economy — exploratory design candidates

Status: **idea bank**, captured 2026-10-22 at the user's request. No production owner/mod, implementation, balance, unlock conditions or runtime acceptance is selected. This is not a second production design contract. If a production project adopts a candidate, its requirements and acceptance belong there.

Evidence owners: [resource saturation](ECONOMY_PROGRESSION_RESEARCH.md), [graveyard host facts](GRAVEYARD_RUNTIME.md).

## Scope and retained direction

- Improve the last third of an ordinary playthrough with limited new content, using existing systems/assets where feasible.
- Optional spending should enable desired play or an appreciable result; ingredient gathering and mandatory chores are not themselves proof of a meaningful choice.
- The user found an alternative to anatomy promising: expensive grave-specific improvement could let players use other developed economies instead of repeatedly preparing better bodies. **Retain for future exploration**, not implement as an adopted mechanic.
- The earlier proposal of exactly three special graves has no host/economic justification. Do not preserve that arbitrary limit as a requirement.
- The user proposed prayer stations as spatial centers for a graveyard subsystem: surrounding occupied graves and their quality may enable functions; stations might instead or additionally improve surrounding graves.
- The user proposed escalating investment (illustrative 10, then 20 faith, etc.) for skull conversion/improvement, potentially using money or other surplus resources. Price series, resource and effect are unselected.
- The user proposed sarcophagi as tangible rewards for substantial, resource-consuming accomplishments. Only the marble candidate currently has the fuller static asset chain; runtime readiness is unproven.

## Candidate structure, not an adopted rule set

Potential loop: arrange a prayer station and surrounding graves -> obtain a usable district -> choose where to invest surplus -> improve selected graves or district capabilities -> obtain visible progress and finite reward opportunities.

This has at least two possible decisions worth investigating:

1. **Breadth versus depth:** establish another useful district versus pay the rising marginal cost of improving the current one.
2. **Anatomy versus other economies:** improve/replace bodies through existing methods versus spend free faith/money/materials on a bounded grave-specific alternative.

Optional additional services (extra sermons/body deliveries) are candidates, not automatically the required purpose of every station. A station-to-extra-sermon connection is easier to motivate than a magical grave-to-donkey bonus. Delivery throughput would need an explicit reason such as demonstrated mortuary capacity and paid arrangements; spatial grave quality alone does not prove that connection.

## Decisions that must remain open

- Activation metric: number of occupied graves, body skull potential, decoration quality, native grave score, or a combination. These give different incentives. Requiring exceptionally good anatomy to unlock its alternative could defeat the goal.
- Effect scope: station aura, one chosen grave per payment, permanent body mutation, or grave-only compensation. Prefer comparing these before technical implementation; they are not equivalent.
- If the mechanism only converts red skulls, it stops when red skulls run out. A finite cemetery and decoration limits also bound useful improvement. Exponentially rising prices alone do not establish an infinite useful sink.
- Price ownership: per grave, per station or global. Clarify whether new construction resets a cheap first tier. Breadth may intentionally be cheaper; demolition/rebuilding should not accidentally reset paid progression.
- Coverage: overlapping graves must not multiply the same benefit. The user's first-built-wins suggestion is one option, not adopted. Nearest eligible station or explicit membership may be easier to explain; ties and reassignment must be deterministic and visible.
- Moving/demolishing a station or exhuming/replacing a body must have clear consequences for coverage, paid investment and the effect's owner. No persistent investment destruction or refund policy is assumed.
- Prevent unintended self-qualification: if a station activates based on the quality it itself grants, paid improvement may unlock further improvement automatically. Decide deliberately whether thresholds use baseline quality excluding these effects.
- Balance the **area** of an aura: one paid tier applied to ten graves is different from one paid change to one grave.
- Show real improvement before payment. Native skull/rating limits can make an apparently helpful conversion produce no visible rating gain.
- A physical-body edit can export improvements through exhumation/organs/zombie use; a cemetery-only alternative could preserve the role of anatomy elsewhere. The technical route and desired semantics are unselected.
- Rewards should reflect completed work, not merely the number of paid button presses. Determine whether sarcophagus reward unlocks crafting, gives a first item, or does something else; no milestone or exclusivity adopted.

## Related reusable objects

Prayer stations are already +2/+4 quality decorations and require books in construction; calling them literally useless is inaccurate, though their active gameplay role could be expanded. Stone/wood progression can be investigated before inventing a new building.

Columbaria, candles/incense, paths, flowerbeds and lanterns are possible subjects for narrow follow-up. Existing use is not evidence they need extra functions. `candle_6` and `skull_candle` remain incomplete asset candidates, not promised easy rewards. Do not start another bulk asset audit without a concrete role/question.

## Prior market check relevant to novelty

- [Pray the Day Away](https://www.nexusmods.com/graveyardkeeper/mods/72?tab=description) advertises extra sermons with consumption/degradation of prayers as well as a free cheat mode. Therefore paid sermons alone are not a novel differentiator.
- [The author's mod catalog](https://github.com/p1xel8ted/Graveyard-Keeper-Mods) also advertises repeat corpse delivery, fast travel and fishing convenience. Descriptions were read; installed compatibility was not tested.
- No representative frequency of complaints about bodies/faith/late waiting has been measured. Player testimony cannot by itself establish target-market size.

Next useful comparison: work one small spatial example with an overlapping grave, a paid improvement and a moved station. This would distinguish intuitive district ownership from hidden rules before any runtime implementation.
