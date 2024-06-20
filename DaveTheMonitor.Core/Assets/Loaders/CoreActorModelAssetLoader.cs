using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;
using System.IO;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreActorModelAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            string json = File.ReadAllText(path);
            return new CoreActorModelAsset(path, name, ActorModel.FromJson(json, mod));
        }
    }
}
