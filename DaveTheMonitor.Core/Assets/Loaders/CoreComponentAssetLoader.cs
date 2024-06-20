using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreComponentAssetLoader : ICoreAssetLoader
    {
        private IMapComponentLoader _loader;

        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            return new CoreMapAsset(path, name, new CoreMap(_loader.LoadComponent(path)));
        }

        public CoreComponentAssetLoader(IMapComponentLoader loader)
        {
            _loader = loader;
        }
    }
}
