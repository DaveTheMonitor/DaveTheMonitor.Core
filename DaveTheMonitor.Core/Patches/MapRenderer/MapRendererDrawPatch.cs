using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Patches.MapRenderer
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "Draw",
        "StudioForge.TotalMiner.Player",
        "StudioForge.TotalMiner.Player")]
    internal static class MapRendererDrawPatch
    {
        public static void Prefix(object __instance, ITMMap ___map, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return;
            }

            ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
            if (CorePlugin.Instance.Game.ModManager is ModManager modManager)
            {
                modManager.ModPreDrawWorldMap(___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                return;
            }
            else
            {
                IEnumerable<ICoreMod> plugins = CorePlugin.Instance.Game.ModManager.GetAllActivePlugins();
                foreach (ICoreMod mod in plugins)
                {
                    mod.Plugin.PreDrawWorldMap(___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                }
            }
        }

        public static void Postfix(object __instance, ITMMap ___map, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return;
            }

            ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
            if (CorePlugin.Instance.Game.ModManager is ModManager modManager)
            {
                modManager.ModPostDrawWorldMap(___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                return;
            }
            else
            {
                IEnumerable<ICoreMod> plugins = CorePlugin.Instance.Game.ModManager.GetAllActivePlugins();
                foreach (ICoreMod mod in plugins)
                {
                    mod.Plugin.PostDrawWorldMap(___map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                }
            }
        }
    }
}
