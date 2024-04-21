using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BiomeGameData : ICoreData<ICoreGame>
    {
        public BiomeRegistry BiomeRegistry { get; private set; }
        public DecorationRegistry DecorationRegistry { get; private set; }
        public bool ShouldSave => false;

        public void Initialize(ICoreGame game)
        {
            
        }

        public void SetRegisters(BiomeRegistry biomes, DecorationRegistry decorations)
        {
            BiomeRegistry = biomes;
            DecorationRegistry = decorations;
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {

        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
