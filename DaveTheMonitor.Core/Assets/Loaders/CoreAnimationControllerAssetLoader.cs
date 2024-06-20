using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreAnimationControllerAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            string json = File.ReadAllText(path);
            return new CoreAnimationControllerAsset(path, name, JsonAnimationController.FromJson(json, mod));
        }
    }
}
