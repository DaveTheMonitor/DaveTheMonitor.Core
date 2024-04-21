using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BiomeRegistry : DefinitionRegistry<Biome>
    {
        protected override void OnRegister(Biome definition)
        {
            
        }

        public BiomeRegistry(ICoreGame game) : base(game, null)
        {

        }
    }
}
