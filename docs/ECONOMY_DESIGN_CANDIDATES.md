# Late-game economy — exploratory design candidates

Status: **idea bank**, captured 2026-10-22 at the user's request. No production owner/mod, implementation, balance, unlock conditions or runtime acceptance is selected. This is not a second production design contract. If a production project adopts a candidate, its requirements and acceptance belong there.

Evidence owners: [resource saturation](ECONOMY_PROGRESSION_RESEARCH.md), [graveyard host facts](GRAVEYARD_RUNTIME.md), [player needs and aesthetic motivation](GRAVEYARD_PLAYER_NEEDS.md).

## Scope and retained direction

- Improve the last third of an ordinary playthrough with limited new content, using existing systems/assets where feasible.
- Optional spending should enable desired play or an appreciable result; ingredient gathering and mandatory chores are not themselves proof of a meaningful choice.
- The user found an alternative to anatomy promising: expensive grave-specific improvement could let players use other developed economies instead of repeatedly preparing better bodies. **Retain for future exploration**, not implement as an adopted mechanic.
- The earlier proposal of exactly three special graves has no host/economic justification. Do not preserve that arbitrary limit as a requirement.
- **Latest clarification:** the user means work on **one selected grave**. Surrounding-grave quality as an activation gate is removed from the current candidate. A spatial district minigame is superseded, not an implementation requirement.
- **Agreed working design:** a grave in a prayer station's radius gains free content inspection and access to two paid actions: convert one red skull to white; add one effective decoration/wreath point without physically replacing decoration. These are complementary alternatives chosen freely by the player, not competing feature proposals.
- Both paid actions use **one escalating sequence for that grave**. Any paid improvement advances its shared counter; switching the action or replacing the prayer station does not restart the sequence. Illustrative 10/20/etc. prices are not agreed balance; numeric tuning is deferred. Improvements should consume substantial surplus faith.
- Inspection shows actual contained items and quantities, including non-organ items if present. It does **not** reveal organs' skull-quality contributions. Do not devalue [Cultist](PERK_MECHANICS.md#cultist); the new contents view need not add numerical organ quality even when that perk exists. Do not assume example item contents normally occur in all native bodies.
- **Agreed working separation:** faith changes grave state; money pays for appearance customization while preserving effectiveness. Silver/gold are selectable styles, not mandatory stronger tiers.
- Paid improvements persist when the prayer station is demolished or moved. The station grants current access; it does not own the paid result. Exact physical-body versus grave-only implementation and exhumation/body-replacement rules remain unselected.
- **Tier candidate:** wooden prayer stations provide early access/preview and allow the first N paid improvements per grave; stone allows continuation of the same sequence without that station-tier cap. N and unlock timing are unselected. A tier cap already reached is not reset by moving/rebuilding the wooden station. Earlier effects stay when access is downgraded or absent.
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
- Shared price progression belongs to the grave; station replacement/reset exploits are resolved at design level by retaining this progression. Exhumation/body replacement and refunds still need a clear product rule.
- Radius size, selection and overlap priority remain open. The former first-built-wins/overlapping-aura discussion is not an adopted dependency. Proposed simplification: show the highest available station tier when multiple stations cover the same grave, without making paid progress depend on a particular station ID.
- Show current station type, grave improvement count/cap, shared next price and actual before/after rating. No need to reopen the user's complementary-action choice merely because both actions can sometimes yield the same numerical gain.
- Numerical faith-income/price tuning is deferred by the user; ordinary/DLC income differences remain evidence for that later task, not a blocker to this design discussion.
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

## Interface exploration

An illustrative interactive grave-window preview compares two layouts: **tabs** (grave work / appearance, contents disclosure within grave work) and **disclosures** (contents / prayer-station work / appearance). Neither layout is adopted or verified in native NGUI. It illustrates a shared price, a wooden cap, stone continuation and retained paid progress with no station. All prices, cap values, body inventory and appearance art are examples.

The preview's paid appearance ownership/free reapplication behavior is only an interface example, not an adopted economy rule. Physical grave/decoration removal, controller navigation, native available space and additional UI constraints require owning-project design/host checks before implementation.

Standalone browser check: shared illustrative prices 10 -> 20 -> 40; wooden cap; stone continuation; retained improvements with no station; paid appearance change without score change; both layouts and no horizontal overflow at 360px. No page errors observed. This proves the illustrative preview's interactions, not native NGUI fit, game persistence or mod compatibility. Desktop/mobile screenshots were inspected.

Next useful decision: compare these contained layouts, choose the clearest access path and agree on exhumation/body-replacement ownership. Numeric pricing remains deferred; native UI implementation is not authorized by the preview.

## Exhumation and body replacement — recommendation pending agreement

The user requested this comparison; no ownership outcome is adopted yet. Preserve the agreed shared per-grave price/count when assessing alternatives.

- **Mixed ownership:** true skull conversion follows the physical body; added wreaths remain with the grave. This matches literal body editing but splits paid results from their shared price owner. Moving one body among low-count graves can reduce progressive conversion cost and may bypass a wooden-tier limit intended for the improvement sequence. Avoid claiming a proven runtime exploit: this is a consequence of the proposed pricing/ownership rules, not implemented behavior. Resolving it needs additional body history or a revised pricing contract.
- **Recommended candidate:** both faith purchases belong to the grave. The conversion is an effective burial modifier, not a change to organs or physical body stats. Native anatomy/embalming changes still follow the body. Paid appearance treatment belongs to the selected physical decoration item, not its current grave. Cosmetic ownership is a recommendation, not an adopted rule or verified save capability.
- Exhumation removes the original body with its ordinary anatomy changes. Both ritual investments and the shared paid-action count stay at the grave; there is no automatic refund or price reset. Empty graves receive no bonus contribution from these investments. A new body receives the grave's effects on burial.
- Conversion capacity applies only to red skulls actually present: effective conversions are bounded by available red skulls. A red-free replacement does not gain white skulls from unused conversion capacity. Decoration remains a separate limit, and the same investment may yield a different score for a different body.
- Moving that body elsewhere exports no prayer-station grave modifier and therefore no extra organ/zombie improvement. Moving an appearance-treated gravestone/fence carries its appearance; quality/effectiveness remains unchanged. Underlying compatible item/save identity still needs host investigation.
- Station demolition/movement only changes current service access. It neither removes paid grave effects nor resets the action count. This repeats the agreed access/result separation, not a new dependency on the station.
- UI should distinguish **body skulls**, **grave modifiers** and **effective burial result**. Never present a grave-only modifier as rewritten organs. Show decoration as its own quality plus paid wreaths. Keep contents inspection free/in radius and without organ-quality disclosure.
- Before exhumation, add a compact consequence summary to the existing action/confirmation area: body leaves; grave investments and next price remain. Show replacement-body before/after result before burial when practical. No extra nested confirmation flow is proposed.
- If the user instead wants actual persistent body cleansing, retain that as a legitimate alternative and explicitly revisit shared-counter/tier semantics before adoption. Full grave demolition/movement and mod-removal behavior still require their own clear ownership rules; no destructive rule is inferred here.
