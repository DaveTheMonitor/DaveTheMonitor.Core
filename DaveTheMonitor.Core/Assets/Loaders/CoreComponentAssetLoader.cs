using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
