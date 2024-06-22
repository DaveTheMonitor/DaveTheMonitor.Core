using DaveTheMonitor.Core.Plugin;
using HarmonyLib;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.ModManager", "HotLoadMods")]
    internal static class HotLoadPatch
    {
        public static void Prefix()
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            CorePlugin.Instance.HotLoad();
        }
    }
}
