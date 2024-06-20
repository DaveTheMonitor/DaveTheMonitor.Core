using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreActorAnimationAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            string json = File.ReadAllText(path);
            return new CoreActorAnimationAsset(path, name, JsonActorAnimation.FromJson(json));
        }
    }
}
