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

## Extended bounded census and paper demand — 2026-10-14

Same accepted input hashes and existing private game_data loader. There are 223 distinct paid single-input item Survey definitions (CraftType.Survey, subtype Alchemy) with a total of 649 authored science and 613 faith. Another eleven SurveySciencePoints definitions are currency conversion, not included. These are authored totals, not demonstrated mandatory player expenditure or a designer-intended completion budget. Excluding failed-experiment goo and monster jelly leaves 198 studied items / 624 science / 609 faith. Some studied objects serve non-alchemy systems and have their own technology-point rewards; all-study expenditure must not be assigned solely to alchemy-information discovery.

The ordinary 34-product/43-formula corpus uses 35 ingredients. The accepted producer catalog and non-workstation source channels yield 38 distinct non-slime material Study inputs with total 97 science/97 faith. Six ingredient inputs have at least one non-Survey acquisition channel; 29 need one of these studied-material routes in the bounded acquisition model. This classification omits purchases of ready alchemical reagents and assumes future availability, not actual early/save-specific access. Do not confuse it with the separate 44-formula/36-input quest-inclusive guidance audit.

Allowing monster jelly while still excluding failed-experiment goo adds four material Study inputs: 42 total / 101 science / 101 faith. The consumer's non-slime guidance coverage requirement must not be mistaken for a player prohibition against these valid alternatives. These two material populations are intentionally distinct sensitivity scenarios.

Paper-demand evidence:

- flyer_bad / flyer_bad_2 at Desk / Desk II consume one clean paper and produce five flyers; flyer_good / flyer_good_2 at printing presses consume two paper and produce ten flyers. Thus the paper part of ten flyers is two sheets, excluding writing supplies/energy.
- Native FlowScript JSON for npc_actor (path_id 150506) has resource node 291 with paper_clean=10 -> Flow_Answer 294 price, and resource node 279 with ink:ink_jar=1 -> answer 282 price. This is the Vagner request, not a one-sheet Astrologer request.
- npc_inquisitor graph (path_id 150539) has resource nodes 885 and 918 with flyer:flyer_bad=10 -> answer prices 888/921. The census does not establish a weekly repeat-paper tax.
- storage_builddesk::give_marketing_point consumes ten flyers; its native end_script is give_marketing_point. Community documentation identifies the one-fame campaign. Arbitrary campaign counts are scenarios, not mandatory counts.
- paper_clean has base_price=1, base_count=2, tier I, product type paper; npc_astrologer's native product_types includes paper. Community trade tables identify the one-silver base price. Current transaction prices, refill/wait timing and arbitrary purchase volume are not measured by these base fields.

Private scratch/results are preserved in the consumer's ignored economics-census-private, economics-extra-private, paper-quest-audit-private and paper-graphs-private files. Only derived quantities/owners are promoted; no graph or dialogue dumps are published. Consumer-specific optimization, scenarios, classification and pricing hypotheses belong to ResearchableAlchemyRecipes/docs/RESEARCH_ECONOMICS_CALCULATIONS.md and its spoiler-free results JSON.
