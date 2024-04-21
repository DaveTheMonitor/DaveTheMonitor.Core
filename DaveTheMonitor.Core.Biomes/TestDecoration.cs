using DaveTheMonitor.Core.API;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class TestDecoration : DecorationDefinition
    {
        public override string Id => "Core.Test";
        public override ITMMap Component => _component;
        public override GlobalPoint3D Origin => new GlobalPoint3D(10, 5, 8);
        private ITMMap _component;

        public override void OnRegister(ICoreMod mod)
        {
            _component = Game.ModManager.GetComponent(mod, "Test").TMMap;
        }
    }
}
