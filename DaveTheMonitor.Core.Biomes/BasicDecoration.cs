using DaveTheMonitor.Core.API;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BasicDecoration : DecorationDefinition
    {
        public override string Id => _id;
        public override ITMMap Component => _component;
        public override GlobalPoint3D Origin => _origin;
        public override Map.CopyType CopyType => _copyType;
        public override bool CanRotate => _canRotate;
        private string _id;
        private string _comId;
        private GlobalPoint3D _origin;
        private Map.CopyType _copyType;
        private bool _canRotate;
        private ITMMap _component;

        public override void OnRegister(ICoreMod mod)
        {
            _component = Game.ModManager.LoadComponent(mod, _comId).TMMap;
        }

        public BasicDecoration(string id, string component, bool canRotate, GlobalPoint3D origin, Map.CopyType copyType)
        {
            _id = id;
            _comId = component;
            _canRotate = canRotate;
            _origin = origin;
            _copyType = copyType;
        }
    }
}
