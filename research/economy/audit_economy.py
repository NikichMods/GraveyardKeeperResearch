"""Read one local GK 1.407 balance asset; publish only bounded derived evidence.

Requires UnityPy 1.23.0 and its TypeTreeGenerator dependencies. No game execution,
save access, network access, copied source, or full balance export. See the research
documents for interpretation; definition prices are not a playthrough budget.
"""
import argparse
from collections import Counter, defaultdict
import hashlib
import json
from pathlib import Path
import sys


def resource_map(value):
    return dict(zip(value.get("_res_type", []), value.get("_res_v", [])))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--assets", type=Path, required=True)
    parser.add_argument("--managed", type=Path, required=True)
    parser.add_argument("--python-libs", type=Path)
    parser.add_argument("--out", type=Path, required=True)
    args = parser.parse_args()
    if args.python_libs:
        sys.path.insert(0, str(args.python_libs))
    import UnityPy
    from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

    hashes = {"resources.assets": hashlib.sha256(args.assets.read_bytes()).hexdigest(),
              "Assembly-CSharp.dll": hashlib.sha256(
                  (args.managed / "Assembly-CSharp.dll").read_bytes()).hexdigest()}
    expected = {
        "resources.assets": "215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca",
        "Assembly-CSharp.dll": "e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8"}
    if hashes != expected:
        raise SystemExit("Unreviewed input identity; update evidence deliberately before auditing")
    env = UnityPy.load(str(args.assets))
    asset = next(iter(env.files.values()))
    generator = TypeTreeGenerator(asset.unity_version)
    generator.load_local_dll_folder(str(args.managed))
    env.typetree_generator = generator
    balances = [obj for obj in env.objects
                if obj.type.name == "MonoBehaviour" and obj.peek_name() == "game_data"]
    assert len(balances) == 1
    data = balances[0].read_typetree()
    techs = {t["id"]: t for t in data["techs_data"]}
    assert len(techs) == len(data["techs_data"])
    items = {item["id"]: item for item in data["items_data"]}
    groups = defaultdict(lambda: {"count": 0, "cost": Counter()})
    branches = defaultdict(lambda: {"count": 0, "cost": Counter()})
    for tech in techs.values():
        key = f"dlc={tech['requires_dlc']};hidden={tech['hidden']};invisible={tech['invisible']}"
        groups[key]["count"] += 1
        groups[key]["cost"].update(resource_map(tech["price"]))
        if not tech["hidden"] and not tech["invisible"]:
            branch = branches[str(tech["branch_type"])]
            branch["count"] += 1
            branch["cost"].update(resource_map(tech["price"]))

    roots = ["Stone gravestones", "Carved marble gravestones", "Shining of faith",
             "Zombie winemaking", "Random text generator"]
    closures = {}
    for root in roots:
        visited = set()

        def visit(key):
            if not key or key in visited:
                return
            visited.add(key)
            for parent in techs[key]["_parents"]:
                visit(parent)

        visit(root)
        cost = Counter()
        hidden = []
        for key in sorted(visited):
            cost.update(resource_map(techs[key]["price"]))
            if techs[key]["hidden"]:
                hidden.append(key)
        closures[root] = {"definition_cost_including_hidden": cost,
                          "hidden_may_be_script_granted": hidden,
                          "nodes": sorted(visited)}

    crafts = data["craft_data"]
    faith_crafts = [{"id": c["id"], "faith": sum(n["value"] for n in c["needs"]
                    if n["id"] == "faith"), "one_time": c["one_time_craft"]}
                   for c in crafts + data["craft_obj_data"] if not c["id"].startswith("surv:")
                   and any(n["id"] == "faith" for n in c["needs"])]
    sample_ids = {"zombie_craft", "zombie_craft_techbook", "box_goods_nails",
                  "grave_bot_stn_1", "grave_top_stn_plate_2", "stone_plate_2",
                  "marble_plate_2", "paper_bad", "carved_wood", "marble_plate_3",
                  "cutlet_carrot", "garden_carrot_planting_1", "rope_hemp",
                  "wine_zombie_3", "bottle_red_vine", "nails", "firewood"}
    samples = []
    prayers = []
    for craft in crafts:
        if craft["id"] in sample_ids:
            samples.append({"id": craft["id"], "station": craft["craft_in"],
                            "inputs": [[n["id"], n["value"]] for n in craft["needs"]],
                            "outputs": [[n["id"], n["value"], n["chance_group"],
                                         n["common_chance"]["_expression"],
                                         n["self_chance"]["_expression"],
                                         n["min_value"]["_expression"],
                                         n["max_value"]["_expression"]]
                                        for n in craft["output"]],
                            "work_expression_not_seconds": craft["craft_time"]["_expression"]})
        if craft["id"].startswith(("pray:b_faith:", "pray:b_faith_money:", "pray:b_souls:")):
            prayers.append({"id": craft["id"], "event": craft["linked_sub_id"],
                            "required_quality": craft["needs_quality"],
                            "faith_multiplier_bonus": craft["k_faith"],
                            "money_multiplier_bonus": craft["k_money"],
                            "success_fixed": [[n["id"], n["value"]] for n in craft["output"]]})

    # Only named quest/transaction graphs, only money/faith resource edges.
    graph_names = {"npc_royal_box", "npc_actress", "npc_cultist", "npc_euric",
                   "npc_carpenter", "npc_tavern owner", "npc_merchant",
                   "npc_gypsy", "npc_teodoro", "npc_refugee_moneylender",
                   "npc_engineer", "npc_hunchback"}
    payments = []
    for obj in env.objects:
        if obj.type.name != "MonoBehaviour" or obj.peek_name() not in graph_names:
            continue
        definition = obj.read_typetree()
        if not definition.get("_serializedGraph"):
            continue
        graph = json.loads(definition["_serializedGraph"], strict=False)
        for node in graph["nodes"]:
            if "$id" not in node:
                continue
            values = node.get("_inputPortValues", {})
            flat = {key: value.get("$content") if isinstance(value, dict) else value
                    for key, value in values.items()}
            kind = node.get("$type", "")
            resource = flat.get("id", flat.get("Id"))
            if "SmartRes" in kind and resource in {"money", "faith"}:
                edges = sorted({(e["_targetNode"]["$ref"], e["_targetPortName"])
                                for e in graph["connections"]
                                if e.get("_sourceNode", {}).get("$ref") == node["$id"]})
                payments.append({"graph": definition["m_Name"], "path_id": obj.path_id,
                                 "node": node["$id"], "resource": resource,
                                 "value": flat.get("v", flat.get("V")), "edges": edges})
            elif "Flow_AddMoney" in kind:
                payments.append({"graph": definition["m_Name"], "path_id": obj.path_id,
                                 "node": node["$id"], "resource": "money_grant",
                                 "value": flat.get("Value"), "edges": []})

    selected_items = ["box_goods", "box_vegetables_silver", "box_vegetables_gold",
                      "bottle_red_vine:3", "cup_beer:3", "ingot_silver", "ingot_gold",
                      "techbook_r_1", "techbook_g_1", "techbook_b_1", "techbook_b_2"]
    result = {"game": "Graveyard Keeper 1.407", "status": "bounded static data",
              "input_sha256": hashes, "unitypy_version": UnityPy.__version__,
              "technology_definitions": len(techs), "technology_strata": groups,
              "visible_definition_branches_not_save_spend": branches,
              "technology_prerequisite_closures": closures,
              "non_survey_faith_crafts": faith_crafts,
              "selected_crafts": samples, "selected_prayers": prayers,
              "prayer_base_expressions": [{"id": p["id"], "faith": p["faith"]["_expression"],
                                           "money": p["money"]["_expression"]}
                                          for p in data["pray_events_data"] if p["id"] in
                                          {"default_1", "default_3", "pray_for_souls_3"}],
              "selected_items": [{"id": key, "base_price_silver": items[key]["base_price"],
                                  "static_cost": items[key]["is_static_cost"],
                                  "use_resources": resource_map(items[key]["params_on_use"])}
                                 for key in selected_items],
              "bounded_graph_resource_edges_not_route_totals": payments}
    args.out.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n",
                        encoding="utf-8", newline="\n")
    print(f"Wrote derived evidence: {len(techs)} technology definitions, "
          f"{len(payments)} bounded resource nodes; no raw asset export")


if __name__ == "__main__":
    main()
