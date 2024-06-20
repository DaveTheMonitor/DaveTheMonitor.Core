using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Assets.Loaders;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Used to load mod content files (eg. textures)
    /// </summary>
    public sealed class ModContentManager : IDisposable
    {
        private struct AssetInfo
        {
            public bool IsLoaded => Asset != null;
            public string Path { get; set; }
            public CoreModAsset Asset { get; set; }
            public Type Type { get; set; }

            public AssetInfo(string path, Type assetType)
            {
                Path = path;
                Type = assetType;
            }
        }

        /// <summary>
        /// This mod's MonoGame content. Used to load MonoGame content such as <see cref="Effect"/>s (shaders).
        /// </summary>
        public ContentManager MGContent { get; private set; }

        /// <summary>
        /// This mod content's root directory.
        /// </summary>
        public string RootDirectory { get; private set; }
        private Dictionary<string, AssetInfo> _assets;
        private Dictionary<Type, ICoreAssetLoader> _loaders;
        private ICoreModManager _modManager;
        private ICoreMod _mod;
        private bool _disposedValue;

        /// <summary>
        /// Loads a texture asset. Texture assets must be in the "Textures" folder.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The texture loaded, or null if it doesn't exist.</returns>
        public Texture2D LoadTexture(string name)
        {
            return LoadAsset<CoreTextureAsset>("Textures/" + name)?.Texture;
        }

        /// <summary>
        /// Loads a texture asset. Texture assets must be in the "Textures" folder.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <param name="returnMissingTexture">If true, this method will return a 16x missing texture if the texture is not found.</param>
        /// <returns>The texture loaded, or null or a missing texture if it doesn't exist.</returns>
        public Texture2D LoadTexture(string name, bool returnMissingTexture)
        {
            Texture2D texture = LoadAsset<CoreTextureAsset>("Textures/" + name)?.Texture;
            if (texture == null && returnMissingTexture)
            {
                texture = CoreGlobalData.MissingTexture16;
            }
            return texture;
        }

        /// <summary>
        /// Loads a component (map) asset. Component assets must be in the "Components" folder.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The component loaded, or null if it doesn't exist.</returns>
        public ICoreMap LoadComponent(string name)
        {
            return LoadAsset<CoreMapAsset>("Components/" + name)?.Map;
        }

        /// <summary>
        /// Loads an actor model asset. Actor model assets must be in the "Models" folder.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The actor model loaded, or null if it doesn't exist.</returns>
        public ActorModel LoadActorModel(string name)
        {
            return LoadAsset<CoreActorModelAsset>("Models/" + name)?.Model;
        }

        /// <summary>
        /// Loads an actor animation asset. Actor animation assets must be in the "Animations" folder.
        /// This asset shouldn't be used for animation directly. Convert it to a <see cref="ActorAnimation"/> first using <see cref="JsonActorAnimation.ToActorAnimation(ActorModel)"/>.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The actor animation loaded, or null if it doesn't exist.</returns>
        public JsonActorAnimation LoadActorAnimation(string name)
        {
            return LoadAsset<CoreActorAnimationAsset>("Animations/" + name)?.Animation;
        }

        /// <summary>
        /// Loads an animation controller asset. Actor animation controller assets must be in the "AnimationControllers" folder.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <returns>The actor animation loaded, or null if it doesn't exist.</returns>
        public JsonAnimationController LoadAnimationController(string name)
        {
            return LoadAsset<CoreAnimationControllerAsset>("AnimationControllers/" + name)?.AnimationController;
        }

        /// <summary>
        /// Loads an asset from this mod.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The asset, or null if it doesn't exist or couldn't be loaded.</returns>
        public T LoadAsset<T>(string name) where T : CoreModAsset
        {
            return LoadAsset(name) as T;
        }

        /// <summary>
        /// Loads an asset from this mod.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The asset, or null if it doesn't exist or couldn't be loaded.</returns>
        public CoreModAsset LoadAsset(string name)
        {
            if (!_assets.TryGetValue(name, out AssetInfo info))
            {
                return null;
            }

            if (info.IsLoaded)
            {
                return info.Asset;
            }

            if (!_loaders.TryGetValue(info.Type, out ICoreAssetLoader loader))
            {
                return null;
            }

            info.Asset = loader.Load(info.Path, name, _mod);
            _assets[name] = info;
            return info.Asset;
        }

        /// <summary>
        /// Unloads the specified asset.
        /// </summary>
        /// <param name="name">The name of the asset to unload.</param>
        public void UnloadAsset(string name)
        {
            if (!_assets.TryGetValue(name, out AssetInfo info) || !info.IsLoaded)
            {
                return;
            }

            info.Asset.Dispose();
            info.Asset = null;
            _assets[name] = info;
        }

        /// <summary>
        /// Unloads the specified asset.
        /// </summary>
        /// <param name="asset">The asset to unload.</param>
        public void UnloadAsset(CoreModAsset asset)
        {
            if (!_assets.TryGetValue(asset.Name, out AssetInfo info) || info.Asset != asset)
            {
                return;
            }

            info.Asset.Dispose();
            info.Asset = null;
            _assets[asset.Name] = info;
        }

        /// <summary>
        /// Adds a new content type.
        /// </summary>
        /// <param name="assetType">The type of the asset.</param>
        /// <param name="path">The path this content is in, eg "Particles"</param>
        /// <param name="filter">The file filter.</param>
        /// <param name="loader">The loader.</param>
        public void AddAssetType(Type assetType, string path, string filter, ICoreAssetLoader loader)
        {
            AddLoader(assetType, loader);

            string fullPath = Path.Combine(RootDirectory, path);
            if (!Directory.Exists(fullPath))
            {
                return;
            }

            EachFile(fullPath, filter, (path, name) =>
            {
                _assets.Add(name, new AssetInfo(path, assetType));
            });
        }

        private void EachFile(string path, string filter, Action<string, string> action)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string name = Path.GetRelativePath(RootDirectory, file);
                name = name.Substring(0, name.LastIndexOf('.')).Replace('\\', '/');
                action(file, name);
            }
        }

        private void AddLoader(Type assetType, ICoreAssetLoader loader)
        {
            if (_loaders.ContainsKey(assetType))
            {
                throw new InvalidOperationException($"A loader for {assetType.FullName} has already been added.");
            }
            _loaders.Add(assetType, loader);
        }

        private ContentManager InitMGContent(string path)
        {
            if (Directory.Exists(path))
            {
                return new ContentManager(CoreGlobals.Content.ServiceProvider, path);
            }
            return null;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MGContent?.Dispose();
                    foreach (AssetInfo info in _assets.Values)
                    {
                        if (info.IsLoaded)
                        {
                            info.Asset.Dispose();
                        }
                    }
                }

                MGContent = null;
                _assets = null;
                _loaders = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ModContentManager(string path, ICoreModManager modManager, ICoreMod mod)
        {
            _assets = new Dictionary<string, AssetInfo>();
            _loaders = new Dictionary<Type, ICoreAssetLoader>();
            _modManager = modManager;
            _mod = mod;
            RootDirectory = path;
            // Allow MonoGame content to be loaded from both the MGContent and Content paths.
            MGContent = InitMGContent(Path.Combine(path, "MGContent")) ?? InitMGContent(Path.Combine(path, "Content"));
        }
    }
}
