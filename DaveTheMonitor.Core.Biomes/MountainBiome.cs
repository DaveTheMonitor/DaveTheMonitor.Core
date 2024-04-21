using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class MountainBiome : Biome
    {
        public override string Id => "Mountains";
        public override string DisplayName => "Mountains";

        public override void OnRegister(ICoreMod mod)
        {
            
        }

        public override BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            if (p.Y <= param.GroundHeight)
            {
                return new BlockAndAux(Block.Basalt, 0);
            }
            else
            {
                return BlockAndAux.None;
            }
        }

        public override int GetGroundHeight(BiomeManager biomeManager, GlobalPoint3D p)
        {
            return p.Y + 60;
        }

        public MountainBiome() : base(0.4f, -0.3f, Color.Gray)
        {
            
        }
    }
}
