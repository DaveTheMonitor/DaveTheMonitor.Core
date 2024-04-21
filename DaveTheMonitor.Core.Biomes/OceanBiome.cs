using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class OceanBiome : Biome
    {
        public override string Id => "Core.Ocean";
        public override string DisplayName => "Ocean";

        public override void OnRegister(ICoreMod mod)
        {
            
        }

        public override BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            return BlockAndAux.None;
        }

        public OceanBiome() : base(0.5f, 1f, Color.Blue)
        {
            
        }
    }
}
