using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GKTechnologyUnlockAudit
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class TechnologyUnlockAuditPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "nikich.gyk.diagnostics.technologyunlockaudit";
        public const string PluginName = "GK Technology Work Perk Audit (Diagnostic)";
        public const string PluginVersion = "0.1.0";

        private static readonly BindingFlags AnyInstance =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags AnyStatic =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static ManualLogSource Log;
        private static Harmony HarmonyInstance;
        private static bool _auditComplete;
        private static MethodInfo _localize;

        private sealed class UnlockRecord
        {
            internal string TechId;
            internal string Type;
            internal string Id;
        }

        private void Awake()
        {
            Log = Logger;

            try
            {
                Type techTreeGui = FindType("TechTreeGUI");
                if (techTreeGui == null)
                    throw new MissingMemberException("TechTreeGUI type not found.");

                MethodInfo open = techTreeGui.GetMethod(
                    "Open",
                    AnyInstance,
                    null,
                    Type.EmptyTypes,
                    null);
                if (open == null)
                    throw new MissingMethodException("TechTreeGUI.Open()");

                MethodInfo postfix = AccessTools.Method(
                    typeof(TechnologyUnlockAuditPlugin),
                    nameof(TechTreeOpenPostfix));

                HarmonyInstance = new Harmony(PluginGuid);
                HarmonyInstance.Patch(open, postfix: new HarmonyMethod(postfix));

                Logger.LogInfo(
                    PluginName + " " + PluginVersion +
                    " loaded. Open the Technology tree once; the audit is read-only and logs one bounded snapshot.");
            }
            catch (Exception ex)
            {
                Logger.LogError("Technology Work/Perk audit initialization failed: " + ex);
            }
        }

        private void OnDestroy()
        {
            if (HarmonyInstance != null)
                HarmonyInstance.UnpatchSelf();
        }

        private static void TechTreeOpenPostfix()
        {
            if (_auditComplete) return;

            try
            {
                RunAudit();
                _auditComplete = true;
            }
            catch (Exception ex)
            {
                Log.LogError("TECH_UNLOCK_AUDIT_FAILED " + ex);
            }
        }

        private static void RunAudit()
        {
            Type gameBalanceType = FindType("GameBalance");
            if (gameBalanceType == null)
                throw new MissingMemberException("GameBalance type not found.");

            object balance = GetStaticMember(gameBalanceType, "me");
            if (balance == null)
                throw new InvalidOperationException("GameBalance.me is null.");

            BindLocalization();

            IList techs = AsList(GetMember(balance, "techs_data"));
            IList perks = AsList(GetMember(balance, "perks_data"));
            IList groups = AsList(GetMember(balance, "object_groups"));
            IList objects = AsList(GetMember(balance, "objs_data"));
            IList crafts = AsList(GetMember(balance, "craft_data"));
            IList objectCrafts = AsList(GetMember(balance, "craft_obj_data"));

            if (techs == null || perks == null || groups == null ||
                objects == null || crafts == null || objectCrafts == null)
            {
                throw new InvalidOperationException(
                    "One or more required GameBalance lists are unavailable.");
            }

            Dictionary<string, object> perkById = IndexById(perks);
            Dictionary<string, object> groupById = IndexById(groups);

            List<UnlockRecord> records = CollectVisibleWorkAndPerkUnlocks(techs);
            List<UnlockRecord> workRecords = records
                .Where(r => r.Type == "Work")
                .OrderBy(r => r.TechId, StringComparer.Ordinal)
                .ThenBy(r => r.Id, StringComparer.Ordinal)
                .ToList();
            List<UnlockRecord> perkRecords = records
                .Where(r => r.Type == "Perk")
                .OrderBy(r => r.TechId, StringComparer.Ordinal)
                .ThenBy(r => r.Id, StringComparer.Ordinal)
                .ToList();

            Log.LogWarning(
                "TECH_UNLOCK_AUDIT_BEGIN visible_work_refs=" + workRecords.Count +
                " visible_perk_refs=" + perkRecords.Count +
                " techs=" + techs.Count +
                " objects=" + objects.Count +
                " crafts=" + crafts.Count +
                " object_crafts=" + objectCrafts.Count + ".");

            int workMissingDescription = AuditWorks(workRecords, groupById);
            int perkMissingDescription = AuditPerks(
                perkRecords,
                perkById,
                crafts,
                objectCrafts,
                objects);

            Log.LogWarning(
                "TECH_UNLOCK_AUDIT_SUMMARY visible_work_refs=" + workRecords.Count +
                " work_missing_description=" + workMissingDescription +
                " visible_perk_refs=" + perkRecords.Count +
                " perk_missing_description=" + perkMissingDescription + ".");

            Log.LogWarning("TECH_UNLOCK_AUDIT_END");
        }

        private static List<UnlockRecord> CollectVisibleWorkAndPerkUnlocks(IList techs)
        {
            List<UnlockRecord> result = new List<UnlockRecord>();

            for (int i = 0; i < techs.Count; i++)
            {
                object tech = techs[i];
                if (tech == null) continue;

                string techId = SafeString(GetMember(tech, "id"));
                MethodInfo getUnlocks = tech.GetType().GetMethod(
                    "GetUnlocksList",
                    AnyInstance,
                    null,
                    Type.EmptyTypes,
                    null);
                if (getUnlocks == null) continue;

                IList unlocks = AsList(getUnlocks.Invoke(tech, null));
                if (unlocks == null) continue;

                for (int u = 0; u < unlocks.Count; u++)
                {
                    object unlock = unlocks[u];
                    if (unlock == null) continue;

                    bool visible = ToBool(GetMember(unlock, "visible"));
                    if (!visible) continue;

                    string type = SafeString(GetMember(unlock, "type"));
                    if (type != "Work" && type != "Perk") continue;

                    result.Add(new UnlockRecord
                    {
                        TechId = techId,
                        Type = type,
                        Id = SafeString(GetMember(unlock, "id"))
                    });
                }
            }

            return result;
        }

        private static int AuditWorks(
            List<UnlockRecord> records,
            Dictionary<string, object> groupById)
        {
            int missing = 0;

            foreach (UnlockRecord record in records)
            {
                string descriptionKey = record.Id + "_d";
                string description = Localize(descriptionKey);
                bool hasDescription =
                    !string.IsNullOrEmpty(description) &&
                    !string.Equals(description, descriptionKey, StringComparison.Ordinal);

                if (!hasDescription) missing++;

                object group;
                groupById.TryGetValue(record.Id, out group);
                IList groupObjects = group == null ? null : AsList(GetMember(group, "objects"));

                List<object> gatedObjects = new List<object>();
                if (groupObjects != null)
                {
                    for (int i = 0; i < groupObjects.Count; i++)
                    {
                        object obj = groupObjects[i];
                        if (obj == null || !ToBool(GetMember(obj, "need_unlock_work"))) continue;

                        IList objGroups = AsList(GetMember(obj, "object_groups"));
                        string firstGroupId =
                            objGroups != null && objGroups.Count > 0
                                ? SafeString(GetMember(objGroups[0], "id"))
                                : string.Empty;

                        if (string.Equals(firstGroupId, record.Id, StringComparison.Ordinal))
                            gatedObjects.Add(obj);
                    }
                }

                Log.LogWarning(
                    "TECH_UNLOCK_AUDIT_WORK tech=" + Q(record.TechId) +
                    " id=" + Q(record.Id) +
                    " name=" + Q(Localize(record.Id)) +
                    " description_present=" + Bool(hasDescription) +
                    " description=" + Q(hasDescription ? description : string.Empty) +
                    " group_found=" + Bool(group != null) +
                    " gated_object_count=" + gatedObjects.Count + ".");

                for (int i = 0; i < gatedObjects.Count; i++)
                {
                    object obj = gatedObjects[i];
                    string objId = SafeString(GetMember(obj, "id"));
                    IList drops = AsList(GetMember(obj, "drop_items"));

                    Log.LogWarning(
                        "TECH_UNLOCK_AUDIT_WORK_OBJECT work=" + Q(record.Id) +
                        " object=" + Q(objId) +
                        " name=" + Q(Localize(objId)) +
                        " work=" + Q(SafeString(GetMember(obj, "work"))) +
                        " drops=" + Q(DescribeItemIds(drops)) + ".");
                }
            }

            return missing;
        }

        private static int AuditPerks(
            List<UnlockRecord> records,
            Dictionary<string, object> perkById,
            IList crafts,
            IList objectCrafts,
            IList objects)
        {
            int missing = 0;

            foreach (UnlockRecord record in records)
            {
                object perk;
                perkById.TryGetValue(record.Id, out perk);

                string description = GetPerkDescription(perk, record.Id);
                bool hasDescription = !string.IsNullOrEmpty(description);
                if (!hasDescription) missing++;

                bool show = perk != null && ToBool(GetMember(perk, "show"));
                float stars = ToFloat(GetMember(perk, "stars"));
                string outputRes = DescribeGameRes(perk == null ? null : GetMember(perk, "output_res"));

                Log.LogWarning(
                    "TECH_UNLOCK_AUDIT_PERK tech=" + Q(record.TechId) +
                    " id=" + Q(record.Id) +
                    " name=" + Q(Localize(record.Id)) +
                    " definition_found=" + Bool(perk != null) +
                    " show=" + Bool(show) +
                    " description_present=" + Bool(hasDescription) +
                    " description=" + Q(description) +
                    " stars=" + stars.ToString("0.###", CultureInfo.InvariantCulture) +
                    " output_res=" + Q(outputRes) + ".");

                List<string> references = FindPerkReferences(
                    record.Id,
                    crafts,
                    objectCrafts,
                    objects);

                if (references.Count == 0)
                {
                    Log.LogWarning(
                        "TECH_UNLOCK_AUDIT_PERK_REFERENCE perk=" + Q(record.Id) +
                        " reference=" + Q("<none found in audited native seams>") + ".");
                }
                else
                {
                    for (int i = 0; i < references.Count; i++)
                    {
                        Log.LogWarning(
                            "TECH_UNLOCK_AUDIT_PERK_REFERENCE perk=" + Q(record.Id) +
                            " reference=" + Q(references[i]) + ".");
                    }
                }
            }

            return missing;
        }

        private static List<string> FindPerkReferences(
            string perkId,
            IList crafts,
            IList objectCrafts,
            IList objects)
        {
            List<string> references = new List<string>();

            ScanCraftListForPerk(perkId, crafts, "craft", references);
            ScanCraftListForPerk(perkId, objectCrafts, "object_craft", references);

            for (int i = 0; i < objects.Count; i++)
            {
                object obj = objects[i];
                if (obj == null) continue;

                string ownerId = SafeString(GetMember(obj, "id"));
                IList drops = AsList(GetMember(obj, "drop_items"));
                ScanItemListForPerk(
                    perkId,
                    drops,
                    "object_drop:" + ownerId,
                    references);

                AddExpressionReferenceIfContains(
                    perkId,
                    GetMember(obj, "add_player_param_after_hp_0_k"),
                    "object_expr:" + ownerId + ":add_player_param_after_hp_0_k",
                    references);
            }

            references.Sort(StringComparer.Ordinal);
            return references.Distinct(StringComparer.Ordinal).ToList();
        }

        private static void ScanCraftListForPerk(
            string perkId,
            IList craftList,
            string kind,
            List<string> references)
        {
            for (int i = 0; i < craftList.Count; i++)
            {
                object craft = craftList[i];
                if (craft == null) continue;

                string craftId = SafeString(GetMember(craft, "id"));
                IList linkedPerks = AsList(GetMember(craft, "linked_perks"));
                if (ContainsString(linkedPerks, perkId))
                {
                    references.Add(
                        kind + ":" + craftId +
                        ":linked_perk stars-consumer");
                }

                ScanItemListForPerk(
                    perkId,
                    AsList(GetMember(craft, "output")),
                    kind + "_output:" + craftId,
                    references);

                ScanItemListForPerk(
                    perkId,
                    AsList(GetMember(craft, "output_to_wgo")),
                    kind + "_output_to_wgo:" + craftId,
                    references);

                AddExpressionReferenceIfContains(
                    perkId,
                    GetMember(craft, "condition"),
                    kind + "_expr:" + craftId + ":condition",
                    references);
            }
        }

        private static void ScanItemListForPerk(
            string perkId,
            IList items,
            string owner,
            List<string> references)
        {
            if (items == null) return;

            for (int i = 0; i < items.Count; i++)
            {
                object item = items[i];
                if (item == null) continue;

                string self = GetExpression(GetMember(item, "self_chance"));
                string common = GetExpression(GetMember(item, "common_chance"));
                string min = GetExpression(GetMember(item, "min_value"));
                string max = GetExpression(GetMember(item, "max_value"));

                if (!ContainsOrdinal(self, perkId) &&
                    !ContainsOrdinal(common, perkId) &&
                    !ContainsOrdinal(min, perkId) &&
                    !ContainsOrdinal(max, perkId))
                {
                    continue;
                }

                references.Add(
                    owner +
                    " item=" + SafeString(GetMember(item, "id")) +
                    " value=" + SafeString(GetMember(item, "value")) +
                    " chance_group=" + SafeString(GetMember(item, "chance_group")) +
                    " self=" + Inline(self) +
                    " common=" + Inline(common) +
                    " min=" + Inline(min) +
                    " max=" + Inline(max));
            }
        }

        private static void AddExpressionReferenceIfContains(
            string perkId,
            object expression,
            string owner,
            List<string> references)
        {
            string raw = GetExpression(expression);
            if (ContainsOrdinal(raw, perkId))
                references.Add(owner + " expression=" + Inline(raw));
        }

        private static string GetPerkDescription(object perk, string perkId)
        {
            if (perk != null)
            {
                MethodInfo method = perk.GetType().GetMethod(
                    "GetDescriptionIfExists",
                    AnyInstance,
                    null,
                    Type.EmptyTypes,
                    null);
                if (method != null)
                {
                    try
                    {
                        return SafeString(method.Invoke(perk, null));
                    }
                    catch
                    {
                    }
                }
            }

            string key = perkId + "_d";
            string localized = Localize(key);
            return string.Equals(localized, key, StringComparison.Ordinal)
                ? string.Empty
                : localized;
        }

        private static string DescribeGameRes(object gameRes)
        {
            if (gameRes == null) return string.Empty;

            IList types = AsList(GetMember(gameRes, "Types"));
            MethodInfo get = gameRes.GetType().GetMethod(
                "Get",
                AnyInstance,
                null,
                new[] { typeof(string), typeof(float) },
                null);

            if (types == null || get == null) return string.Empty;

            List<string> parts = new List<string>();
            for (int i = 0; i < types.Count; i++)
            {
                string type = Convert.ToString(types[i], CultureInfo.InvariantCulture);
                object value = get.Invoke(gameRes, new object[] { type, 0f });
                parts.Add(type + "=" + Convert.ToString(value, CultureInfo.InvariantCulture));
            }

            return string.Join(";", parts.ToArray());
        }

        private static string DescribeItemIds(IList items)
        {
            if (items == null) return string.Empty;

            List<string> parts = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                object item = items[i];
                if (item == null) continue;

                string id = SafeString(GetMember(item, "id"));
                string self = GetExpression(GetMember(item, "self_chance"));
                string common = GetExpression(GetMember(item, "common_chance"));

                string part = id;
                if (!string.IsNullOrEmpty(self)) part += "{self=" + Inline(self) + "}";
                if (!string.IsNullOrEmpty(common)) part += "{common=" + Inline(common) + "}";
                parts.Add(part);
            }

            return string.Join(";", parts.ToArray());
        }

        private static string GetExpression(object expression)
        {
            if (expression == null) return string.Empty;

            MethodInfo raw = expression.GetType().GetMethod(
                "GetRawExpressionString",
                AnyInstance,
                null,
                Type.EmptyTypes,
                null);
            if (raw != null)
            {
                try
                {
                    return SafeString(raw.Invoke(expression, null));
                }
                catch
                {
                }
            }

            return SafeString(GetMember(expression, "_expression"));
        }

        private static Dictionary<string, object> IndexById(IList list)
        {
            Dictionary<string, object> result =
                new Dictionary<string, object>(StringComparer.Ordinal);

            if (list == null) return result;

            for (int i = 0; i < list.Count; i++)
            {
                object value = list[i];
                if (value == null) continue;

                string id = SafeString(GetMember(value, "id"));
                if (!string.IsNullOrEmpty(id) && !result.ContainsKey(id))
                    result.Add(id, value);
            }

            return result;
        }

        private static void BindLocalization()
        {
            if (_localize != null) return;

            Type gjl = FindType("GJL");
            if (gjl == null) return;

            MethodInfo[] methods = gjl.GetMethods(AnyStatic);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                ParameterInfo[] parameters = method.GetParameters();

                if (method.Name == "L" &&
                    method.ReturnType == typeof(string) &&
                    parameters.Length == 1 &&
                    parameters[0].ParameterType == typeof(string))
                {
                    _localize = method;
                    return;
                }
            }
        }

        private static string Localize(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (_localize == null) return key;

            try
            {
                return SafeString(_localize.Invoke(null, new object[] { key }));
            }
            catch
            {
                return key;
            }
        }

        private static bool ContainsString(IList list, string expected)
        {
            if (list == null) return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(
                    Convert.ToString(list[i], CultureInfo.InvariantCulture),
                    expected,
                    StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsOrdinal(string text, string value)
        {
            return !string.IsNullOrEmpty(text) &&
                   !string.IsNullOrEmpty(value) &&
                   text.IndexOf(value, StringComparison.Ordinal) >= 0;
        }

        private static IList AsList(object value)
        {
            return value as IList;
        }

        private static object GetMember(object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return null;

            Type type = obj.GetType();
            for (Type current = type; current != null; current = current.BaseType)
            {
                FieldInfo field = current.GetField(
                    name,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    try { return field.GetValue(obj); }
                    catch { return null; }
                }

                PropertyInfo property = current.GetProperty(
                    name,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);
                if (property != null &&
                    property.GetIndexParameters().Length == 0 &&
                    property.CanRead)
                {
                    try { return property.GetValue(obj, null); }
                    catch { return null; }
                }
            }

            return null;
        }

        private static object GetStaticMember(Type type, string name)
        {
            if (type == null) return null;

            FieldInfo field = type.GetField(name, AnyStatic);
            if (field != null)
            {
                try { return field.GetValue(null); }
                catch { return null; }
            }

            PropertyInfo property = type.GetProperty(name, AnyStatic);
            if (property != null &&
                property.GetIndexParameters().Length == 0 &&
                property.CanRead)
            {
                try { return property.GetValue(null, null); }
                catch { return null; }
            }

            return null;
        }

        private static Type FindType(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int a = 0; a < assemblies.Length; a++)
            {
                Assembly assembly = assemblies[a];

                try
                {
                    Type direct = assembly.GetType(name, false);
                    if (direct != null) return direct;

                    Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        if (type != null && type.Name == name)
                            return type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Type[] types = ex.Types;
                    if (types == null) continue;

                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        if (type != null && type.Name == name)
                            return type;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private static bool ToBool(object value)
        {
            try { return value != null && Convert.ToBoolean(value, CultureInfo.InvariantCulture); }
            catch { return false; }
        }

        private static float ToFloat(object value)
        {
            try { return value == null ? 0f : Convert.ToSingle(value, CultureInfo.InvariantCulture); }
            catch { return 0f; }
        }

        private static string SafeString(object value)
        {
            return value == null
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static string Bool(bool value)
        {
            return value ? "true" : "false";
        }

        private static string Inline(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Trim();
        }

        private static string Q(string value)
        {
            string safe = value ?? string.Empty;
            return "\"" + safe
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n") + "\"";
        }
    }
}
