using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Shaders;
using DaveTheMonitor.Core.Storage;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Compiler;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static StudioForge.Engine.Components.CameraComponent;
using static System.Windows.Forms.Design.AxImporter;

namespace DaveTheMonitor.Core
{
    internal sealed class CoreGame : ICoreGame
    {
        public ITMGame TMGame { get; private set; }
        public ModManager ModManager { get; private set; }
        public ICoreMod ParticlesModule { get; private set; }
        public IMapComponentLoader MapComponentLoader { get; private set; }
        public bool IsHost => true;
        public CommandRegistry CommandRegister { get; private set; }
        public ICoreItemRegistry ItemRegistry { get; private set; }
        public ICoreActorRegistry ActorRegistry { get; private set; }
        public IScriptRuntime ScriptRuntime { get; set; }
        public IScriptCompiler ScriptCompiler { get; set; }
        public string FullPath { get; private set; }
        public int WorldCount => _worlds.Length;
        internal static Func<ICoreWorld, string, Vector3, Vector3, object> _spawnParticle;
        internal static Action<ICoreWorld, object> _destroyParticle;
        internal static AccessTools.FieldRef<object, ParticleData[]> _systemParticles;
        internal static AccessTools.FieldRef<object, List<ParticleData>> _customParticles;
        private static WorldDrawStage[] _drawStages = Enum.GetValues<WorldDrawStage>();
        internal ICoreWorld _currentWorld;
        private ICoreWorld _overworld;
        private ICoreWorld[] _worlds;
        private CoreDataCollection<ICoreGame> _data;
        private Dictionary<ulong, byte[]> _playerSaveData;
        private Dictionary<WorldDrawStage, List<WorldDrawAction>> _preDrawActions;
        private Dictionary<WorldDrawStage, List<WorldDrawAction>> _postDrawActions;
        private int _playerSaveStateTmVersion;
        private int _playerSaveStateCoreVersion;
        private bool _worldsInitialized;

        static CoreGame()
        {
            Type globals2 = AccessTools.TypeByName("StudioForge.TotalMiner.Globals2");
            _systemParticles = AccessTools.FieldRefAccess<ParticleData[]>(globals2, "SystemParticleData");
            _customParticles = AccessTools.FieldRefAccess<List<ParticleData>>(globals2, "CustomParticleData");
        }

        #region ICoreGame

        ICoreModManager ICoreGame.ModManager => ModManager;
        public IGameShader GameShader { get; private set; }
        public PcgRandom Random => TMGame.Random;
        public GraphicsDevice GraphicsDevice => TMGame.GraphicsDevice;
        public WindowManager WindowManager => TMGame.WindowManager;
        public ITMTexturePack TexturePack => TMGame.TexturePack;
        public SpriteBatchSafe SpriteBatch => TMGame.SpriteBatch;

        public ICorePlayer GetLocalPlayer(PlayerIndex playerIndex)
        {
            return _overworld.ActorManager.GetPlayer(TMGame.GetLocalPlayer(playerIndex));
        }

        public void AddNotification(string message)
        {
            TMGame.AddNotification(message);
        }

        public void AddNotification(string message, NotifyRecipient recType)
        {
            TMGame.AddNotification(message, recType);
        }

        public void AddNotification(string message, Color color, NotifyRecipient recType)
        {
            TMGame.AddNotification(message, color, recType);
        }

        public INewGuiMenuScreen OpenMenu(ICorePlayer player, NewGuiMenu[] menus = null, bool mainTabs = true)
        {
            return TMGame.OpenPauseMenu(player.TMPlayer, menus, mainTabs);
        }

        public void AddScreen(IGameScreen screen, ICorePlayer player)
        {
            TMGame.AddScreen(screen, player.TMPlayer);
        }

        #endregion

        public bool IsInGame(ICorePlayer player)
        {
            return player.IsActive;
        }

        public bool IsInGame(ITMPlayer player)
        {
            return _currentWorld.ActorManager.IsActive(player);
        }

        public object ModCall(string modId, params object[] args)
        {
            return ModManager.Call(modId, args);
        }

        void ICoreGame.RegisterWorld(string id, WorldOptions options)
        {
            CreateWorld(id, options);
        }

        public void InitializeAllWorlds()
        {
            if (!_worldsInitialized)
            {
                foreach (ICoreWorld world in _worlds)
                {
                    InitializeWorld(world);
                }
                _worldsInitialized = true;
            }
        }

        private void InitializeWorld(ICoreWorld world)
        {
            foreach (ICoreMod mod in ModManager.GetAllActivePlugins())
            {
                mod.Plugin.InitializeWorld(world);
            }
        }

        private void CreateWorld(string id, WorldOptions options)
        {
            SaveMapHead header = TMGame.World.Header;
            Map currentMap = (Map)TMGame.World.Map;
            ConstructorInfo ctor = AccessTools.Constructor(AccessTools.TypeByName("StudioForge.TotalMiner.MapStrategyTM"), new Type[] { TMGame.GetType() });

            options = new WorldOptions()
            {
                Size = options.Size ?? header.MaxMapSize,
                RegionSize = options.RegionSize ?? header.RegionSize,
                ChunkSize = options.ChunkSize ?? header.ChunkSize,
                Biome = options.Biome ?? TMGame.World.CurrentBiome,
                BiomeParams = options.BiomeParams ?? header.BiomeParams,
                DrawOptions = options.DrawOptions,
                GroundBlock = options.GroundBlock ?? GetGroundBlock(options.FlatWorldType, header.TerrainData),
                FlatWorldType = options.FlatWorldType ?? GetFlatType(options.GroundBlock, header.TerrainData),
                Seed = options.Seed ?? header.MapSeed,
                SeaLevel = options.SeaLevel ?? currentMap.SeaLevel,
                MapStrategy = options.MapStrategy ?? (MapStrategy)ctor.Invoke(new object[] { TMGame }),
            };

            Type[] types = new Type[]
            {
                TMGame.GetType(),
                typeof(string), //name
                typeof(float), //tileSize
                typeof(Point3D), //MaxMapSize
                typeof(BoxInt), //mapBound
                typeof(Point3D), //regionSize
                typeof(Point3D), //chunkSize
                typeof(BlockDataXML[]), // blockData
                typeof(int), //maxLight
                typeof(int), //seed
                typeof(ushort), //initialCacheCount
                typeof(int), //cacheExpandSize
                typeof(MapStrategy), //strategy
                typeof(bool), //isHost
                typeof(bool), //allowMeshCreatorToSplitOrFade
            };
            object[] param = new object[]
            {
                TMGame,
                id,
                1,
                options.Size.Value,
                new BoxInt()
                {
                    Min = new GlobalPoint3D(-options.Size.Value.X / 2, 0, -options.Size.Value.Z / 2),
                    Max = new GlobalPoint3D(options.Size.Value.X / 2, options.Size.Value.Y, options.Size.Value.Z / 2)
                },
                options.RegionSize.Value,
                options.ChunkSize.Value,
                Globals1.BlockData,
                15,
                options.Seed.Value,
                (ushort)4,
                128,
                options.MapStrategy,
                ((Map)TMGame.World.Map).IsHost,
                true
            };

            Map map = (Map)AccessTools.Constructor(AccessTools.TypeByName("StudioForge.TotalMiner.MapTM"), types).Invoke(param);
            map.SeaLevel = options.SeaLevel.Value;
            SetBlockIds(map);
            map.PregenerateRegions(false, true, null);
            CreateWorld(id, new CoreMap((ITMMap)map), options);
        }

        private Block GetGroundBlock(FlatWorldType? definedType, TerrainData data)
        {
            if (definedType.HasValue)
            {
                return definedType switch
                {
                    FlatWorldType.Natural => Block.Grass,
                    FlatWorldType.Default => Block.Grass,
                    FlatWorldType.Sky => Block.None,
                    FlatWorldType.Space => Block.None,
                    _ => Block.Grass
                };
            }

            if (data.BiomeType == BiomeType.Flat)
            {
                return data.GroundBlock switch
                {
                    Item.NaturalWorld => Block.Grass,
                    Item.SkyWorld => Block.None,
                    Item.SpaceWorld => Block.None,
                    _ => data.GroundBlock < Item.zLastBlockID ? (Block)data.GroundBlock : Block.None
                };
            }

            return Block.Grass;
        }

        private FlatWorldType GetFlatType(Block? definedBlock, TerrainData data)
        {
            if (definedBlock.HasValue)
            {
                return FlatWorldType.Default;
            }

            if (data.BiomeType == BiomeType.Flat)
            {
                return data.GroundBlock switch
                {
                    Item.NaturalWorld => FlatWorldType.Natural,
                    Item.SkyWorld => FlatWorldType.Sky,
                    Item.SpaceWorld => FlatWorldType.Space,
                    _ => FlatWorldType.Default
                };
            }

            return FlatWorldType.Natural;
        }

        private void SetBlockIds(Map map)
        {
            map.OutOfBoundsBlockID = (byte)Block.zLastBlockID;
            map.WaterBlockID = (byte)Block.Water;
            map.LavaBlockID = (byte)Block.Lava;
            map.RopeBlockID = (byte)Block.Rope;
            map.BedrockID = (byte)Block.Bedrock;
            map.InvisibleBarrierID = (byte)Block.InvisibleBarrier;
        }

        private void CreateWorld(string id, ICoreMap map, WorldOptions options)
        {
            int index = _worlds.Length;
            Array.Resize(ref _worlds, _worlds.Length + 1);
            CoreWorld world = new CoreWorld(this, id, index, map, options);
            _worlds[index] = world;
            world.LoadContent();

            // We test if worlds have been initialized here so we don't
            // call ICorePlugin.InitializeWorld until Initialize and
            // InitializeGame has been called for all plugins.
            if (_worldsInitialized)
            {
                InitializeWorld(world);
            }
        }

        public IEnumerable<ICoreWorld> GetAllWorlds()
        {
            return _worlds;
        }

        public ICoreWorld GetWorld(string id)
        {
            foreach (ICoreWorld world in _worlds)
            {
                if (world.Id == id)
                {
                    return world;
                }
            }
            return null;
        }

        public ICoreWorld GetWorld(ICoreMap map)
        {
            foreach (ICoreWorld world in _worlds)
            {
                if (world.Map == map)
                {
                    return world;
                }
            }
            return null;
        }

        public ICoreWorld GetWorld(Map map)
        {
            foreach (ICoreWorld world in _worlds)
            {
                if (world.Map.TMMap == map)
                {
                    return world;
                }
            }
            return null;
        }

        public ICorePlayer GetPlayer(ITMPlayer player)
        {
            return _overworld.ActorManager.GetPlayer(player);
        }

        public ICoreActor GetActor(ITMActor actor)
        {
            return _currentWorld.ActorManager.GetActor(actor);
        }

        public void Update()
        {
            ModManager.ModUpdate();
            foreach (ICoreWorld world in _worlds)
            {
                ModManager.ModUpdate(world);
            }
            foreach (ICoreActor actor in _currentWorld.ActorManager.Actors)
            {
                ModManager.ModUpdate(actor);
            }
        }

        public void GetModules()
        {
            ParticlesModule = ModManager.GetMod("Core.Particles");
            _spawnParticle = (Func<ICoreWorld, string, Vector3, Vector3, object>)ParticlesModule?.Plugin.ModCall("GetSpawnParticleDelegate");
            _destroyParticle = (Action<ICoreWorld, object>)ParticlesModule?.Plugin.ModCall("GetDestroyParticleInstanceDelegate");
        }

        public void RegisterCommands()
        {
            CommandRegister.RegisterCommands(GetType().Assembly);
            foreach (ICoreMod mod in ModManager.GetAllActivePlugins())
            {
                CommandRegister.RegisterCommands(mod.Assembly);
            }
        }

        internal void AddActors()
        {
            List<ITMPlayer> list = new List<ITMPlayer>();
            TMGame.GetAllPlayers(list);
            foreach (ITMPlayer player in list)
            {
                _currentWorld.ActorManager.AddActor(player);
            }
            foreach (ITMActor actor in TMGame.World.NpcManager.NpcList)
            {
                _currentWorld.ActorManager.AddActor(actor);
            }
        }

        public void AddPreDrawWorldMap(WorldDrawStage stage, WorldDrawAction action)
        {
            _preDrawActions ??= new Dictionary<WorldDrawStage, List<WorldDrawAction>>();

            foreach (WorldDrawStage s in _drawStages)
            {
                WorldDrawStage s2 = stage & s;
                if (s2 > 0 && _preDrawActions.TryGetValue(s2, out List<WorldDrawAction> actions))
                {
                    actions.Add(action);
                }
                else
                {
                    actions = new List<WorldDrawAction>()
                    {
                        action
                    };
                    _preDrawActions[s2] = actions;
                }
            }
        }

        public void AddPostDrawWorldMap(WorldDrawStage stage, WorldDrawAction action)
        {
            _postDrawActions ??= new Dictionary<WorldDrawStage, List<WorldDrawAction>>();

            foreach (WorldDrawStage s in _drawStages)
            {
                WorldDrawStage s2 = stage & s;
                if (s2 > 0 && _postDrawActions.TryGetValue(s2, out List<WorldDrawAction> actions))
                {
                    actions.Add(action);
                }
                else
                {
                    actions = new List<WorldDrawAction>()
                    {
                        action
                    };
                    _postDrawActions[s2] = actions;
                }
            }
        }

        public void RunPreDrawWorldMap(WorldDrawStage stage, ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {
            if (_preDrawActions == null)
            {
                return;
            }

            if (_preDrawActions.TryGetValue(stage, out List<WorldDrawAction> actions))
            {
                RunActions(actions, map, player, virtualPlayer, stage, options);
            }
        }

        public void RunPostDrawWorldMap(WorldDrawStage stage, ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {
            if (_postDrawActions == null)
            {
                return;
            }

            if (_postDrawActions.TryGetValue(stage, out List<WorldDrawAction> actions))
            {
                RunActions(actions, map, player, virtualPlayer, stage, options);
            }
        }

        private void RunActions(List<WorldDrawAction> list, ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawStage stage, WorldDrawOptions options)
        {
            foreach (WorldDrawAction action in list)
            {
                action(map, player, virtualPlayer, options, stage);
            }
        }

        internal Texture2D StitchModItemTextures(Texture2D atlas, int size, out bool changed)
        {
            int totalItems = 0;
            foreach (CoreItem item in ItemRegistry)
            {
                if (item.TextureHD != null || item.TextureSD != null)
                {
                    totalItems++;
                }
            }

            if (totalItems == 0)
            {
                changed = false;
                return atlas;
            }

            Texture2D newAtlas = new Texture2D(CoreGlobals.GraphicsDevice, atlas.Width, atlas.Height);
            Color[] data = new Color[atlas.Width * atlas.Height];
            atlas.GetData(data, 0, data.Length);
            newAtlas.SetData(data, 0, data.Length);
            StitchItemAtlas(newAtlas, size);

            changed = true;
            return newAtlas;
        }

        private void StitchItemAtlas(Texture2D dest, int size)
        {
            int width = dest.Width;
            int height = dest.Height;
            // should always be 32
            int atlasWidth = width / size;
            foreach (CoreItem item in ItemRegistry)
            {
                if (item.TextureHD == null && item.TextureSD == null)
                {
                    continue;
                }

                Texture2D texture = size == 32 ? item.TextureHD : item.TextureSD;
                Rectangle rect = size == 32 ? item.TextureHDSrc : item.TextureSDSrc;
                if (rect.Width != size || rect.Height != size)
                {
                    throw new InvalidOperationException($"Item texture must be {size}x{size}");
                }

                Rectangle destRect = TexturePack.ItemSrcRect((Item)item.NumId);
                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(0, rect, data, 0, data.Length);

                dest.SetData(0, destRect, data, 0, data.Length);
            }
        }

        public void WriteState(BinaryWriter writer)
        {
            if (_data.ShouldSaveState())
            {
                writer.Write(true);
                _data.WriteState(writer);
            }
            else
            {
                writer.Write(false);
            }

            long countPos = writer.BaseStream.Position;
            writer.Write(0);

            int count = 0;
            foreach (ICoreWorld world in _worlds)
            {
                if (world.ShouldSaveState())
                {
                    writer.Write(world.Id);
                    long lengthPos = writer.BaseStream.Position;
                    writer.Write(0);
                    long itemStart = writer.BaseStream.Position;
                    world.WriteState(writer);
                    long itemEnd = writer.BaseStream.Position;
                    writer.BaseStream.Position = lengthPos;
                    writer.Write((int)(itemEnd - itemStart));
                    writer.BaseStream.Position = itemEnd;
                    count++;
                }
            }

            long end = writer.BaseStream.Position;
            writer.BaseStream.Position = countPos;
            writer.Write(count);
            writer.BaseStream.Position = end;

            int playerCount = 1;
            writer.Write(playerCount);
            ICorePlayer player = GetLocalPlayer(PlayerIndex.One);
            writer.Write(player.Id.ID);

            long playerLengthPos = writer.BaseStream.Position;
            writer.Write(0);

            long playerStart = writer.BaseStream.Position;
            player.WriteState(writer);
            long playerEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = playerLengthPos;
            writer.Write((int)(playerEnd - playerStart));
            writer.BaseStream.Position = playerEnd;
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            if (reader.ReadBoolean())
            {
                _data.ReadState(ModManager, reader, tmVersion, coreVersion);
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string id = reader.ReadString();
                int length = reader.ReadInt32();
                ICoreWorld world = GetWorld(id);
                if (world == null)
                {
                    reader.BaseStream.Position += length;
                }

                world.ReadState(reader, tmVersion, coreVersion);
            }

            int players = reader.ReadInt32();
            for (int i = 0; i < players; i++)
            {
                // We copy player save data to _playerSaveData
                // since the players don't exist at this point
                // in loading.
                // We then read the data later when the player
                // is added.
                _playerSaveData ??= new Dictionary<ulong, byte[]>();
                ulong id = reader.ReadUInt64();
                int length = reader.ReadInt32();
                byte[] data = reader.ReadBytes(length);
                _playerSaveData[id] = data;
            }
            _playerSaveStateTmVersion = tmVersion;
            _playerSaveStateCoreVersion = coreVersion;
        }

        public bool ShouldSaveState()
        {
            ICorePlayer localPlayer = GetLocalPlayer(PlayerIndex.One);
            if (localPlayer.ShouldSaveState())
            {
                return true;
            }

            if (_data.ShouldSaveState())
            {
                return true;
            }

            foreach (ICoreWorld world in _worlds)
            {
                if (world.ShouldSaveState())
                {
                    return true;
                }
            }

            return false;
        }

        public void LoadPlayerState(ICorePlayer player)
        {
#if DEBUG
            CorePlugin.Log($"Loading player state for {player.Name}");
#endif
            if (_playerSaveData?.TryGetValue(player.Id.ID, out byte[] data) == true)
            {
                using MemoryStream memStream = new MemoryStream(data);
                using BinaryReader reader = new BinaryReader(memStream);
                player.ReadState(reader, _playerSaveStateTmVersion, _playerSaveStateCoreVersion);
            }
        }

        public void Unload()
        {
            _worlds = null;
            _currentWorld = null;
            ItemRegistry = null;
            ActorRegistry = null;
            ScriptRuntime.Dispose();
            ScriptRuntime = null;
            ScriptCompiler = null;
            ModManager = null;
            ParticlesModule = null;
            MapComponentLoader = null;
        }

        public T GetData<T>(ICoreMod mod) where T : ICoreData<ICoreGame>
        {
            return _data.GetData<T>(mod);
        }

        public void SetData(ICoreMod mod, ICoreData<ICoreGame> data)
        {
            _data.SetData(mod, data);
        }

        public T SetDefaultData<T>(ICoreMod mod) where T : ICoreData<ICoreGame>, new()
        {
            return _data.SetDefaultData<T>(mod);
        }

        internal CoreGame(ITMGame game)
        {
            CoreMap map = new CoreMap(game.World.Map);
            SaveMapHead header = game.World.Header;
            WorldOptions overworldOptions = new WorldOptions()
            {
                Biome = game.World.CurrentBiome,
                BiomeParams = game.World.Header.BiomeParams,
                Size = (Point3D)map.MapSize,
                RegionSize = map.RegionSize,
                ChunkSize = map.ChunkSize,
                DrawOptions = WorldDrawOptions.All,
                MapStrategy = map.BWMap.MapStrategy,
                SeaLevel = header.TerrainData.SeaLevel,
                Seed = map.BWMap.Seed,
                GroundBlock = GetGroundBlock(null, header.TerrainData),
                FlatWorldType = GetFlatType(null, header.TerrainData),
            };

            TMGame = game;
            _overworld = new CoreWorld(this, "Core.Overworld", 0, map, overworldOptions);
            ((CoreWorld)_overworld).LoadContent();
            _currentWorld = _overworld;
            _worldsInitialized = false;
            _worlds = new ICoreWorld[1] { _overworld };
            _data = new CoreDataCollection<ICoreGame>(this);
            MapComponentLoader = new MapComponentLoader(this);
            ModManager = new ModManager(MapComponentLoader);
            CommandRegister = new CommandRegistry();
            ItemRegistry = new ItemRegistry(this);
            ActorRegistry = new ActorRegistry(this);
            GameShader = new GameShader();
            string path = game.World.WorldPath;
            if (Utils.StartsWithDriveLetter(path))
            {
                FullPath = path;
            }
            else
            {
                FullPath = Path.Combine(FileSystem.RootPath, path);
            }
        }
    }
}
