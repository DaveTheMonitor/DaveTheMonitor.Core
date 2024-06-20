using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
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
        public ModContentManager Content { get; private set; }
        public ICoreModManager ModManager { get; private set; }
        private CoreAssemblyLoadContext _loadContext;
        private Dictionary<string, CoreModAsset> _assets;
        private Dictionary<string, string> _assetPaths;
        private bool _disposedValue;

        public void Load(ModInfo info)
        {
            Content = new ModContentManager(FullPath, ModManager, this);
            LoadPlugin(info);
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
                    Content?.Dispose();
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

        public Mod(string path, ModInfo info, ITMMod mod, ModType type, ICoreModManager modManager)
        {
            FullPath = path;
            Id = info.Id;
            Type = type;
            Plugin = null;
            TMMod = mod;
            Version = DeserializationHelper.TryParseModVersion(info.Version, out ModVersion result) ? result : new ModVersion(1, 0, 0);
            TypeOffsets = Traverse.Create(mod).Field("typeOffsets").GetValue<EnumTypeOffsets>();
            _assets = new Dictionary<string, CoreModAsset>();
            ModManager = modManager;
        }
    }
}
