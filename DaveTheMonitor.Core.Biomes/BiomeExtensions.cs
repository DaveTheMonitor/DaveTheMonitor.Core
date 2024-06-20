using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Biomes
{
    public static class BiomeExtensions
    {
        public static BiomeRegistry BiomeRegistry(this ICoreGame game)
        {
            return game.GetData<BiomeGameData>().BiomeRegistry;
        }

        public static DecorationRegistry DecorationRegistry(this ICoreGame game)
        {
            return game.GetData<BiomeGameData>().DecorationRegistry;
        }

        public static BiomeManager BiomeManager(this ICoreWorld world)
        {
            return world.GetData<BiomeWorldData>().BiomeManager;
        }

        public static BiomeActorData BiomeData(this ICoreActor actor)
        {
            return actor.GetData<BiomeActorData>();
        }

        public static Biome CurrentBiome(this ICoreActor actor)
        {
            return actor.BiomeData().CurrentBiome;
        }
    }
}
