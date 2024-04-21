using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core
{
    internal sealed class Mod : ICoreMod, IDisposable
    {
        public string FullPath { get; private set; }
        public string Id { get; private set; }
        public ModType Type { get; private set; }
        public ICorePlugin Plugin { get; private set; }
        public ITMMod TMMod { get; private set; }
        public ModVersion Version { get; private set; }
        public EnumTypeOffsets TypeOffsets { get; private set; }
        public Assembly Assembly { get; private set; }
        public ContentManager MGContent { get; private set; }
        private CoreAssemblyLoadContext _loadContext;
        private Dictionary<string, CoreModAsset> _assets;
        private bool _disposedValue;

        public void Load(ModInfo info, IMapComponentLoader componentLoader)
        {
            LoadAllContent(info, componentLoader);
        }

        private void LoadAllContent(ModInfo info, IMapComponentLoader componentLoader)
        {
            if (_assets.Count != 0)
            {
                foreach (CoreModAsset value in _assets.Values)
                {
                    value.Dispose();
                }
                _assets.Clear();
            }
            LoadTextures(_assets);
            LoadComponents(_assets, componentLoader);
            string path = Path.Combine(FullPath, "MGContent");
            if (Directory.Exists(path))
            {
                MGContent = new ContentManager(CoreGlobals.Content.ServiceProvider, Path.Combine(FullPath, "MGContent"));
            }
            LoadPlugin(info);
        }

        private void LoadTextures(Dictionary<string, CoreModAsset> dictionary)
        {
            EachFile(Path.Combine(FullPath, "Textures"), "*.png", (string file, string relative) =>
            {
                Texture2D texture = Texture2D.FromFile(CoreGlobals.GraphicsDevice, file);
                dictionary.Add(relative, new CoreTextureAsset(file, texture));
            });
        }

        private void LoadComponents(Dictionary<string, CoreModAsset> dictionary, IMapComponentLoader componentLoader)
        {
            EachFile(Path.Combine(FullPath, "Components"), "*.com", (string file, string relative) =>
            {
                ITMMap map = componentLoader.LoadComponent(file);
                dictionary.Add(relative, new CoreMapAsset(file, new CoreMap(map)));
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
                string relative = Path.GetRelativePath(FullPath, file).Replace('\\', '/');
                relative = relative.Substring(0, relative.LastIndexOf('.'));
                action(file, relative);
            }
        }

        public CoreModAsset GetAsset(string name)
        {
            if (name == null)
            {
                return null;
            }

            _assets.TryGetValue(name, out CoreModAsset asset);
            return asset;
        }

        public T GetAsset<T>(string name) where T : CoreModAsset
        {
            if (name == null)
            {
                return default(T);
            }

            if (_assets.TryGetValue(name, out CoreModAsset asset) && asset is T t)
            {
                return t;
            }
            return default(T);
        }

        public Texture2D GetTexture(string name, int size)
        {
            CoreTextureAsset asset = GetAsset<CoreTextureAsset>($"Textures/{name}");
            if (asset != null)
            {
                return asset.Texture;
            }

            return size switch
            {
                16 => GlobalData.MissingTexture16,
                32 => GlobalData.MissingTexture32,
                64 => GlobalData.MissingTexture64,
                _ => null
            };
        }

        public ICoreMap GetComponent(string name)
        {
            return GetAsset<CoreMapAsset>($"Components/{name}")?.Map;
        }

        public string GetFullPathToAsset(string name)
        {
            return GetAsset(name)?.FullPath;
        }

        private void LoadPlugin(ModInfo info)
        {
            if (info.Plugin == null)
            {
                return;
            }

            _loadContext = new CoreAssemblyLoadContext($"{TMMod.ID}", true);
            string fileName = Path.GetFileName(info.Plugin);
            using Stream stream = File.OpenRead(Path.Combine(FullPath, fileName));
            Assembly assembly = _loadContext.LoadFromStream(stream);
            AssemblyPluginEntryAttribute entry = assembly.GetCustomAttribute<AssemblyPluginEntryAttribute>();
            Type pluginType = null;
            if (entry != null)
            {
                pluginType = entry.PluginType;
                if (pluginType.GetCustomAttribute<PluginEntryAttribute>() == null)
                {
                    throw new Exception("Plugin entry must specify PluginEntryAttribute.");
                }
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<PluginEntryAttribute>() != null)
                    {
                        pluginType = type;
                        break;
                    }
                }
            }

            ICorePlugin plugin = (ICorePlugin)Activator.CreateInstance(pluginType);
            Plugin = plugin;
            Assembly = assembly;
            plugin.Initialize(this);
            Component.RegisterComponents(assembly);
        }

        public void Unload()
        {
            Plugin?.UnloadMod();
            _loadContext?.Unload();
            if (_assets.Count != 0)
            {
                foreach (CoreModAsset value in _assets.Values)
                {
                    value.Dispose();
                }
                _assets.Clear();
            }
        }

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MGContent?.Dispose();
                    foreach (CoreModAsset asset in _assets.Values)
                    {
                        asset.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public Mod(string path, ModInfo info, ITMMod mod, ModType type)
        {
            FullPath = path;
            Id = info.Id;
            Type = type;
            Plugin = null;
            TMMod = mod;
            Version = DeserializationHelper.TryParseModVersion(info.Version, out ModVersion result) ? result : new ModVersion(1, 0, 0);
            TypeOffsets = Traverse.Create(mod).Field("typeOffsets").GetValue<EnumTypeOffsets>();
            _assets = new Dictionary<string, CoreModAsset>();
        }
    }
}
