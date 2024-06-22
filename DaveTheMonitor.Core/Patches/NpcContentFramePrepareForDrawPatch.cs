using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Graphics.NpcContentFrame", "PrepareForDraw")]
    internal static class NpcContentFramePrepareForDrawPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            // we disable drawing actors with a custom model since our ActorRenderer
            // handles drawing them

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            list.ModifyFirst(i => i.Calls(AccessTools.Method("StudioForge.TotalMiner.Actor:get_IsDeadOrInactiveOrDisabled")), index =>
            {
                index++;
                Label label = (Label)list[index].operand;
                index++;
                list.InsertRange(index, new CodeInstruction[]
                {
                    CodeInstruction.LoadLocal(8),
                    CodeInstruction.Call(typeof(NpcContentFramePrepareForDrawPatch), nameof(HasModel)),
                    new CodeInstruction(OpCodes.Brtrue, label)
                });
            });

            return list;
        }

        public static bool HasModel(ITMActor actor)
        {
            if (!CorePlugin.IsValid)
            {
                return false;
            }

            ICoreActor coreActor = CorePlugin.Instance.Game.GetActor(actor);
            if (coreActor == null)
            {
                return false;
            }

            return coreActor.Model != null;
        }
    }
}
