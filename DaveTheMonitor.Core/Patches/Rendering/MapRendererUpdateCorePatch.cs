using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Wrappers;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "UpdateCore")]
    internal static class MapRendererUpdateCorePatch
    {
        private static FieldInfo _field = AccessTools.Field(typeof(TerrainData), nameof(TerrainData.GroundBlock));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(MapRendererUpdateCorePatch);

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = FindSpaceWorldTest(list, out Label? label);
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
                new CodeInstruction(OpCodes.Brfalse, label.Value)
            });

            return list;
        }

        internal static int FindSpaceWorldTest(List<CodeInstruction> list, out Label? label)
        {
            for (int i = 0; i < list.Count; i++)
            {
                CodeInstruction i1 = list[i];
                if (!i1.LoadsField(_field))
                {
                    continue;
                }
                CodeInstruction i2 = list[i + 1];
                if (!i2.LoadsConstant((int)Item.SpaceWorld))
                {
                    continue;
                }
                CodeInstruction i3 = list[i + 2];
                i3.Branches(out label);
                return i;
            }
            label = null;
            return -1;
        }

        public static bool IsSpaceWorld(object instance, object mapRenderer)
        {
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return ((ITMGame)instance).World.Header.TerrainData.GroundBlock == Item.SpaceWorld;
            }

            MapRenderer renderer = new MapRenderer((DrawableGameObjectBase)mapRenderer);
            ICoreWorld world = CorePlugin.Instance.Game.GetWorld((Map)renderer.Map);
            if (world == null)
            {
                return ((ITMGame)instance).World.Header.TerrainData.GroundBlock == Item.SpaceWorld;
            }

            return world.WorldOptions.FlatWorldType == FlatWorldType.Space;
        }
    }
}
