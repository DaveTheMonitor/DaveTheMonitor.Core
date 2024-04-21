using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Effects
{
    public sealed class EffectGameData : ICoreData<ICoreGame>
    {
        public ActorEffectRegistry EffectRegistry { get; private set; }
        public bool ShouldSave => false;

        public void Initialize(ICoreGame game)
        {
            
        }

        public void SetRegister(ActorEffectRegistry registry)
        {
            EffectRegistry = registry;
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {

        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
