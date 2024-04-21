using DaveTheMonitor.Core.Biomes;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.BiomeBase", "TreeDecorationCore")]
    internal static class TreeDecorationCorePatch
    {
        public static bool Prefix(int maxY,
            object ___map,
            ushort ___seaLevel,
            int ___groundHeight,
            PcgRandom ___random,
            int ___chunkSizeX,
            int ___chunkSizeY,
            int ___chunkSizeZ,
            GlobalPoint3D ___chunkGlobalOffset,
            MapChunk ___chunk,
            BiomeParams ___biomeParams)
        {
            BiomeManager biomeManager = CorePlugin.Instance.Game.GetWorld((Map)___map).BiomeManager();
            Point3D p = new Point3D();
            p.X = ___random.Next(___chunkSizeX);
            p.Z = ___random.Next(___chunkSizeZ);
            GlobalPoint3D gp = new GlobalPoint3D();
            gp.X = p.X + ___chunkGlobalOffset.X;
            gp.Z = p.Z + ___chunkGlobalOffset.Z;
            Biome biome = biomeManager.GetBiome(gp.X, gp.Z);
            
            if (biome is DefaultBiome or OceanBiome)
            {
                return true;
            }

            Map map = (Map)___map;
            p.Y = map.ChunkSize.Y - 1;
            gp.Y = ___chunk.GlobalOffset.Y + p.Y;
            Block top = (Block)___chunk.GetBlockID(new Point3D(p.X, p.Y, p.Z));
            Block block = top;
            bool canPlace = false;
            int groundHeight = biomeManager.GetGroundHeight(gp.X, gp.Z);

            while (!canPlace && p.Y >= 0)
            {
                p.Y--;
                gp.Y--;
                top = block;
                block = (Block)___chunk.GetBlockID(p);
                canPlace = gp.Y == groundHeight + 1;
            }

            if (canPlace)
            {
                gp.Y = ___chunk.GlobalOffset.Y + p.Y;
                BiomeGenerationParams param = new BiomeGenerationParams(___seaLevel, groundHeight, map, ___random);
                biome.PlaceDecoration(biomeManager, gp, param);
            }
            return false;
        }
    }
}
