using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Particles
{
    public sealed class ParticleGameData : ICoreData<ICoreGame>
    {
        public ParticleRegistry ParticleRegistry { get; private set; }
        public bool ShouldSave => false;

        public void Initialize(ICoreGame game)
        {
            
        }

        public void SetRegistry(ParticleRegistry registry)
        {
            ParticleRegistry = registry;
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            
        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
