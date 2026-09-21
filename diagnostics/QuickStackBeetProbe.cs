using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace GKQuickStackBeetProbe
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("pandamodding.gyk.quickstack", BepInDependency.DependencyFlags.HardDependency)]
    public sealed class QuickStackBeetProbePlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "nikich.gyk.diagnostics.quickstackbeetprobe";
        public const string PluginName = "GK Quick Stack Beet Slice Probe (Diagnostic)";
        public const string PluginVersion = "0.1.0";

        private const string QuickStackGuid = "pandamodding.gyk.quickstack";
        private const string TargetItemId = "meal:beet_slice";

        private static readonly BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static ManualLogSource Log;
        private static PluginInfo QuickStackInfo;
        private static Assembly QuickStackAssembly;
        private static readonly List<MethodInfo> QuickStackMethods = new List<MethodInfo>();
        private static Harmony HarmonyInstance;
        private static Type MainGameType;
        private static int AttemptCounter;
        private static string LastCanFingerprint;
        private static string LastCountFingerprint;

        private sealed class AttemptState
        {
            internal int Number;
            internal object Chest;
        }

        private sealed class Context
        {
            internal object Chest;
            internal object ChestData;
            internal IList ChestInventory;
            internal object Player;
            internal object PlayerData;
            internal IList PlayerInventory;
            internal object PlayerItem;
            internal object ChestItem;
        }

        private void Awake()
        {
            Log = Logger;

            if (!Chainloader.PluginInfos.TryGetValue(QuickStackGuid, out QuickStackInfo) ||
                QuickStackInfo == null || QuickStackInfo.Instance == null)
            {
                Logger.LogError("Quick Stack plugin was not available through BepInEx PluginInfos. Probe disabled.");
                return;
            }

            QuickStackAssembly = QuickStackInfo.Instance.GetType().Assembly;
            MainGameType = FindType("MainGame");
            DiscoverQuickStackMethods();

            Logger.LogInfo(
                PluginName + " " + PluginVersion + " loaded. target_item=" + TargetItemId +
                ", quickstack_assembly=" + QuickStackAssembly.GetName().Name +
                ", quickstack_version=" + QuickStackAssembly.GetName().Version + ".");

            LogRelevantSignatures();

            HarmonyInstance = new Harmony(PluginGuid);
            int tryPatched = PatchTryQuickStack();
            int canPatched = PatchCanQuickStack();
            int countPatched = PatchCountPotentialMove();

            Logger.LogInfo(
                "Probe hooks installed: TryQuickStack=" + tryPatched +
                ", CanQuickStack=" + canPatched +
                ", CountPotentialMove=" + countPatched +
                ". Diagnostics are emitted only for states involving " + TargetItemId + ".");

            if (tryPatched == 0)
                Logger.LogWarning("No compatible TryQuickStack method was found. CanQuickStack/CountPotentialMove probes may still capture the failure state.");
        }

        private void OnDestroy()
        {
            if (HarmonyInstance != null)
                HarmonyInstance.UnpatchSelf();
        }

        private static int PatchTryQuickStack()
        {
            MethodInfo prefix = AccessTools.Method(typeof(QuickStackBeetProbePlugin), nameof(TryQuickStackPrefix));
            MethodInfo postfix = AccessTools.Method(typeof(QuickStackBeetProbePlugin), nameof(TryQuickStackPostfix));
            MethodInfo finalizer = AccessTools.Method(typeof(QuickStackBeetProbePlugin), nameof(TryQuickStackFinalizer));
            int patched = 0;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != "TryQuickStack" || method.GetParameters().Length != 1) continue;
                HarmonyInstance.Patch(method, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix), finalizer: new HarmonyMethod(finalizer));
                patched++;
            }

            return patched;
        }

        private static int PatchCanQuickStack()
        {
            MethodInfo postfix = AccessTools.Method(typeof(QuickStackBeetProbePlugin), nameof(CanQuickStackPostfix));
            int patched = 0;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != "CanQuickStack" || method.ReturnType != typeof(bool) || method.GetParameters().Length != 1) continue;
                HarmonyInstance.Patch(method, null, new HarmonyMethod(postfix));
                patched++;
            }

            return patched;
        }

        private static int PatchCountPotentialMove()
        {
            MethodInfo postfix = AccessTools.Method(typeof(QuickStackBeetProbePlugin), nameof(CountPotentialMovePostfix));
            int patched = 0;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != "CountPotentialMove" || method.ReturnType != typeof(int) || method.GetParameters().Length != 1) continue;
                HarmonyInstance.Patch(method, null, new HarmonyMethod(postfix));
                patched++;
            }

            return patched;
        }

        private static void TryQuickStackPrefix(object[] __args, MethodBase __originalMethod, out AttemptState __state)
        {
            object chest = __args != null && __args.Length > 0 ? __args[0] : null;
            __state = new AttemptState { Number = ++AttemptCounter, Chest = chest };

            try
            {
                if (HasTargetOnEitherSide(chest))
                    LogFullAnalysis(__state.Number, "BEFORE TryQuickStack", chest, true);
            }
            catch (Exception ex)
            {
                Log.LogError("[QS BEET PROBE #" + __state.Number + "] prefix diagnostic failed: " + ex);
            }
        }

        private static void TryQuickStackPostfix(object[] __args, MethodBase __originalMethod, AttemptState __state)
        {
            if (__state == null) return;
            object chest = __state.Chest ?? (__args != null && __args.Length > 0 ? __args[0] : null);

            try
            {
                if (HasTargetOnEitherSide(chest))
                    LogFullAnalysis(__state.Number, "AFTER TryQuickStack", chest, false);
            }
            catch (Exception ex)
            {
                Log.LogError("[QS BEET PROBE #" + __state.Number + "] postfix diagnostic failed: " + ex);
            }
        }

        private static Exception TryQuickStackFinalizer(Exception __exception, AttemptState __state)
        {
            if (__exception != null)
            {
                int number = __state == null ? 0 : __state.Number;
                Log.LogError("[QS BEET PROBE #" + number + "] TryQuickStack threw " + __exception.GetType().Name + ": " + __exception.Message);
            }
            return __exception;
        }

        private static void CanQuickStackPostfix(object[] __args, bool __result)
        {
            object chest = __args != null && __args.Length > 0 ? __args[0] : null;
            if (__result || !HasTargetOnBothSides(chest)) return;

            try
            {
                string fingerprint = BuildFingerprint(chest, "can=False");
                if (fingerprint == LastCanFingerprint) return;
                LastCanFingerprint = fingerprint;
                LogSnapshot("CanQuickStack=False", chest);
            }
            catch (Exception ex)
            {
                Log.LogWarning("[QS BEET PROBE] CanQuickStack snapshot failed: " + ex.Message);
            }
        }

        private static void CountPotentialMovePostfix(object[] __args, int __result)
        {
            object chest = __args != null && __args.Length > 0 ? __args[0] : null;
            if (__result != 0 || !HasTargetOnBothSides(chest)) return;

            try
            {
                string fingerprint = BuildFingerprint(chest, "count=0");
                if (fingerprint == LastCountFingerprint) return;
                LastCountFingerprint = fingerprint;
                LogSnapshot("CountPotentialMove=0", chest);
            }
            catch (Exception ex)
            {
                Log.LogWarning("[QS BEET PROBE] CountPotentialMove snapshot failed: " + ex.Message);
            }
        }

        private static void LogFullAnalysis(int number, string phase, object chest, bool invokeQuickStackQueries)
        {
            Context context = BuildContext(chest);
            Log.LogWarning("[QS BEET PROBE #" + number + "] " + phase + " | " + BuildSummary(context));
            LogItemDetails(number, context);

            if (!invokeQuickStackQueries) return;

            LogOneArgQuery(number, "CanQuickStack", chest);
            LogOneArgQuery(number, "CountPotentialMove", chest);
            LogPlayerPredicate(number, context, "ShouldSkipPlayerItem");
            LogBoundHelper(number, context, "HasMatchingItemInChest");
            LogBoundHelper(number, context, "CountHowMuchCouldBeStacked");
            LogBoundHelper(number, context, "CountHowMuchCouldFitIntoEmptySlotsAfterStacking");
        }

        private static void LogSnapshot(string reason, object chest)
        {
            Context context = BuildContext(chest);
            Log.LogWarning("[QS BEET PROBE] " + reason + " | " + BuildSummary(context));
            LogItemDetails(0, context);
        }

        private static Context BuildContext(object chest)
        {
            Context context = new Context();
            context.Chest = chest;
            context.ChestData = GetMember(chest, "data");
            context.ChestInventory = GetInventory(chest);

            object mainGame = GetStaticMember(MainGameType, "me");
            context.Player = GetMember(mainGame, "player");
            context.PlayerData = GetMember(context.Player, "data");
            context.PlayerInventory = GetInventory(context.Player);
            context.PlayerItem = FirstItemById(context.PlayerInventory, TargetItemId);
            context.ChestItem = FirstItemById(context.ChestInventory, TargetItemId);
            return context;
        }

        private static string BuildSummary(Context context)
        {
            object chestDef = GetMember(context.Chest, "obj_def") ?? GetMember(context.Chest, "definition");
            object capacity = GetMember(chestDef, "inventory_size") ?? GetMember(context.ChestData, "inventory_size");
            string chestId = Convert.ToString(GetMember(context.Chest, "obj_id") ?? GetMember(context.Chest, "id"));

            return
                "chest_id=" + Safe(chestId) +
                " chest_type=" + TypeName(context.Chest) +
                " chest_items=" + Count(context.ChestInventory) +
                " chest_capacity=" + Safe(capacity) +
                " player_items=" + Count(context.PlayerInventory) +
                " player_beet_stacks=" + CountItemsById(context.PlayerInventory, TargetItemId) +
                " chest_beet_stacks=" + CountItemsById(context.ChestInventory, TargetItemId) +
                " player_beet_total=" + SumValues(context.PlayerInventory, TargetItemId) +
                " chest_beet_total=" + SumValues(context.ChestInventory, TargetItemId) + ".";
        }

        private static void LogItemDetails(int number, Context context)
        {
            List<object> playerItems = ItemsById(context.PlayerInventory, TargetItemId);
            List<object> chestItems = ItemsById(context.ChestInventory, TargetItemId);
            string prefix = number > 0 ? "[QS BEET PROBE #" + number + "] " : "[QS BEET PROBE] ";

            for (int i = 0; i < playerItems.Count; i++)
                Log.LogWarning(prefix + "PLAYER_BEET[" + i + "] " + DescribeItem(playerItems[i]));

            for (int i = 0; i < chestItems.Count; i++)
                Log.LogWarning(prefix + "CHEST_BEET[" + i + "] " + DescribeItem(chestItems[i]));

            for (int p = 0; p < playerItems.Count; p++)
            {
                for (int c = 0; c < chestItems.Count; c++)
                {
                    object pDef = GetMember(playerItems[p], "definition");
                    object cDef = GetMember(chestItems[c], "definition");
                    string pId = Convert.ToString(GetMember(playerItems[p], "id"));
                    string cId = Convert.ToString(GetMember(chestItems[c], "id"));
                    Log.LogWarning(
                        prefix + "PAIR p=" + p + " c=" + c +
                        " id_equal=" + string.Equals(pId, cId, StringComparison.Ordinal) +
                        " definition_same_ref=" + ReferenceEquals(pDef, cDef) +
                        " definition_type_equal=" + (pDef != null && cDef != null && pDef.GetType() == cDef.GetType()) + ".");
                }
            }
        }

        private static string DescribeItem(object item)
        {
            if (item == null) return "<null>";
            object definition = GetMember(item, "definition");
            object objDef = GetMember(item, "obj_def");
            object stackCount = GetMember(definition, "stack_count");
            object defId =
                GetMember(definition, "id") ??
                GetMember(definition, "ID") ??
                GetMember(definition, "_id") ??
                GetMember(definition, "item_id");

            return
                "item_ref=" + RefId(item) +
                " item_type=" + TypeName(item) +
                " id=" + Safe(GetMember(item, "id")) +
                " value=" + Safe(GetMember(item, "value")) +
                " definition_ref=" + RefId(definition) +
                " definition_type=" + TypeName(definition) +
                " definition_id=" + Safe(defId) +
                " stack_count=" + Safe(stackCount) +
                " obj_def_ref=" + RefId(objDef) +
                " is_equipped=" + Safe(GetMember(item, "is_equipped")) +
                " control_enabled=" + Safe(GetMember(item, "control_enabled")) +
                " is_bag=" + Safe(GetMember(item, "is_bag")) + ".";
        }

        private static void LogOneArgQuery(int number, string methodName, object arg)
        {
            bool found = false;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != methodName || method.GetParameters().Length != 1) continue;
                ParameterInfo parameter = method.GetParameters()[0];
                if (arg == null || !parameter.ParameterType.IsInstanceOfType(arg)) continue;
                found = true;

                object target;
                if (!TryResolveTarget(method, out target))
                {
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " not invoked: instance target unresolved for " + MethodSignature(method));
                    continue;
                }

                try
                {
                    object result = method.Invoke(target, new[] { arg });
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " => " + Safe(result) + " via " + MethodSignature(method));
                }
                catch (Exception ex)
                {
                    Exception actual = Unwrap(ex);
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " diagnostic invoke failed: " + actual.GetType().Name + ": " + actual.Message);
                }
            }

            if (!found)
                Log.LogWarning("[QS BEET PROBE #" + number + "] no compatible one-argument " + methodName + " method found.");
        }

        private static void LogPlayerPredicate(int number, Context context, string methodName)
        {
            List<object> playerItems = ItemsById(context.PlayerInventory, TargetItemId);
            if (playerItems.Count == 0) return;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != methodName || method.GetParameters().Length != 1) continue;
                object playerItem = playerItems[0];
                if (!method.GetParameters()[0].ParameterType.IsInstanceOfType(playerItem)) continue;

                object target;
                if (!TryResolveTarget(method, out target)) continue;

                try
                {
                    object result = method.Invoke(target, new[] { playerItem });
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   "(PLAYER_BEET[0]) => " + Safe(result) + " via " + MethodSignature(method));
                }
                catch (Exception ex)
                {
                    Exception actual = Unwrap(ex);
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " diagnostic invoke failed: " + actual.GetType().Name + ": " + actual.Message);
                }
            }
        }

        private static void LogBoundHelper(int number, Context context, string methodName)
        {
            bool anyNamed = false;

            foreach (MethodInfo method in QuickStackMethods)
            {
                if (method.Name != methodName) continue;
                anyNamed = true;

                object[] args;
                string reason;
                if (!TryBindArguments(method, context, out args, out reason))
                {
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " not invoked: " + reason + " | " + MethodSignature(method));
                    continue;
                }

                object target;
                if (!TryResolveTarget(method, out target))
                {
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " not invoked: instance target unresolved | " + MethodSignature(method));
                    continue;
                }

                try
                {
                    object result = method.Invoke(target, args);
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " => " + Safe(result) + " args=" + DescribeArgs(args) +
                                   " via " + MethodSignature(method));
                }
                catch (Exception ex)
                {
                    Exception actual = Unwrap(ex);
                    Log.LogWarning("[QS BEET PROBE #" + number + "] " + methodName +
                                   " diagnostic invoke failed: " + actual.GetType().Name + ": " + actual.Message +
                                   " | " + MethodSignature(method));
                }
            }

            if (!anyNamed)
                Log.LogWarning("[QS BEET PROBE #" + number + "] helper " + methodName + " was not present in Quick Stack assembly.");
        }

        private static bool TryBindArguments(MethodInfo method, Context context, out object[] args, out string reason)
        {
            ParameterInfo[] parameters = method.GetParameters();
            args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                object value = ResolveArgument(parameter, context);

                if (value == null && parameter.ParameterType.IsValueType &&
                    Nullable.GetUnderlyingType(parameter.ParameterType) == null)
                {
                    reason = "unresolved value-type parameter " + parameter.Name + ":" + parameter.ParameterType.FullName;
                    return false;
                }

                if (value == null && !parameter.IsOptional)
                {
                    reason = "unresolved parameter " + parameter.Name + ":" + parameter.ParameterType.FullName;
                    return false;
                }

                args[i] = value ?? Type.Missing;
            }

            reason = null;
            return true;
        }

        private static object ResolveArgument(ParameterInfo parameter, Context context)
        {
            string name = (parameter.Name ?? string.Empty).ToLowerInvariant();
            Type type = parameter.ParameterType;

            if (name.Contains("player") && name.Contains("item") && Fits(type, context.PlayerItem)) return context.PlayerItem;
            if (name.Contains("chest") && name.Contains("item") && Fits(type, context.ChestItem)) return context.ChestItem;
            if (name.Contains("player") && name.Contains("invent") && Fits(type, context.PlayerInventory)) return context.PlayerInventory;
            if (name.Contains("chest") && name.Contains("invent") && Fits(type, context.ChestInventory)) return context.ChestInventory;
            if (name.Contains("chest") && name.Contains("data") && Fits(type, context.ChestData)) return context.ChestData;
            if (name.Contains("player") && name.Contains("data") && Fits(type, context.PlayerData)) return context.PlayerData;
            if (name.Contains("chest") && Fits(type, context.Chest)) return context.Chest;
            if (name.Contains("player") && Fits(type, context.Player)) return context.Player;
            if (name.Contains("item") && Fits(type, context.PlayerItem)) return context.PlayerItem;

            object[] candidates =
            {
                context.Chest,
                context.ChestData,
                context.ChestInventory,
                context.Player,
                context.PlayerData,
                context.PlayerInventory,
                context.PlayerItem,
                context.ChestItem
            };

            object unique = null;
            int matches = 0;

            for (int i = 0; i < candidates.Length; i++)
            {
                if (!Fits(type, candidates[i])) continue;
                unique = candidates[i];
                matches++;
            }

            return matches == 1 ? unique : null;
        }

        private static bool Fits(Type type, object value)
        {
            return value != null && type.IsInstanceOfType(value);
        }

        private static string DescribeArgs(object[] args)
        {
            if (args == null) return "[]";
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) builder.Append(", ");
                object arg = args[i];
                builder.Append(TypeName(arg)).Append("#").Append(RefId(arg));
            }

            return builder.Append("]").ToString();
        }

        private static bool TryResolveTarget(MethodInfo method, out object target)
        {
            target = null;
            if (method.IsStatic) return true;

            if (QuickStackInfo != null && QuickStackInfo.Instance != null &&
                method.DeclaringType.IsInstanceOfType(QuickStackInfo.Instance))
            {
                target = QuickStackInfo.Instance;
                return true;
            }

            Type type = method.DeclaringType;

            foreach (FieldInfo field in type.GetFields(AnyStatic))
            {
                if (!type.IsAssignableFrom(field.FieldType)) continue;
                try
                {
                    object value = field.GetValue(null);
                    if (value == null) continue;
                    target = value;
                    return true;
                }
                catch { }
            }

            foreach (PropertyInfo property in type.GetProperties(AnyStatic))
            {
                if (!type.IsAssignableFrom(property.PropertyType) ||
                    property.GetIndexParameters().Length != 0) continue;

                try
                {
                    object value = property.GetValue(null, null);
                    if (value == null) continue;
                    target = value;
                    return true;
                }
                catch { }
            }

            return false;
        }

        private static void DiscoverQuickStackMethods()
        {
            QuickStackMethods.Clear();

            foreach (Type type in GetLoadableTypes(QuickStackAssembly))
            {
                if (type == null) continue;

                MethodInfo[] methods;
                try
                {
                    methods = type.GetMethods(
                        BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.DeclaredOnly);
                }
                catch
                {
                    continue;
                }

                QuickStackMethods.AddRange(methods);
            }
        }

        private static void LogRelevantSignatures()
        {
            string[] names =
            {
                "TryQuickStack",
                "CanQuickStack",
                "CountPotentialMove",
                "ShouldSkipPlayerItem",
                "HasMatchingItemInChest",
                "CountHowMuchCouldBeStacked",
                "CountHowMuchCouldFitIntoEmptySlotsAfterStacking"
            };

            for (int n = 0; n < names.Length; n++)
            {
                bool found = false;

                foreach (MethodInfo method in QuickStackMethods)
                {
                    if (method.Name != names[n]) continue;
                    found = true;
                    Log.LogInfo("Quick Stack signature: " + MethodSignature(method));
                }

                if (!found)
                    Log.LogInfo("Quick Stack signature: " + names[n] + " <not found>");
            }
        }

        private static string MethodSignature(MethodInfo method)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(method.IsStatic ? "static " : "instance ")
                .Append(method.ReturnType.FullName)
                .Append(" ")
                .Append(method.DeclaringType == null ? "<unknown>" : method.DeclaringType.FullName)
                .Append(".")
                .Append(method.Name)
                .Append("(");

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0) builder.Append(", ");
                builder.Append(parameters[i].ParameterType.FullName)
                    .Append(" ")
                    .Append(parameters[i].Name);
            }

            return builder.Append(")").ToString();
        }

        private static bool HasTargetOnEitherSide(object chest)
        {
            Context context = BuildContext(chest);
            return context.PlayerItem != null || context.ChestItem != null;
        }

        private static bool HasTargetOnBothSides(object chest)
        {
            Context context = BuildContext(chest);
            return context.PlayerItem != null && context.ChestItem != null;
        }

        private static string BuildFingerprint(object chest, string prefix)
        {
            Context context = BuildContext(chest);

            return prefix +
                   "|" + RefId(chest) +
                   "|p=" + SumValues(context.PlayerInventory, TargetItemId) +
                   "|c=" + SumValues(context.ChestInventory, TargetItemId) +
                   "|pc=" + Count(context.PlayerInventory) +
                   "|cc=" + Count(context.ChestInventory);
        }

        private static IList GetInventory(object owner)
        {
            if (owner == null) return null;

            IList direct = GetMember(owner, "inventory") as IList;
            if (direct != null) return direct;

            object data = GetMember(owner, "data");
            IList dataList = GetMember(data, "inventory") as IList;
            if (dataList != null) return dataList;

            return null;
        }

        private static object FirstItemById(IList inventory, string id)
        {
            if (inventory == null) return null;

            for (int i = 0; i < inventory.Count; i++)
            {
                object item = inventory[i];

                if (item != null &&
                    string.Equals(Convert.ToString(GetMember(item, "id")), id, StringComparison.Ordinal))
                    return item;
            }

            return null;
        }

        private static List<object> ItemsById(IList inventory, string id)
        {
            List<object> result = new List<object>();
            if (inventory == null) return result;

            for (int i = 0; i < inventory.Count; i++)
            {
                object item = inventory[i];

                if (item != null &&
                    string.Equals(Convert.ToString(GetMember(item, "id")), id, StringComparison.Ordinal))
                    result.Add(item);
            }

            return result;
        }

        private static int CountItemsById(IList inventory, string id)
        {
            return ItemsById(inventory, id).Count;
        }

        private static int SumValues(IList inventory, string id)
        {
            int total = 0;
            if (inventory == null) return 0;

            for (int i = 0; i < inventory.Count; i++)
            {
                object item = inventory[i];

                if (item == null ||
                    !string.Equals(Convert.ToString(GetMember(item, "id")), id, StringComparison.Ordinal))
                    continue;

                object value = GetMember(item, "value");
                try { total += Convert.ToInt32(value); }
                catch { }
            }

            return total;
        }

        private static int Count(IList list)
        {
            return list == null ? -1 : list.Count;
        }

        private static object GetMember(object obj, string name)
        {
            if (obj == null) return null;
            Type type = obj.GetType();

            FieldInfo field = type.GetField(name, AnyInstance);
            if (field != null)
            {
                try { return field.GetValue(obj); }
                catch { return null; }
            }

            PropertyInfo property = type.GetProperty(name, AnyInstance);
            if (property != null && property.GetIndexParameters().Length == 0 && property.CanRead)
            {
                try { return property.GetValue(obj, null); }
                catch { return null; }
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
            if (property != null && property.GetIndexParameters().Length == 0 && property.CanRead)
            {
                try { return property.GetValue(null, null); }
                catch { return null; }
            }

            return null;
        }

        private static Type FindType(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type direct = assembly.GetType(name, false);
                    if (direct != null) return direct;

                    foreach (Type type in assembly.GetTypes())
                        if (type != null && type.Name == name) return type;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    if (ex.Types == null) continue;

                    for (int i = 0; i < ex.Types.Length; i++)
                        if (ex.Types[i] != null && ex.Types[i].Name == name) return ex.Types[i];
                }
                catch { }
            }

            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try { return assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { return ex.Types ?? new Type[0]; }
            catch { return new Type[0]; }
        }

        private static string TypeName(object value)
        {
            return value == null ? "<null>" : value.GetType().FullName;
        }

        private static string RefId(object value)
        {
            return value == null ? "null" : RuntimeHelpers.GetHashCode(value).ToString("x8");
        }

        private static string Safe(object value)
        {
            return value == null ? "<null>" : Convert.ToString(value);
        }

        private static Exception Unwrap(Exception ex)
        {
            TargetInvocationException target = ex as TargetInvocationException;
            return target != null && target.InnerException != null ? target.InnerException : ex;
        }
    }
}
