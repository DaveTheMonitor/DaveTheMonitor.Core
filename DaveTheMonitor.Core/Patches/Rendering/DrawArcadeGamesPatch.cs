using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Wrappers;
using HarmonyLib;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "DrawArcadeGames")]
    internal static class DrawArcadeGamesPatch
    {
        private const WorldDrawStage _stage = WorldDrawStage.Arcade;
        private const WorldDrawOptions _option = WorldDrawOptions.Arcade;

        public static bool Prefix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if ((DrawGlobals.WorldDrawOptions & _option) > 0)
            {
                ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
                CorePlugin.Instance.Game.RunPreDrawWorldMap(_stage, new MapRenderer((DrawableGameObjectBase)__instance).Map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                return true;
            }

            return false;
        }

        public static void Postfix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return;
            }

            if ((DrawGlobals.WorldDrawOptions & _option) > 0)
            {
                ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
                CorePlugin.Instance.Game.RunPostDrawWorldMap(_stage, new MapRenderer((DrawableGameObjectBase)__instance).Map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
            }
        }
    }
}
