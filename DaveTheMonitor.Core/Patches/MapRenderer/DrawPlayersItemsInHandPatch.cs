using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches.MapRenderer
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "DrawPlayersItemsInHand")]
    internal static class DrawPlayersItemsInHandPatch
    {
        public static bool Prefix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            // This method draws other players' and NPCs' held items.
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if ((DrawGlobals.WorldDrawOptions & WorldDrawOptions.Avatars) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
