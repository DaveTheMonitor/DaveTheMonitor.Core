using DaveTheMonitor.Core.Biomes;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.BlockWorld;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.SemiAlpineBiome", "GetPlaneData")]
    internal static class SemiAlpineGetPlaneDataPatch
    {
        public static bool Prefix(int x, int z, ref int __result, object ___map, bool ___useImageMap)
        {
            if (___useImageMap)
            {
                return true;
            }
            BiomeManager biomeManager = CorePlugin.Instance.Game.GetWorld((Map)___map).BiomeManager();
            __result = biomeManager.GetGroundHeight(x, z);
            return false;
        }
    }
}
