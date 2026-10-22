# Late-game economy — exploratory design candidates

Status: **idea bank**, captured 2026-10-22 at the user's request. No production owner/mod, implementation, balance, unlock conditions or runtime acceptance is selected. This is not a second production design contract. If a production project adopts a candidate, its requirements and acceptance belong there.

Evidence owners: [resource saturation](ECONOMY_PROGRESSION_RESEARCH.md), [graveyard host facts](GRAVEYARD_RUNTIME.md), [player needs and aesthetic motivation](GRAVEYARD_PLAYER_NEEDS.md).

## Scope and retained direction

- Improve the last third of an ordinary playthrough with limited new content, using existing systems/assets where feasible.
- Optional spending should enable desired play or an appreciable result; ingredient gathering and mandatory chores are not themselves proof of a meaningful choice.
- The user found an alternative to anatomy promising: expensive grave-specific improvement could let players use other developed economies instead of repeatedly preparing better bodies. **Retain for future exploration**, not implement as an adopted mechanic.
- The earlier proposal of exactly three special graves has no host/economic justification. Do not preserve that arbitrary limit as a requirement.
- **Latest clarification:** the user means work on **one selected grave**. Surrounding-grave quality as an activation gate is removed from the current candidate. A spatial district minigame is superseded, not an implementation requirement.
- The prayer station is the proposed existing interaction point for three actions: inspect buried organ composition; convert one red skull to white; add effective decoration/wreath quality without replacing the physical decoration. The user wants improvements to be **expensive**, primarily consuming substantial surplus faith. Exact prices, action scope and implementation are unselected.
- The user proposed escalating investment (illustrative 10, then 20 faith, etc.). Whether actions share counters and how prices follow the selected grave remain open. These numbers are not agreed balance.
- Organ inspection is distinct from native total-skull display. Payment/free access and unlocks are unselected; exact community demand was not established by the bounded search. Respect or deliberately redesign [Cultist's visibility gate](PERK_MECHANICS.md#cultist) before revealing organ contributions.
- Light/sparks on improved graves are possible feedback, not a mandatory visual treatment. The user also wants to serve players building a visually preferred cemetery rather than only maximum-score layouts.
- The user proposed sarcophagi as tangible rewards for substantial, resource-consuming accomplishments. Only the marble candidate currently has the fuller static asset chain; runtime readiness is unproven.

## Candidate structure, not an adopted rule set

Current potential loop: use a prayer station -> inspect/select a grave -> choose which limitation to address -> pay faith directly -> receive a visible/effective improvement. No preliminary event or surrounding-quality activation task is required.

This has at least two possible decisions worth investigating:

1. **Breadth versus depth:** improve another grave at a lower tier versus pay the rising marginal cost of improving the current one.
2. **Anatomy versus other economies:** improve/replace bodies through existing methods versus spend free faith/money/materials on a bounded grave-specific alternative.

Optional additional services (extra sermons/body deliveries) are candidates, not automatically the required purpose of every station. A station-to-extra-sermon connection is easier to motivate than a magical grave-to-donkey bonus. Delivery throughput would need an explicit reason such as demonstrated mortuary capacity and paid arrangements; spatial grave quality alone does not prove that connection.

## Decisions that must remain open

- The surrounding-quality activation metric is removed from the current candidate. No district-quality bootstrap is required.
- Effect ownership remains open: permanent body mutation or grave-only compensation. One selected grave per interaction is the current scope; do not silently replace it with an aura.
- If the mechanism only converts red skulls, it stops when red skulls run out. A finite cemetery and decoration limits also bound useful improvement. Exponentially rising prices alone do not establish an infinite useful sink.
- Price ownership: per grave, per station or global. Clarify whether new construction resets a cheap first tier. Breadth may intentionally be cheaper; demolition/rebuilding should not accidentally reset paid progression.
- Station reach and grave selection are unselected. The former first-built-wins/overlapping-aura discussion is not an adopted dependency of single-grave work.
- Moving/demolishing a station or exhuming/replacing a body must have clear consequences for coverage, paid investment and the effect's owner. No persistent investment destruction or refund policy is assumed.
- A proposed simplification is to make the station a service point rather than owner of paid progress; **not adopted**. Moving a service point should not accidentally erase paid investments.
- Show actual before/after rating before payment. Adding only white skulls or decoration can be ineffective at the other limit. Red conversion and extra decoration can partially substitute for one another while reds remain, so compare their pricing.
- Compare prices with repeatable faith income and remaining uses. Ordinary and DLC prayer income differ. A number that sounds large in isolation is not proof of a costly late-game purchase.
- Appearance and function need not be locked together. Useful improvement should not force a gold finish or strong particles on every player.
- A physical-body edit can export improvements through exhumation/organs/zombie use; a cemetery-only alternative could preserve the role of anatomy elsewhere. The technical route and desired semantics are unselected.
- Rewards should reflect completed work, not merely the number of paid button presses. Determine whether sarcophagus reward unlocks crafting, gives a first item, or does something else; no milestone or exclusivity adopted.

## Related reusable objects

- **Sarcophagus embellishment:** the user proposed directly investing money in visible gold/shine stages, perhaps increasing graveyard quality too. Unlock, stages, costs and relation to accomplishment rewards are open.
- **Appearance treatments:** purchase silver/gold or other visual variants for existing decorations while retaining function. Money may pay for craftsmanship; literal coin melting is unnecessary. Ingots are another possible input, not a settled requirement. Whole-sprite tint does not establish convincing metal, lifecycle persistence or compatibility; implementation cost is unverified.
- **Communion cookies:** proposed recipe, possible faith cost, sales to the Bishop and perhaps relationship effects. `communion_cookies` exists; no output recipe was found in the targeted `craft_data` check. Buyer/seller eligibility, new recipe and relationship behavior remain unverified. Faith-to-money conversion competes with the intended faith sink and is not itself a money sink.
- **Red candles:** proposed Bishop stock and +4 church quality in a candelabrum. This is a proposal, not an established native `candle_6` effect; usage/placement compatibility needs verification.
- **Perfume gifts:** proposed expensive gifts advancing merchants' trade tiers. Exact native item/use, eligible recipients, tier progression and price need separate checks. Trade tiers must not be conflated with friendship or quest progress.

Prayer stations are already +2/+4 quality decorations and require books in construction; calling them literally useless is inaccurate, though their active gameplay role could be expanded. Stone/wood progression can be investigated before inventing a new building.

Columbaria, candles/incense, paths, flowerbeds and lanterns are possible subjects for narrow follow-up. Existing use is not evidence they need extra functions. `candle_6` and `skull_candle` remain incomplete asset candidates, not promised easy rewards. Do not start another bulk asset audit without a concrete role/question.

## Prior market check relevant to novelty

- [Pray the Day Away](https://www.nexusmods.com/graveyardkeeper/mods/72?tab=description) advertises extra sermons with consumption/degradation of prayers as well as a free cheat mode. Therefore paid sermons alone are not a novel differentiator.
- [The author's mod catalog](https://github.com/p1xel8ted/Graveyard-Keeper-Mods) also advertises repeat corpse delivery, fast travel and fishing convenience. Descriptions were read; installed compatibility was not tested.
- No representative frequency of complaints about bodies/faith/late waiting has been measured. Player testimony cannot by itself establish target-market size.

Next useful comparison: inspect several actual grave states with different body/decoration limits, compare the real gains of each station action and express candidate prices in sermon cycles. Visual variants need a separate small preview before production commitment.
