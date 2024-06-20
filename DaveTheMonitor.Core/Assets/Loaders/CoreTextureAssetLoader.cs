using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreTextureAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
            return new CoreTextureAsset(path, name, Texture2D.FromFile(CoreGlobals.GraphicsDevice, path));
        }
    }
}
