using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "Draw",
        "StudioForge.TotalMiner.Player",
        "StudioForge.TotalMiner.Player")]
    internal static class MapRendererDrawPatch
    {
        public static bool Prefix(object __instance, ITMMap ___map, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if (DrawGlobals.WorldDrawOptions == WorldDrawOptions.None)
            {
                return false;
            }

            ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
            CorePlugin.Instance.Game.RunPreDrawWorldMap(WorldDrawStage.Draw, ___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
            return true;
        }

        public static void Postfix(object __instance, ITMMap ___map, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return;
            }

            ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
            CorePlugin.Instance.Game.RunPostDrawWorldMap(WorldDrawStage.Draw, ___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
        }
    }
}
