using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Assets.Loaders;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace DaveTheMonitor.Core
{
    internal sealed class ModManager : ICoreModManager
    {
        private struct AssetTypeInfo
        {
            public Type Type { get; set; }
            public string Path { get; set; }
            public string Filter { get; set; }
            public ICoreAssetLoader Loader { get; set; }

            public AssetTypeInfo(Type type, string path, string filter, ICoreAssetLoader loader)
            {
                Type = type;
                Path = path;
                Filter = filter;
                Loader = loader;
            }
        }
        public static string ContentPath => "CoreContent";
        public int ActiveMods => _activeMods.Count;
        private List<ICoreMod> _activeMods;
        private List<ICoreMod> _activePlugins;
        private IMapComponentLoader _mapLoader;
        private List<AssetTypeInfo> _assetTypeInfo;

        public bool IsModActive(string id)
        {
            return GetMod(id) != null;
        }

        public IEnumerable<ICoreMod> GetAllActiveMods()
        {
            return _activeMods;
        }

        public IEnumerable<ICoreMod> GetAllActivePlugins()
        {
            return _activePlugins;
        }

        public ICoreMod GetMod(string id)
        {
            foreach (ICoreMod mod in _activeMods)
            {
                if (mod.Id == id)
                {
                    return mod;
                }
            }
            return null;
        }

        public ICoreMod GetDefiningMod(ActorType actor)
        {
            ICoreMod defining = null;
            int highestOffset = 0;
            foreach (ICoreMod mod in _activeMods)
            {
                int offset = mod.TypeOffsets.ActorType;
                if (offset > highestOffset && offset <= (int)actor)
                {
                    highestOffset = offset;
                    defining = mod;
                }
            }
            return defining ?? CorePlugin.CoreMod;
        }

        public ICoreMod GetDefiningMod(Item item)
        {
            ICoreMod defining = null;
            int highestOffset = 0;
            foreach (ICoreMod mod in _activeMods)
            {
                int offset = mod.TypeOffsets.ActorType;
                if (offset > highestOffset && offset <= (int)item)
                {
                    highestOffset = offset;
                    defining = mod;
                }
            }
            return defining ?? CorePlugin.CoreMod;
        }

        public CoreModAsset LoadAsset(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadAsset(strippedName);
        }

        public T LoadAsset<T>(ICoreMod mod, string name) where T : CoreModAsset
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadAsset<T>(strippedName);
        }

        public Texture2D LoadTexture(ICoreMod mod, string name)
        {
            return LoadTexture(mod, name, true);
        }

        public Texture2D LoadTexture(ICoreMod mod, string name, bool returnMissingTexture)
        {
            // TODO: add TM.Texture for items and blocks?
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadTexture(strippedName, returnMissingTexture);
        }

        public ICoreMap LoadComponent(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadComponent(strippedName);
        }

        public ActorModel LoadActorModel(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadActorModel(strippedName);
        }

        public JsonActorAnimation LoadActorAnimation(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadActorAnimation(strippedName);
        }

        public JsonAnimationController LoadAnimationController(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadAnimationController(strippedName);
        }

        public SoundEffect LoadSound(ICoreMod mod, string name)
        {
            ICoreMod targetMod = GetTargetMod(name, out string strippedName) ?? mod;
            return targetMod.Content.LoadSound(strippedName);
        }

        private ICoreMod GetTargetMod(string name, out string strippedName)
        {
            int index = name.LastIndexOf('.');
            if (index == -1)
            {
                strippedName = name;
                return null;
            }

            string modId = name.Substring(0, index);
            ICoreMod mod = GetMod(modId);

            strippedName  = name.Substring(index + 1);
            return mod;
        }

        internal Assembly Resolve(AssemblyName name)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                if (mod.Assembly == null)
                {
                    continue;
                }

                AssemblyName target = mod.Assembly.GetName();
                if (name.Name == target.Name &&
                    name.CultureName == target.CultureName &&
                    name.Version.Major == target.Version.Major &&
                    name.Version <= target.Version)
                {
                    return mod.Assembly;
                }
            }
            return null;
        }

        public object Call(string modId, params object[] args)
        {
            return GetMod(modId)?.Plugin?.ModCall(args);
        }

        internal ICoreMod LoadMod(ITMMod tmMod, string path)
        {
#if DEBUG
            CorePlugin.Log($"Loading mod {tmMod.FullPath}: {path}");
#endif

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            string infoPath = Path.Combine(path, "Info.json");
            if (!File.Exists(infoPath))
            {
                infoPath = Path.Combine(path, "Info.jsonc");
            }
            if (!File.Exists(infoPath))
            {
                throw new FileNotFoundException("Mod must contain Info.json", infoPath);
            }

            ModInfo modInfo = JsonSerializer.Deserialize<ModInfo>(File.ReadAllText(infoPath), options);
            Mod mod = new Mod(path, modInfo, tmMod, ModType.Core, this);
            mod.Load(modInfo);
            if (mod.Content != null)
            {
                foreach (AssetTypeInfo info in _assetTypeInfo)
                {
                    mod.Content.AddAssetType(info.Type, info.Path, info.Filter, info.Loader);
                }
            }
            return mod;
        }

        internal bool TryLoadMod(ITMMod tmMod, string path, out ICoreMod mod)
        {
            try
            {
                mod = LoadMod(tmMod, path);
                return true;
            }
            catch
            {
                mod = null;
                return false;
            }
        }

        internal void EnableMod(ICoreMod mod)
        {
            if (!_activeMods.Contains(mod))
            {
                _activeMods.Add(mod);
                if (mod.Plugin != null)
                {
                    _activePlugins.Add(mod);
                }
            }
        }

        internal void EnableAll(IEnumerable<ICoreMod> mods)
        {
            foreach (Mod mod in mods)
            {
                EnableMod(mod);
            }
        }

        internal ICollection<ICoreMod> LoadAll(IEnumerable<ITMMod> mods)
        {
            List<ICoreMod> list = new List<ICoreMod>();
            foreach (ITMMod mod in mods)
            {
                if (_activeMods.FindIndex(m => m.Id == mod.ID) != -1)
                {
                    continue;
                }

                string corePath = Path.Combine(mod.FullPath, ContentPath);
                if (Directory.Exists(corePath))
                {
                    if (TryLoadMod(mod, corePath, out ICoreMod coreMod))
                    {
                        list.Add(coreMod);
                        continue;
                    }
                }

                ModInfo info = new ModInfo(mod.ID, mod.Version);
                list.Add(new Mod(mod.FullPath, info, mod, ModType.TM, this));
            }

            return list;
        }

        internal void UnloadAndDisableMod(ICoreMod mod)
        {
            mod.Unload();
            _activeMods.Remove(mod);
            _activePlugins.Remove(mod);
        }

        internal void UnloadAndDisableAll()
        {
            foreach (ICoreMod mod in _activeMods)
            {
                mod.Unload();
                // This is in case another mod adds non-disposable
                // ICoreMod implementations.
                if (mod is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _activeMods.Clear();
            _activePlugins.Clear();
        }

        public bool ModHandleInput(ICorePlayer player)
        {
            bool handled = false;
            foreach (ICoreMod mod in _activePlugins)
            {
                if (mod.Plugin.HandleInput(player))
                {
                    handled = true;
                }
            }
            return handled;
        }

        public void ModUpdate()
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.Update();
            }
        }

        public void ModUpdate(ICoreWorld world)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.Update(world);
            }
        }

        public void ModUpdate(ICoreActor actor)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.Update(actor);
            }
        }

        public void ModDraw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.Draw(player, virtualPlayer, vp);
            }
        }

        public void AddAssetLoader(Type assetType, string path, string filter, ICoreAssetLoader loader)
        {
            foreach (AssetTypeInfo info in _assetTypeInfo)
            {
                if (info.Type == assetType)
                {
                    throw new InvalidOperationException($"A loader for {assetType.FullName} has already been added.");
                }
            }

            _assetTypeInfo.Add(new AssetTypeInfo(assetType, path, filter, loader));
            foreach (ICoreMod mod in _activeMods)
            {
                mod.Content.AddAssetType(assetType, path, filter, loader);
            }
        }

        public ModManager(IMapComponentLoader componentLoader)
        {
            _activeMods = new List<ICoreMod>();
            _activePlugins = new List<ICoreMod>();
            _mapLoader = componentLoader;
            _assetTypeInfo = new List<AssetTypeInfo>()
            {
                new AssetTypeInfo(typeof(CoreTextureAsset), "Textures", "*.png", new CoreTextureAssetLoader()),
                new AssetTypeInfo(typeof(CoreMapAsset), "Components", "*.com", new CoreComponentAssetLoader(_mapLoader)),
                new AssetTypeInfo(typeof(CoreActorModelAsset), "Models", "*.json", new CoreActorModelAssetLoader()),
                new AssetTypeInfo(typeof(CoreActorAnimationAsset), "Animations", "*.json", new CoreActorAnimationAssetLoader()),
                new AssetTypeInfo(typeof(CoreAnimationControllerAsset), "AnimationControllers", "*.json", new CoreAnimationControllerAssetLoader()),
                new AssetTypeInfo(typeof(CoreSoundAsset), "Sounds", ".mp3|.wav", new CoreSoundAssetLoader()),
            };
        }
    }
}
