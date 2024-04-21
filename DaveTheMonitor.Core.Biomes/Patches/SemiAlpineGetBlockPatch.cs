using DaveTheMonitor.Core.Biomes;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using System;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.SemiAlpineBiome", "GetBlock")]
    internal static class SemiAlpineGetBlockPatch
    {
        public static void SetDefaultGroundAndBelowBlock(object instance, int x, int y, int z, int grassyStoneChance)
        {
            throw new NotImplementedException();
        }

        public static bool Prefix(
            GlobalPoint3D p,
            object ___map,
            ushort ___seaLevel,
            int ___groundHeight,
            PcgRandom ___random,
            ref byte ___getBlockResultBlockID,
            ref byte ___getBlockResultAux,
            ref byte ___getBlockResultLight)
        {
            BiomeManager biomeManager = CorePlugin.Instance.Game.GetWorld((Map)___map).BiomeManager();
            Biome biome = biomeManager.GetBiome(p.X, p.Z, out Biome border, out float blend);

            Biome mainBiome;
            if (border != null)
            {
                mainBiome = ___random.RandomChance(blend) ? border : biome;
            }
            else
            {
                mainBiome = biome;
            }

            if (mainBiome is DefaultBiome or OceanBiome)
            {
                return true;
            }

            Map map = (Map)___map;
            BiomeGenerationParams param = new BiomeGenerationParams(___seaLevel, ___groundHeight, map, ___random);
            int rockDepth = mainBiome.GetRockDepth(biomeManager, p, param);
            if (p.Y <= ___groundHeight - rockDepth)
            {
                return true;
            }

            BlockAndAux block = mainBiome.GetBlock(biomeManager, p, param);
            ___getBlockResultBlockID = (byte)block.Block;
            ___getBlockResultAux = block.Aux;
            if (block.Block != 0)
            {
                ___getBlockResultLight = 0;
            }
            else
            {
                ___getBlockResultLight = (byte)(map.SunLight.SunLight << 4);
            }
            return false;
        }
    }
}
