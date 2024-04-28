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
    [Patch("StudioForge.TotalMiner.ChunkLoader", "UpdateCaveWorms")]
    internal static class ChunkLoaderUpdateCaveWormsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(ChunkLoaderUpdateCaveWormsPatch);
            Type gameInstance = AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance");
            MethodInfo method = AccessTools.Method(gameInstance, "get_CurrentBiome");

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = list.FindIndex(i => i.Calls(method));
            while (index != -1)
            {
                CodeInstruction oi = list[index];
                list.RemoveAt(index);
                list.InsertRange(index, new CodeInstruction[]
                {
                    CodeInstruction.LoadArgument(0),
                    CodeInstruction.Call(type, nameof(GetBiome))
                });
                oi.MoveBlocksTo(list[index]);

                index = list.FindIndex(i => i.Calls(method));
            }

            return list;
        }

        public static BiomeType GetBiome(object gameInstance, object loader)
        {
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return ((ITMGame)gameInstance).World.CurrentBiome;
            }

            ChunkLoader chunkLoader = new ChunkLoader((IThreadWorkItem)loader);

            ICoreWorld world = CorePlugin.Instance.Game.GetWorld((Map)chunkLoader.Map);
            if (world == null)
            {
                return ((ITMGame)gameInstance).World.CurrentBiome;
            }

            return world.CurrentBiome;
        }
    }
}
