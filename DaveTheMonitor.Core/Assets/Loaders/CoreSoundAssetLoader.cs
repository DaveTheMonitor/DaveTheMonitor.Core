using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework.Audio;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreSoundAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            return new CoreSoundAsset(path, name, SoundEffect.FromFile(path));
        }
    }
}
