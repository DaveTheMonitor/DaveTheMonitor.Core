using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches.MapRenderer
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "DrawPlayerClipboard")]
    internal static class DrawPlayerClipboardPatch
    {
        public static bool Prefix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if ((DrawGlobals.WorldDrawOptions & WorldDrawOptions.Clipboard) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
