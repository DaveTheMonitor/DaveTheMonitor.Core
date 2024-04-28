using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Wrappers;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.Graphics.SkyCurtain", "LoadGeometry",
        "System.Single",
        "System.Int32")]
    internal static class SkyCurtainLoadGeometryPatch
    {
        private static FieldInfo _field = AccessTools.Field(typeof(TerrainData), nameof(TerrainData.GroundBlock));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(SkyCurtainLoadGeometryPatch);

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = MapRendererUpdateCorePatch.FindSpaceWorldTest(list, out Label? label);
            if (index == -1)
            {
                return list;
            }

            index -= 2;
            list.RemoveRange(index, 5);
            list.InsertRange(index, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.Call(type, nameof(IsSpaceWorld)),
                new CodeInstruction(OpCodes.Brtrue, label.Value)
            });

            return list;
        }

        public static bool IsSpaceWorld(object instance, object skyCurtain)
        {
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return ((ITMGame)instance).World.Header.TerrainData.GroundBlock == Item.SpaceWorld;
            }

            SkyCurtain curtain = new SkyCurtain((IHasContent)skyCurtain);
            ICoreWorld world = CorePlugin.Instance.Game.GetWorld(curtain.Map);
            if (world == null)
            {
                return ((ITMGame)instance).World.Header.TerrainData.GroundBlock == Item.SpaceWorld;
            }

            return world.WorldOptions.FlatWorldType == FlatWorldType.Space;
        }
    }
}
