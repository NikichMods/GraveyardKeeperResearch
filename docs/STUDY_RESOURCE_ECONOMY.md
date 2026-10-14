# Study resource economy — static 1.407 audit, 2026-10-14

Status: bounded static findings, not installed economy/playtime measurements. Consumer: ResearchableAlchemyRecipes research economics. Existing payment/cancellation runtime evidence remains in CRAFTING_INVENTORY_AND_TRADING.md.

Inputs: resources.assets SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`; Assembly-CSharp.dll SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`, matching accepted 1.407 identities. Read-only UnityPy/TypeTreeGenerator game_data projection, using the existing private loader. Consumer scratch/result: ignored work/palette/economics-audit-private.{py,json}; no game assets or full native definitions published.

## Paper/science conversion and production

| Native operation | Authored inputs | Authored output | Limits |
| --- | --- | --- | --- |
| surv:paper_clean | 1 clean paper | 2 science through output_to_wgo | SurveySciencePoints subtype; zero authored time/energy, repeatable; science belongs to station, not technology-point currency |
| paper_bad | 1 pigskin paper | 4 clean paper | Church workbench |
| skroll_skin_pig | 1 human skin | 4 pigskin paper | Church workbench; do not infer this skin's opportunity cost or access from yield |
| skroll_skin_pig_2 | 5 bat wings | 2 pigskin paper | Church workbench |
| pail_wet_paper | 1 water bucket, 20 wheat | 1 paper glop | Church workbench |
| paper_from_herb | 2 paper glop, 1 white paint | 30 clean paper | Paper press; paint acquisition is an additional dependency; no alchemy formula disclosed |
| old_paper / old_paper_2 | 1 ruined book, 1 river sand | 2 clean paper | Desk / Desk II |

Derived authored yield, before acquisition/setup costs: one skin -> 16 clean paper -> 32 science; five bat wings -> 8 clean paper -> 16 science; one press operation -> 60 science. These are resource equivalences, not rates per day/minute or proof that all routes are available to a particular save. They differ from some community comments and must not be silently replaced by remembered/wiki values.

Paper crafting unlocks the church-workbench placement, skroll_skin_pig, hidden sibling @skroll_skin_pig_2, and paper_bad. Paper production unlocks the paper-press placement, paper_from_herb and pail_wet_paper. Hardspares includes the relevant skin-extraction crafts. This audit does not reconstruct the full technology prerequisite graph or measure route availability on early saves.

## Payment and study bounds

Existing common-work owner supports inventory needs and station needs_from_wgo; faith/science execution is installed-confirmed. Other ordinary inventory items are structurally supported by that owner, but no new-item research runtime acceptance is implied. Money and technology points must not be treated as ordinary Item costs without inspecting their respective native owners.

In the guidance producer-input subset with matching native Survey records, costs include 1 faith/1 science for common low-cost materials, 2/2 and 3/3 for others, and 7/7 or 10/10 for expensive alternatives. Failed-experiment goo Survey records can cost 1 science and no faith. This is a subset/classification, not a demand that players study all routes, an exhaustive cheapest-route calculation, or a universal faith prerequisite for every ingredient. Production/processing inputs that bypass Survey remain separate.

Native Cancel refunds inventory inputs as world drops but does not generally restore station needs_from_wgo; accepted runtime science retention remains relevant if research uses science as its principal cost. Selecting a science-heavy price does not adopt cancellation loss.

rand_alchemy_unlock has authored base_price=5 and tier I alchemy product membership. This is not by itself a proved current 5-silver transaction, draw distribution or remaining-only selection rule. Community reports identify the familiar 5-silver random-recipe purchase; exact dynamic trading and random grant semantics are not newly audited here.
