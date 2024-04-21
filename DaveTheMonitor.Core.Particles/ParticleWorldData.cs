using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Particles
{
    public sealed class ParticleWorldData : ICoreData<ICoreWorld>
    {
        public ParticleManager ParticleManager { get; private set; }
        public bool ShouldSave => false;

        public void Initialize(ICoreWorld world)
        {
            int max = 8129 * 2;
            ParticleManager = new ParticleManager(world, max, ParticlesPlugin.Instance._particleShader, world.Game.ParticleRegistry());
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            
        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
