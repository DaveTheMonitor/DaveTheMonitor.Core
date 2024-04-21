using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace DaveTheMonitor.Core.Biomes
{
    public struct BiomeGenerationParams
    {
        public int SeaLevel { get; private set; }
        public int GroundHeight { get; private set; }
        public Map Map { get; private set; }
        public PcgRandom Random { get; private set; }

        public BiomeGenerationParams(int seaLevel, int groundHeight, Map map, PcgRandom random)
        {
            SeaLevel = seaLevel;
            GroundHeight = groundHeight;
            Map = map;
            Random = random;
        }
    }
}
