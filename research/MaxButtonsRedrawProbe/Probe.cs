// SPDX-License-Identifier: MPL-2.0
// Research only. Observe original calls; never skip, replace results or suppress exceptions.
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
[assembly: AssemblyVersion("0.1.1.0")]
[assembly: AssemblyFileVersion("0.1.1.0")]
namespace NikichMods.MaxButtonsRedrawResearch
{
    [BepInPlugin(Id, "Max Buttons Redraw Observer", "0.1.1")]
    public sealed class Probe : BaseUnityPlugin
    {
        private const string Id = "nikich.gyk.maxbuttonsredrawprobe";
        private static ManualLogSource _log;
        private static Type _plugin, _plannerControls;
        private static int _inputs, _windows, _errors, _hides;
        private static bool _craftSeen;
        private Harmony _harmony;
        [ThreadStatic] private static int _input;
        private sealed class State { internal string Before; internal int Input; }
        private void Start()
        {
            _log = Logger;
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "MaxButtonsRedux");
                _plugin = assembly.GetType("MaxButtonsRedux.Plugin", true);
                Type crafting = assembly.GetType("MaxButtonsRedux.MaxButtonCrafting", true);
                var planner = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "CraftingPlanner");
                _plannerControls = planner == null ? null : planner.GetType("CraftingPlanner.PlannerRowControls");
                _harmony = new Harmony(Id);
                Bind(AccessTools.Method(_plugin, "HandleCraftGuiInput", new[] {typeof(Rewired.Player)}), "BeforeInput", "AfterInput");
                Bind(AccessTools.Method(crafting, "SetAmount", new[] {typeof(CraftItemGUI), typeof(int)}), "BeforeAmount", "AfterAmount");
                Bind(AccessTools.Method(typeof(CraftItemGUI), "Redraw", Type.EmptyTypes), "BeforeRedraw", "AfterRedraw");
                Bind(AccessTools.Method(typeof(CraftGUI), "Hide", new[] {typeof(bool)}), "BeforeHide", "AfterHide");
                BaseGUI.on_window_opened += Window;
                BaseGUI.on_window_closed += Window;
                Emit("ready version=0.1.1 max_sha256=" + Digest(assembly.Location) + " planner=" + (planner == null ? "absent" : planner.GetName().Version.ToString()) +
                     " planner_sha256=" + (planner == null ? "absent" : Digest(planner.Location)));
                Owners(AccessTools.Method(typeof(CraftItemGUI), "Redraw", Type.EmptyTypes));
                Owners(AccessTools.Method(typeof(CraftItemGUI), "OnOver", Type.EmptyTypes));
                Owners(AccessTools.Method(typeof(CraftGUI), "Hide", new[] {typeof(bool)}));
                Owners(AccessTools.Method(typeof(CraftGUI), "OnClosePressed", Type.EmptyTypes));
            }
            catch (Exception ex) { Emit("observer_error phase=install error=" + ex); Stop(); }
        }
        private void Bind(MethodInfo method, string prefix, string finalizer)
        {
            if (method == null || method.ReturnType != typeof(void)) throw new InvalidOperationException("Missing void seam");
            _harmony.Patch(method, prefix: new HarmonyMethod(typeof(Probe), prefix) {priority = Priority.First},
                finalizer: new HarmonyMethod(typeof(Probe), finalizer) {priority = Priority.Last});
        }
        private static void Owners(MethodInfo method)
        {
            var info = Harmony.GetPatchInfo(method);
            if (info == null) return;
            foreach (var group in new[] {info.Prefixes, info.Postfixes, info.Transpilers, info.Finalizers})
                foreach (var patch in group)
                    Emit("patch method=" + method.DeclaringType.Name + "." + method.Name + " owner=" + patch.owner + " handler=" + patch.PatchMethod.DeclaringType.FullName + "." + patch.PatchMethod.Name);
        }
        private static string Digest(string path)
        { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant(); }
        private static void Emit(string text) { try { if (_log != null) _log.LogInfo("MAX_REDRAW event=" + text); } catch { } }
        private static object Field(object value, string name)
        { return value == null ? null : AccessTools.Field(value.GetType(), name).GetValue(value); }
        private static string Presence(object value)
        {
            if (ReferenceEquals(value, null)) return "clr_null";
            var unity = value as UnityEngine.Object;
            return !ReferenceEquals(unity, null) && unity == null ? "unity_null" : "present";
        }
        private static string Fields(CraftItemGUI row)
        {
            string result = " row=" + Presence(row);
            if (ReferenceEquals(row, null)) return result;
            foreach (string name in new[] {"craft_definition", "craft_gui_interface", "_craft_gui", "item_gui", "ingredients_table", "ingredients_table_universal", "label_name", "label_descr", "selection_frame", "amount_buttons", "_ingredients", "_ingredient_buttons", "_perk_icons"})
            {
                object value = Field(row, name);
                var array = value as Array;
                result += " " + name + "=" + (array == null ? Presence(value) : "length" + array.Length);
            }
            var list = Field(row, "_possible_crafts") as IList;
            int index = (int)Field(row, "_current_craft_index");
            var definition = list != null && index >= 0 && index < list.Count ? list[index] as CraftDefinition : null;
            result += " possible=" + (list == null ? "null" : list.Count.ToString()) + " index=" + index + " current=" + (definition == null ? "null" : definition.id);
            var cell = row.item_gui;
            if (cell != null) result += " quality_icon=" + Presence(Field(cell, "quality_icon")) + " x1=" + Presence(Field(cell, "x1"));
            if (row != null)
            {
                Transform t = row.transform;
                string path = t.name;
                for (int i = 0; t.parent != null && i < 9; i++) { t = t.parent; path = t.name + "/" + path; }
                result += " active=" + row.gameObject.activeInHierarchy + " path=" + path.Replace(' ', '_') + " planner_controls=" + (_plannerControls == null ? "unavailable" : Presence(row.GetComponent(_plannerControls)));
            }
            return result;
        }
        private static string Context()
        {
            string result = " input=" + _input;
            foreach (string name in new[] {"_craftGuiOpen", "_itemCountGuiOpen", "_unsafeInteraction"}) result += " " + name + "=" + AccessTools.Field(_plugin, name).GetValue(null);
            CraftGUI craft = GUIElements.me != null ? GUIElements.me.craft : null;
            result += " craft_top=" + (craft != null && craft.is_shown_and_top) + " windows=";
            foreach (BaseGUI window in BaseGUI.opened_windows) if (window != null) result += window.GetType().Name + ",";
            return result;
        }
        private static string Snapshot(CraftItemGUI row)
        { try { return Context() + Fields(row); } catch (Exception ex) { return "snapshot_error=" + ex.GetType().Name; } }
        private static void BeforeInput(out int __state)
        {
            // This method is called every frame while the cached flag is set.
            // No reflection, snapshots or logs until a real SetAmount/error occurs.
            __state = _input; _input = _inputs + 1;
        }
        private static Exception AfterInput(Exception __exception, int __state)
        {
            try
            {
                if (__exception != null && _errors++ < 12)
                    Emit("input_exception" + Snapshot(AccessTools.Field(_plugin, "_craftItemGui").GetValue(null) as CraftItemGUI) + " exception=" + __exception);
            }
            catch { }
            finally { _input = __state; }
            return __exception;
        }
        private static void BeforeAmount(CraftItemGUI __0, int __1, out State __state)
        {
            __state = ++_inputs <= 64 ? new State {Input = _input, Before = Snapshot(__0)} : null;
            if (__state != null) Emit("set_amount amount=" + __1 + __state.Before);
        }
        private static Exception AfterAmount(Exception __exception, State __state)
        {
            if (__exception != null && _errors++ < 12) Emit("amount_exception" + (__state == null ? "" : __state.Before) + " exception=" + __exception);
            return __exception;
        }
        private static void BeforeRedraw(CraftItemGUI __instance, out State __state)
        { __state = _input > 0 && _inputs <= 64 ? new State {Input = _input, Before = Snapshot(__instance)} : null; }
        private static Exception AfterRedraw(Exception __exception, State __state)
        {
            if (__state != null && (__exception != null || __state.Input <= 12)) Emit("redraw_result input=" + __state.Input + " outcome=" + (__exception == null ? "normal" : "exception") + __state.Before + (__exception == null ? "" : " exception=" + __exception));
            return __exception;
        }
        private static string CachedSnapshot()
        { try { return Snapshot(AccessTools.Field(_plugin, "_craftItemGui").GetValue(null) as CraftItemGUI); } catch { return "snapshot_unavailable"; } }
        private static bool ObserveWindow(bool craft, bool technology, bool top)
        {
            if (craft && top) _craftSeen = true;
            if (!_craftSeen || (!craft && !technology) || _windows >= 48) return false;
            _windows++;
            return true;
        }
        private static void BeforeHide(out string __state)
        {
            __state = null;
            try
            {
                CraftGUI craft = GUIElements.me != null ? GUIElements.me.craft : null;
                if (craft != null && craft.is_shown_and_top) _craftSeen = true;
                if (_craftSeen) __state = CachedSnapshot();
            }
            catch { }
        }
        private static Exception AfterHide(Exception __exception, string __state)
        { try { if (__state != null && _hides++ < 24) Emit("craft_hide before=" + __state + " after=" + CachedSnapshot()); } catch { } return __exception; }
        private static void Window(BaseGUI gui)
        { try { if (gui != null && ObserveWindow(gui is CraftGUI, gui is TechTreeGUI, gui.is_shown_and_top)) Emit("window_change type=" + gui.GetType().Name + CachedSnapshot()); } catch { } }
        private void Stop()
        {
            BaseGUI.on_window_opened -= Window; BaseGUI.on_window_closed -= Window;
            try { if (_harmony != null) _harmony.UnpatchSelf(); } catch { }
            _harmony = null;
        }
        private void OnDestroy() { Stop(); }
    }
}
