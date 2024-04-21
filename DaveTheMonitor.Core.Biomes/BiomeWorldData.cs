using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BiomeWorldData : ICoreData<ICoreWorld>
    {
        public BiomeManager BiomeManager { get; private set; }
        public bool ShouldSave => false;

        public void Initialize(ICoreWorld world)
        {
            BiomeManager = new BiomeManager(world);
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {

        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
