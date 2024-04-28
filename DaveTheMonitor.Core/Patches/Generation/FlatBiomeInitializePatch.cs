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
    [Patch("StudioForge.TotalMiner.FlatBiome", "Initialize",
        "StudioForge.TotalMiner.GameInstance",
        "StudioForge.TotalMiner.MapTM",
        "StudioForge.TotalMiner.BiomeParams")]
    internal static class FlatBiomeInitializePatch
    {
        public static void Postfix(ITMGame instance, Map map, BiomeParams biomeParams, ref Item ___groundID, ref byte ___groundBlockID)
        {
            if (CorePlugin.Instance?.Game == null || !CorePlugin.Instance.Game.HasMultipleWorlds)
            {
                return;
            }

            ICoreWorld world = CorePlugin.Instance.Game.GetWorld(map);
            if (world == null)
            {
                return;
            }

            ___groundBlockID = (byte)world.WorldOptions.GroundBlock.Value;
            ___groundID = world.WorldOptions.FlatWorldType switch
            {
                FlatWorldType.Natural => Item.NaturalWorld,
                FlatWorldType.Sky => Item.SkyWorld,
                FlatWorldType.Space => Item.SpaceWorld,
                _ => (Item)___groundBlockID
            };
        }
    }
}
