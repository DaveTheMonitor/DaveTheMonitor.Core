using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Assets.Loaders;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
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

        public IEnumerable<ICoreMod> GetAllActiveMods(ModType modType)
        {
            List<ICoreMod> mods = new List<ICoreMod>();
            foreach (ICoreMod mod in _activeMods)
            {
                if (mod.Type == modType)
                {
                    mods.Add(mod);
                }
            }
            return mods;
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

        public static int RegisterAllBlueprints(ICoreGame game)
        {
            ICoreItemRegistry registry = game.ItemRegistry;
            int count = 0;
            foreach (ICoreMod mod in game.ModManager.GetAllActiveMods(ModType.Core))
            {
                ForEachJson(Path.Combine(mod.FullPath, "Blueprints"), json =>
                {
                    BlueprintXML[] blueprints = ParseBlueprints(json, registry);
                    int start = Globals1.BlueprintData.Length;
                    Array.Resize(ref Globals1.BlueprintData, Globals1.BlueprintData.Length + blueprints.Length);
                    for (int i = 0; i < blueprints.Length; i++)
                    {
                        Globals1.BlueprintData[start + i] = blueprints[i];
                    }
                    count += blueprints.Length;
                });
            }
            return count;
        }

        private static BlueprintXML[] ParseBlueprints(string json, ICoreItemRegistry registry)
        {
            JsonDocument doc = JsonDocument.Parse(json, DeserializationHelper.DocumentOptionsTrailingCommasSkipComments);
            JsonElement root = doc.RootElement;
            ModVersion version = DeserializationHelper.GetVersionProperty(root, "Version") ?? throw new InvalidCoreJsonException("Blueprint Version must be specified");
            if (version != new ModVersion(1, 0, 0))
            {
                throw new InvalidCoreJsonException($"Blueprint version {version} not found.");
            }

            if (!root.TryGetProperty("Blueprints", out JsonElement blueprintsElement))
            {
                throw new InvalidCoreJsonException("Blueprint must specify at least one blueprint.");
            }

            if (blueprintsElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidCoreJsonException("Blueprint Blueprints must be an array of objects.");
            }

            List<BlueprintXML> list = new List<BlueprintXML>();
            foreach (JsonElement blueprintElement in blueprintsElement.EnumerateArray())
            {
                if (blueprintElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidCoreJsonException("Blueprint Blueprints must be an array of objects.");
                }

                bool isDefault = DeserializationHelper.GetBoolProperty(blueprintElement, "IsDefault") ?? false;
                Vector2 depth = DeserializationHelper.GetVector2Property(blueprintElement, "Depth") ?? new Vector2(0, 1);
                BlueprintCraftType type = DeserializationHelper.GetEnumProperty<BlueprintCraftType>(blueprintElement, "Type") ?? BlueprintCraftType.Crafting;
                if (type == BlueprintCraftType.Crafting)
                {
                    if (!blueprintElement.TryGetProperty("Pattern", out JsonElement patternElement))
                    {
                        throw new InvalidCoreJsonException("Blueprint Pattern must be specified.");
                    }

                    if (patternElement.ValueKind != JsonValueKind.Array)
                    {
                        throw new InvalidCoreJsonException("Blueprint Pattern must be an array of arrays of items.");
                    }

                    int patternLength = patternElement.GetArrayLength();
                    if (patternLength == 0 || patternLength > 3)
                    {
                        throw new InvalidCoreJsonException("Blueprint Pattern must contain 1-3 rows.");
                    }

                    InventoryItemXML[,] items = new InventoryItemXML[3, 3];
                    int row = 0;
                    foreach (JsonElement arr in patternElement.EnumerateArray())
                    {
                        if (arr.ValueKind != JsonValueKind.Array)
                        {
                            throw new InvalidCoreJsonException("Blueprint Pattern must be an array of arrays of items.");
                        }

                        int length = arr.GetArrayLength();
                        if (length == 0 || length > 3)
                        {
                            throw new InvalidCoreJsonException("Blueprint Pattern array must contain 1-3 items.");
                        }

                        int column = 0;
                        foreach (JsonElement itemElement in arr.EnumerateArray())
                        {
                            if (itemElement.ValueKind == JsonValueKind.String)
                            {
                                CoreItem item = registry.GetDefinition(itemElement.GetString()) ?? TMItems.None;
                                items[row, column] = new InventoryItemXML()
                                {
                                    ItemID = item.ItemType,
                                    Count = item == TMItems.None ? 0 : 1
                                };
                            }
                            else if (itemElement.ValueKind == JsonValueKind.Object)
                            {
                                if (!itemElement.TryGetProperty("ID", out JsonElement idElement))
                                {
                                    throw new InvalidCoreJsonException("Blueprint Pattern item must specify an ID");
                                }

                                if (idElement.ValueKind != JsonValueKind.String)
                                {
                                    throw new InvalidCoreJsonException("Blueprint Pattern item ID must be a string.");
                                }

                                CoreItem item = registry.GetDefinition(idElement.GetString()) ?? TMItems.None;
                                if (item == TMItems.None)
                                {
                                    items[row, column] = new InventoryItemXML()
                                    {
                                        ItemID = Item.None,
                                        Count = 0
                                    };
                                    column++;
                                    continue;
                                }

                                ushort count = DeserializationHelper.GetUInt16Property(itemElement, "Count") ?? 1;
                                ushort durability = DeserializationHelper.GetUInt16Property(itemElement, "Durability") ?? 0;
                                items[row, column] = new InventoryItemXML()
                                {
                                    ItemID = item.ItemType,
                                    Count = count,
                                    Durability = durability,
                                };
                            }
                            else
                            {
                                throw new InvalidCoreJsonException("Blueprint Pattern item must be a string or object.");
                            }

                            column++;
                        }
                        row++;
                    }

                    if (!blueprintElement.TryGetProperty("Result", out JsonElement resultElement))
                    {
                        throw new InvalidCoreJsonException("Blueprint Result must be specified.");
                    }

                    InventoryItemNDXML result;
                    if (resultElement.ValueKind == JsonValueKind.String)
                    {
                        CoreItem item = registry.GetDefinition(resultElement.GetString()) ?? TMItems.None;
                        result = new InventoryItemNDXML()
                        {
                            ItemID = item.ItemType,
                            Count = 1
                        };
                    }
                    else if (resultElement.ValueKind == JsonValueKind.Object)
                    {
                        if (!resultElement.TryGetProperty("ID", out JsonElement idElement))
                        {
                            throw new InvalidCoreJsonException("Blueprint Result item must specify an ID");
                        }

                        if (idElement.ValueKind != JsonValueKind.String)
                        {
                            throw new InvalidCoreJsonException("Blueprint Result item ID must be a string.");
                        }

                        CoreItem item = registry.GetDefinition(idElement.GetString()) ?? TMItems.None;
                        if (item == TMItems.None)
                        {
                            result = new InventoryItemNDXML()
                            {
                                ItemID = Item.None,
                                Count = 1
                            };
                        }

                        ushort count = DeserializationHelper.GetUInt16Property(resultElement, "Count") ?? 1;
                        result = new InventoryItemNDXML()
                        {
                            ItemID = item.ItemType,
                            Count = count
                        };
                    }
                    else
                    {
                        throw new InvalidCoreJsonException("Blueprint Result item must be a string or object.");
                    }

                    BlueprintXML xml = new BlueprintXML()
                    {
                        IsDefault = isDefault,
                        IsValid = true,
                        CraftType = type,
                        Depth = depth,
                        Result = result,
                        ItemID = result.ItemID,
                        Material11 = items[2, 0],
                        Material12 = items[2, 1],
                        Material13 = items[2, 2],
                        Material21 = items[1, 0],
                        Material22 = items[1, 1],
                        Material23 = items[1, 2],
                        Material31 = items[0, 0],
                        Material32 = items[0, 1],
                        Material33 = items[0, 2],
                    };
                    list.Add(xml);
                }
            }
            return list.ToArray();
        }

        private static void ForEachJson(string fullPath, Action<string> action)
        {
            if (!Directory.Exists(fullPath))
            {
                return;
            }

            string[] files = DeserializationHelper.GetJsonFiles(fullPath, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                action(json);
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
