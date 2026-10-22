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
- **Revised coverage rule, accepted 2026-10-23:** paid improvements remain stored at the grave. Any wooden or stone station in radius activates **all** stored faith improvements. With no covering station they are dormant, not deleted or refunded. Appearance changes remain on the physical decoration and do not depend on faith coverage.
- **Tier rule:** wooden prayer stations provide early inspection and permit the first N paid improvements per grave; stone permits further purchases without that tier cap. N is illustrative. The highest covering tier governs purchase permission only. A wooden station also activates all previously purchased effects, even above N; no effect ordering or station ID ownership is needed. Moving/rebuilding never resets the shared counter.
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
- Effect ownership is agreed as grave-only compensation, not permanent body mutation. One selected grave per interaction is the current scope; do not silently replace it with an aura.
- If the mechanism only converts red skulls, it stops when red skulls run out. A finite cemetery and decoration limits also bound useful improvement. Exponentially rising prices alone do not establish an infinite useful sink.
- Shared price progression belongs to the grave and remains through exhumation/body replacement. No automatic refund/reset occurs. Station replacement does not reset this progression.
- Radius size and grave selection remain open. Overlap uses the highest covering tier for purchase permission; any covering station activates all stored effects. First-built-wins and ordered partial activation are superseded.
- Show current station type, grave improvement count/cap, shared next price and actual before/after rating. No need to reopen the user's complementary-action choice merely because both actions can sometimes yield the same numerical gain.
- Numerical faith-income/price tuning is deferred by the user; ordinary/DLC income differences remain evidence for that later task, not a blocker to this design discussion.
- Appearance and function need not be locked together. Useful improvement should not force a gold finish or strong particles on every player.
- The selected grave-only effect does not export skull improvements through exhumation/organs/zombie use. The technical route remains unverified.
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

An illustrative interactive grave-window preview compares two layouts: **tabs** and **disclosures**. Neither is adopted or verified in native NGUI. It illustrates shared pricing, purchase caps, stored versus active improvements and temporary appearance fitting. All prices, cap values, inventory contents and drawn appearance silhouettes are examples, not extracted game sprites.

The current appearance baseline reuses existing available gravestone/fence looks while retaining the source item's quality. New gold/silver, Gothic, carved-wood, ivy and other motifs are optional later content. Availability/unlocks, prices, repeat-order policy and category compatibility details remain open. The preview illustrates a paid commission per confirmed change, not an adopted repeat-payment rule.

Earlier standalone checks covered shared prices, caps and appearance score preservation; their old no-station-active and metal-finish examples are superseded by the current coverage and appearance decisions. Browser checks prove only illustrative interactions, never native NGUI fit, game persistence or mod compatibility.

Updated standalone check, 2026-10-23: shared prices/caps; stone continuation; no-coverage score returning to the unmodified example; all three bought effects reactivating under wooden coverage despite a two-purchase cap; fitting/cancellation without spending; confirmed appearance purchase preserving score; both layouts and 360px without horizontal overflow; no page errors. Desktop/mobile screenshots inspected. The cemetery and silhouettes are schematic interaction aids, not verified native art previews.

Accepted fitting direction, 2026-10-23: choose the installed decoration slot -> browse available existing looks -> temporarily preview on the actual grave in the actual cemetery -> confirm the visible price or cancel. Browsing/cancellation spends nothing and does not commit an item change. A separate rendered scene inside the grave window is not required. The user accepted existing appearances for the first version and deferred concrete native UI layout to a later iteration. Technical feasibility remains unverified. Appearance availability/unlocks remain open; restricting to learned technologies is a recommendation, not an adopted rule.

## Exhumation and body replacement — agreed working design

The user accepted this ownership recommendation on 2026-10-23. These are working product semantics, not verified implementation or runtime acceptance.

- **Mixed ownership:** true skull conversion follows the physical body; added wreaths remain with the grave. This matches literal body editing but splits paid results from their shared price owner. Moving one body among low-count graves can reduce progressive conversion cost and may bypass a wooden-tier limit intended for the improvement sequence. Avoid claiming a proven runtime exploit: this is a consequence of the proposed pricing/ownership rules, not implemented behavior. Resolving it needs additional body history or a revised pricing contract.
- **Selected:** both faith purchases belong to the grave. The conversion is an effective burial modifier, not a change to organs or physical body stats. Native anatomy/embalming changes still follow the body. Paid appearance treatment belongs to the selected physical decoration item, not its current grave. Storage/transfer/rendering capability still needs end-to-end verification.
- Exhumation removes the original body with its ordinary anatomy changes. Both ritual investments and the shared paid-action count stay at the grave; there is no automatic refund or price reset. Empty graves receive no bonus contribution from these investments. A new body receives the grave's effects on burial.
- Conversion capacity applies only to red skulls actually present: effective conversions are bounded by available red skulls. A red-free replacement does not gain white skulls from unused conversion capacity. Decoration remains a separate limit, and the same investment may yield a different score for a different body.
- Moving that body elsewhere exports no prayer-station grave modifier and therefore no extra organ/zombie improvement. Moving an appearance-treated gravestone/fence carries its appearance; quality/effectiveness remains unchanged. Underlying compatible item/save identity still needs host investigation.
- Station demolition/movement re-evaluates coverage: without any station, stored faith effects become dormant; under either tier they all activate again. Investments and action count remain. This latest decision supersedes the former access-only rule. Before demolition, a compact affected-grave/quality-loss preview would make the dependency understandable; that presentation is a recommendation.
- UI should distinguish **body skulls**, **grave modifiers** and **effective burial result**. The user accepted a compact two-line summary of body state and effective ritual result. Never present a grave-only modifier as rewritten organs. Show decoration as its own quality plus paid wreaths. Keep contents inspection free/in radius and without organ-quality disclosure.
- Before exhumation, add a compact consequence summary to the existing action/confirmation area: body leaves; grave investments and next price remain. Show replacement-body before/after result before burial when practical. No extra nested confirmation flow is proposed.
- Actual persistent body cleansing is not the selected route. Reopen only on new user direction, including its pricing implications. Full grave demolition/movement and mod-removal behavior still require clear rules; the user's belief that an excavated grave cannot be removed is not treated as verified host evidence.

## Appearance identity and presentation — 2026-10-23

The user emphasized that an appearance must remain distinguishable on the physical item in inventory/storage, after installation/removal and in tooltips. A marker and changed icon are candidate presentation; an exact badge is not selected. Distinguish cosmetic treatment from native bronze/silver/gold quality. Do not alter native item-quality stats merely to obtain a quality star.

Proposed presentation: retain the base item name, show a selectable finish in the icon and a separate tooltip line such as `Finish: silver-plated`; preserve the native decoration-quality information. Grave-bound extra wreaths/skull effects must not be attributed to a removed decoration in its tooltip.

The [bounded host check](GRAVEYARD_RUNTIME.md#appearance-identity-seams--2026-10-23) finds serializable instance parameters and non-stackable sampled grave decorations, making a namespaced numeric appearance parameter a plausible first route. It is not a completed round trip: installation/removal recipes, UI redraw and world sprites need explicit handling. Separate definition IDs remain another route, with registration/recipe/loading implications; no architecture selected yet.

Next technical acceptance slice, if implementation is later authorized: one base decoration, ordinary plus two finishes, distinguish them -> chest -> save/load -> install -> remove -> transfer to a second grave -> save/load; verify retained appearance and unchanged quality/durability. Also inspect a stack-changing mod only if it affects these items. No production probe is requested or supplied here.

## Concept review — 2026-10-23, recommendations not adopted

The user requested critique assuming technical feasibility. Do not resume implementation-seam research as a substitute for this design review. Numerical faith prices remain deferred. The user identified early station availability versus late-game positioning and asked whether technology-point spending fits naturally; point spending is optional, not a requirement to force into the system.

- **Core benefit and audience:** the current design offers optional graveyard convenience and appearance choice. It can shorten disliked repetition for players still pursuing a desired cemetery. It does not establish a new source of goals for someone already finished with the graveyard, or new demand for every production chain. Do not promise universal late-game renewal.
- **Scope resolved by the user:** partial anatomy bypass is intentional. For an unspoiled 2-red/4-white body, two conversions produce effective 0-red/6-white; extra decoration alone cannot exceed six. Do not add independent white-skull creation to obtain a complete anatomy bypass.
- **Early access need not equal early economical use:** free inspection can be useful from the wooden station onward. Paid improvement can remain a poor early use of faith and become attractive with free late income. First-step price matters structurally: a low first price per grave can be spread over many graves despite a small wooden per-grave cap. A steep later curve does not protect early progression by itself.
- **Candidate timing resolution:** prefer gradual access through the existing wooden/stone progression, with costly paid work rather than an arbitrary last-third calendar lock. If strict late-only power is desired instead, select one meaningful later unlock separately. Do not stack a new research gate, church score gate and high price without a distinct purpose for each. No unlock criterion is adopted.
- **Radius revision:** coverage now maintains active faith effects as well as access. Moving a station away suspends uncovered graves; wooden stations can maintain all improvements acquired under stone. Distinguish dormant investments from lost investments in the UI. This adds a placement dependency, without reinstating surrounding-quality gates.
- **Money and appearance:** willingness to spend depends on an appealing set of actual looks; gold/silver alone do not establish that demand. Avoid restoring a compulsory best appearance through score-linked tiers. Sarcophagus quality bought with coins remains a separate side proposal, not automatically part of the agreed appearance-only treatment system.
- **Technology points:** native red/green/blue balances differ by route/automation; do not add a repeat tax in all three colors. A finite optional unlock of an actual new treatment/tool/style family fits their knowledge role better than paying points on each grave. If money is then charged to apply the treatment, explain the different benefits of learning a method versus commissioning work. This candidate would absorb only finite points and can feel like two tolls without a meaningful method unlock. No new technology branch or point sink is adopted; leaving colors outside the initial scope is valid.
- **Next design decision:** evaluate the coverage/status presentation and appearance fitting flow. Partial bypass and reuse of existing looks are accepted directions. Exact initial faith price remains unbalanced: 10 is the user's illustrative serious early expense, not a verified fixed number of sermons/weeks.
