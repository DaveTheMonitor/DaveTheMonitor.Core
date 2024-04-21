using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class DefaultBiome : Biome
    {
        public override string Id => "Core.Default";
        public override string DisplayName => "None";

        public override void OnRegister(ICoreMod mod)
        {
            
        }

        public override BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            return BlockAndAux.None;
        }

        public DefaultBiome(Color color) : base(0.5f, 0.5f, color)
        {
            
        }
    }
}
