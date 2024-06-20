using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
