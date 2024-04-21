using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using System;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class GlacierBiome : Biome
    {
        public override string Id => "Core.Glacier";
        public override string DisplayName => "Frozen Glaciers";

        public override void OnRegister(ICoreMod mod)
        {
            
        }

        public override bool HasFog => true;
        public override Color GetFogColor(ICoreWorld world) => new Color(40, 44, 54, 230);
        public override float GetFogDistance(ICoreWorld world) => 30;
        public override string ParticleEmitter => "Core.SnowEmitter";

        public override int GetGroundHeight(BiomeManager biomeManager, GlobalPoint3D p)
        {
            return p.Y;
        }

        public override BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            if (p.Y <= param.GroundHeight)
            {
                int depth = param.GroundHeight - p.Y;
                if (depth == 0)
                {
                    return new BlockAndAux(Block.Snow, 0);
                }
                else
                {
                    return new BlockAndAux(Block.Basalt, 0);
                }
            }
            else if (p.Y == param.GroundHeight + 1 && param.Random.RandomChance(0.5))
            {
                return new BlockAndAux(Block.SnowLayer, (byte)param.Random.Next(3));
            }
            return BlockAndAux.None;
        }

        public override void PlaceDecoration(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            if (param.Random.RandomChance(0.02))
            {
                PlaceDecoration("Core.SnowBoulderLarge1", p, param);
            }
            else if (param.Random.RandomChance(0.14))
            {
                PlaceDecoration("Core.SnowBoulder1", p, param);
            }
            else if (param.Random.RandomChance(0.26))
            {
                PlaceDecoration("Core.SnowBoulderSmall1", p, param);
            }
            else if (param.Random.RandomChance(0.5))
            {
                GenerateTree(p, param.Map, param.Random);
            }
        }

        private static void GenerateTree(GlobalPoint3D p, Map map, PcgRandom random)
        {
            int height = random.Next(16, 24);
            for (int i = 0; i < height; i++)
            {
                map.SetBlockData(p + new GlobalPoint3D(0, i, 0), (byte)Block.Wood, 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
            }
            map.SetBlockData(p + new GlobalPoint3D(0, height, 0), (byte)Block.SnowLayer, 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
            GlobalPoint3D dir = new GlobalPoint3D(1, 0, 0);
            GenerateBranches(map, random, p + dir, dir, height);
            dir = new GlobalPoint3D(-1, 0, 0);
            GenerateBranches(map, random, p + dir, dir, height);
            dir = new GlobalPoint3D(0, 0, 1);
            GenerateBranches(map, random, p + dir, dir, height);
            dir = new GlobalPoint3D(0, 0, -1);
            GenerateBranches(map, random, p + dir, dir, height);
        }

        private static void GenerateBranches(Map map, PcgRandom random, GlobalPoint3D p, GlobalPoint3D dir, int height)
        {
            int nextBranch = random.Next(2, 5);
            for (int i = 0; i < height - 1; i++)
            {
                if (i != nextBranch)
                {
                    continue;
                }

                float iNormalized = Math.Max(i / (float)height, 0);
                int length = random.RandomChance(1 - (iNormalized * 0.8f) - 0.2f) ? 2 : 1;
                //int length = (int)MathHelper.Lerp(3, 1, Math.Min(iNormalized + (float)world.Game.Random.NextDouble() * 0.2f, 1));
                GenerateBranch(map, random, p + new GlobalPoint3D(0, i, 0), dir, length);
                nextBranch += random.Next(3, 6);
            }
        }

        private static void GenerateBranch(Map map, PcgRandom random, GlobalPoint3D p, GlobalPoint3D dir, int length)
        {
            for (int i = 0; i < length; i++)
            {
                GlobalPoint3D t = p + (dir * i);
                byte aux = GetDirAux(dir);
                map.SetBlockData(t, (byte)Block.Wood, aux, UpdateBlockMethod.Generation, GamerID.Sys1, false);

                int snowHeight = length - i - 1;
                map.SetBlockData(t + GlobalPoint3D.Up, (byte)Block.SnowLayer, (byte)snowHeight, UpdateBlockMethod.Generation, GamerID.Sys1, false);
            }
        }

        private static byte GetDirAux(GlobalPoint3D dir)
        {
            if (dir.X != 0)
            {
                return 1;
            }
            else if (dir.Z != 0)
            {
                return 2;
            }
            return 0;
        }

        public GlacierBiome() : base(0f, 0.2f, Color.LightBlue)
        {
            
        }
    }
}
