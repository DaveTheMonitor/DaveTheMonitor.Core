using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.Engine.Core.Node", "GetType", "System.String")]
    internal static class NodeGetTypePatch
    {
        private static AccessTools.FieldRef<object, IList> _activePlugins =
            AccessTools.FieldRefAccess<IList>(AccessTools.TypeByName("StudioForge.TotalMiner.ModManager"), "ActivePlugins");
        private static AccessTools.FieldRef<object, object> _pluginHost =
            AccessTools.FieldRefAccess<object>(AccessTools.TypeByName("StudioForge.TotalMiner.Mod"), "PluginHost");
        private static Func<object, Array> _pluginContexts =
            MethodHelper.CreateInvoker<Func<object, Array>>(AccessTools.Method("StudioForge.TotalMiner.PluginHost:get_PluginContexts"));
        private static Func<object, Assembly> _pluginAssembly =
            MethodHelper.CreateInvoker<Func<object, Assembly>>(AccessTools.Method("TotalMinerAPI.Plugin.PluginAssemblyContext:get_PluginAssembly"));

        public static bool Prefix(string typeName, ref Type __result)
        {
            // Fixes a bug in vanilla where custom behavior tree
            // nodes aren't loaded from the correct assembly.
            if (CorePlugin.Instance?.Game == null)
            {
                return true;
            }

            int i = typeName.IndexOf(',');
            if (i == -1)
            {
                __result = null;
                return false;
            }

            string assemblyName = typeName.Substring(i + 1).TrimStart();
            typeName = typeName.Substring(0, i);

            // Vanilla mods
            IList mods = _activePlugins();
            foreach (object obj in mods)
            {
                ITMMod mod = (ITMMod)obj;
                object pluginHost = _pluginHost(mod);
                Array contexts = _pluginContexts(pluginHost);
                foreach (object context in contexts)
                {
                    Assembly assembly = _pluginAssembly(context);
                    if (assembly.FullName == assemblyName)
                    {
                        Type type = assembly.GetType(typeName);
                        if (type != null)
                        {
                            __result = type;
                            return false;
                        }
                    }
                }
            }

            // Core mods
            if (assemblyName == typeof(CorePlugin).Assembly.FullName)
            {
                Type type = typeof(CorePlugin).Assembly.GetType(typeName);
                if (type != null)
                {
                    __result = type;
                    return false;
                }
            }

            foreach (ICoreMod mod in CorePlugin.Instance.Game.ModManager.GetAllActivePlugins())
            {
                if (mod.Assembly.FullName == assemblyName)
                {
                    Type type = mod.Assembly.GetType(typeName);
                    if (type != null)
                    {
                        __result = type;
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
