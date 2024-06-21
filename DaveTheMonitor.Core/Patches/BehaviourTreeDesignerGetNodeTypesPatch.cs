using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Screens2.BehaviourTreeDesigner", "GetNodeTypes")]
    internal static class BehaviourTreeDesignerGetNodeTypesPatch
    {
        private static Action<object, Assembly, Type, List<Type>> _loadNodeTypes =
            MethodHelper.CreateInvoker<Action<object, Assembly, Type, List<Type>>>(AccessTools.Method("StudioForge.TotalMiner.Screens2.BehaviourTreeDesigner:LoadNodeTypes"));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            // By default, the behavior tree designer searches mod assemblies loaded by
            // the game for nodes.
            // Because the Core Mod loads assemblies outside of TM's mod loader, we
            // patch the method to search those as well so Core mods can add new
            // behavior nodes.

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            list.InsertFirst(i => i.Calls(AccessTools.Method("StudioForge.TotalMiner.Screens2.BehaviourTreeDesigner:LoadNodeTypes")),
                1, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.LoadLocal(1),
                CodeInstruction.LoadLocal(0),
                CodeInstruction.Call(typeof(BehaviourTreeDesignerGetNodeTypesPatch), nameof(LoadCoreNodes))
            });

            return list;
        }

        public static void LoadCoreNodes(object instance, Type baseType, List<Type> result)
        {
            if (CorePlugin.Instance?.Game == null)
            {
                return;
            }

            _loadNodeTypes(instance, typeof(CorePlugin).Assembly, baseType, result);
            foreach (ICoreMod mod in CorePlugin.Instance.Game.ModManager.GetAllActivePlugins())
            {
                _loadNodeTypes(instance, mod.Assembly, baseType, result);
            }
        }
    }
}
