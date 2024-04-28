using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Wrappers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.MapChunkTM", "GenerateCore")]
    internal static class MapChunkTMGenerateCorePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(MapChunkTMGenerateCorePatch);
            Type gameInstance = AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance");
            MethodInfo method = AccessTools.Method(gameInstance, "get_CurrentBiome");

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = list.FindIndex(i => i.Calls(method));
            if (index == -1)
            {
                return list;
            }

            list.RemoveAt(index);
            list.InsertRange(index, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.Call(type, nameof(GetBiome))
            });

            return list;
        }

        public static BiomeType GetBiome(object gameInstance, object chunk)
        {
            ITMGame game = (ITMGame)gameInstance;
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return game.World.CurrentBiome;
            }

            MapChunk mapChunk = (MapChunk)chunk;
            ICoreWorld world = CorePlugin.Instance.Game.GetWorld(mapChunk.Region.Map);
            if (world == null)
            {
                return game.World.CurrentBiome;
            }

            return world.CurrentBiome;
        }
    }
}
