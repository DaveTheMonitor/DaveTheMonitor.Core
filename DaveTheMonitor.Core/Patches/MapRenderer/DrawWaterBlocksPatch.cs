﻿using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches.MapRenderer
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "DrawWaterBlocks")]
    internal static class DrawWaterBlocksPatch
    {
        public static bool Prefix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if ((DrawGlobals.WorldDrawOptions & WorldDrawOptions.TransparentBlocks) > 0)
            {
                return true;
            }

            return false;
        }
    }
}