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
    [Patch("StudioForge.TotalMiner.ChunkDecoratorPending", "Initialize")]
    internal static class ChunkDecoratorPendingPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(ChunkDecoratorPendingPatch);
            Type gameInstance = AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance");
            FieldInfo field = AccessTools.Field(typeof(SaveMapHead), nameof(SaveMapHead.BiomeParams));

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = list.FindIndex(i => i.LoadsField(field));
            if (index == -1)
            {
                return list;
            }

            index--;
            list.RemoveAt(index);
            list.RemoveAt(index);
            list.InsertRange(index, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(4),
                CodeInstruction.Call(type, nameof(GetBiomeParams))
            });

            return list;
        }

        public static BiomeParams GetBiomeParams(object gameInstance, object chunk)
        {
            ITMGame game = (ITMGame)gameInstance;
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return game.World.Header.BiomeParams;
            }

            MapChunk mapChunk = (MapChunk)chunk;
            ICoreWorld world = CorePlugin.Instance.Game.GetWorld(mapChunk.Region.Map);
            if (world == null)
            {
                return game.World.Header.BiomeParams;
            }

            return world.WorldOptions.BiomeParams;
        }
    }
}
