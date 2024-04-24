using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Plugin;
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
        public static readonly string ContentPath = "CSR";
        public int ActiveMods => _activeMods.Count;
        private List<ICoreMod> _activeMods;
        private List<ICoreMod> _activePlugins;
        private IMapComponentLoader _mapLoader;

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
            return defining ?? CorePlugin.Instance.CoreMod;
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
            return defining ?? CorePlugin.Instance.CoreMod;
        }

        public CoreModAsset GetAsset(ICoreMod mod, string fullId)
        {
            ICoreMod targetMod = GetTargetMod(fullId, out string strippedId) ?? mod;
            return targetMod.GetAsset(strippedId);
        }

        public T GetAsset<T>(ICoreMod mod, string fullId) where T : CoreModAsset
        {
            ICoreMod targetMod = GetTargetMod(fullId, out string strippedId) ?? mod;
            return targetMod.GetAsset<T>(strippedId);
        }

        public Texture2D GetTexture(ICoreMod mod, string fullId, int size)
        {
            // TODO: add TM.Texture for items and blocks?
            ICoreMod targetMod = GetTargetMod(fullId, out string strippedId) ?? mod;
            return targetMod.GetTexture(strippedId, size);
        }

        public ICoreMap GetComponent(ICoreMod mod, string fullId)
        {
            ICoreMod targetMod = GetTargetMod(fullId, out string strippedId) ?? mod;
            return targetMod.GetComponent(strippedId);
        }

        private ICoreMod GetTargetMod(string fullId, out string strippedId)
        {
            int index = fullId.LastIndexOf('.');
            if (index == -1)
            {
                strippedId = fullId;
                return null;
            }

            string modId = fullId.Substring(0, index);
            ICoreMod mod = GetMod(modId);

            strippedId  = fullId.Substring(index + 1);
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
            Mod mod = new Mod(path, modInfo, tmMod, ModType.Core);
            mod.Load(modInfo, _mapLoader);
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
                list.Add(new Mod(mod.FullPath, info, mod, ModType.TM));
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

        public void ModPreDrawWorldMap(ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.PreDrawWorldMap(map, player, virtualPlayer, options);
            }
        }

        public void ModPostDrawWorldMap(ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {
            foreach (ICoreMod mod in _activePlugins)
            {
                mod.Plugin.PostDrawWorldMap(map, player, virtualPlayer, options);
            }
        }

        public ModManager(IMapComponentLoader componentLoader)
        {
            _activeMods = new List<ICoreMod>();
            _activePlugins = new List<ICoreMod>();
            _mapLoader = componentLoader;
        }
    }
}
