using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Helpers;
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
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        internal static Func<ICoreWorld, string, Vector3, Vector3, object> _spawnParticle;
        internal static Action<ICoreWorld, object> _destroyParticle;
        internal static AccessTools.FieldRef<object, ParticleData[]> _systemParticles;
        internal static AccessTools.FieldRef<object, List<ParticleData>> _customParticles;
        internal ICoreWorld _currentWorld;
        private ITMWorld _tmWorld;
        private ICoreWorld[] _worlds;
        private CoreDataCollection<ICoreGame> _data;
        private Dictionary<ulong, byte[]> _playerSaveData;
        private int _playerSaveStateTmVersion;
        private int _playerSaveStateCoreVersion;
        private CoreDataInitializer<ICoreActor> _playerDataInitializer;
        private CoreDataInitializer<ICoreActor>[] _actorDataInitializers;
        private bool _disposedValue;

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
        public bool CombatEnabled => _tmWorld.GetCombatEnabled();
        public bool KeepItemsOnDeath => _tmWorld.GetKeepItemsOnDeath();
        public GameMode GameMode => _tmWorld.GameMode;
        public StudioForge.TotalMiner.GameDifficulty Difficulty => _tmWorld.Difficulty;
        public bool IsCreativeMode => _tmWorld.IsCreativeMode;
        public bool IsFiniteResources => _tmWorld.IsFiniteResources;
        public bool IsSkillsEnabled => _tmWorld.IsSkillsEnabled;
        public bool IsLocalSkillsEnabled => _tmWorld.IsLocalSkillsEnabled;
        public bool IsLocalSkills => _tmWorld.IsLocalSkills;

        public ICorePlayer GetLocalPlayer(PlayerIndex playerIndex)
        {
            return _currentWorld.ActorManager.GetPlayer(TMGame.GetLocalPlayer(playerIndex));
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

        public IEnumerable<ICoreWorld> GetAllWorlds()
        {
            return _worlds;
        }

        public ICoreWorld GetWorld(string id)
        {
            return _currentWorld.Id == id ? _currentWorld : null;
        }

        public ICoreWorld GetWorld(ICoreMap map)
        {
            return _currentWorld.Map == map ? _currentWorld : null;
        }

        public ICoreWorld GetWorld(Map map)
        {
            return _currentWorld.Map.TMMap == map ? _currentWorld : null;
        }

        public ICorePlayer GetPlayer(ITMPlayer player)
        {
            return _currentWorld.ActorManager.GetPlayer(player);
        }

        public ICorePlayer GetPlayer(GamerID id)
        {
            return _currentWorld.ActorManager.GetPlayer(id);
        }

        public bool TryGetPlayer(ITMPlayer player, out ICorePlayer result)
        {
            result = GetPlayer(player);
            return result != null;
        }

        public ICoreActor GetActor(ITMActor actor)
        {
            return _currentWorld.ActorManager.GetActor(actor);
        }

        public ICoreActor GetActor(GamerID id)
        {
            return _currentWorld.ActorManager.GetActor(id);
        }

        public void InitializeDefaultData(ICoreActor actor)
        {
            if (actor.IsPlayer)
            {
                _playerDataInitializer?.CreateAndInitializeAll(actor);
            }
            _actorDataInitializers?[actor.CoreActor.NumId]?.CreateAndInitializeAll(actor);
        }

        internal void AllModsLoaded()
        {
            foreach (ICoreMod mod in ModManager.GetAllActivePlugins())
            {
                foreach (Type type in mod.Assembly.GetTypes())
                {
                    ActorDataAttribute actorAttribute = type.GetCustomAttribute<ActorDataAttribute>();
                    if (actorAttribute != null)
                    {
                        CreateActorDataInitializers(type, actorAttribute.Actors);
                    }

                    PlayerDataAttribute playerAttribute = type.GetCustomAttribute<PlayerDataAttribute>();
                    if (playerAttribute != null)
                    {
                        _playerDataInitializer ??= new CoreDataInitializer<ICoreActor>();
                        _playerDataInitializer.AddType(type);
                    }
                }
            }
        }

        private void CreateActorDataInitializers(Type type, string[] actors)
        {
            if (actors == null)
            {
                _actorDataInitializers ??= new CoreDataInitializer<ICoreActor>[Globals1.NpcTypeData.Length];
                for (int i = 0; i < _actorDataInitializers.Length; i++)
                {
                    CreateActorDataInitializer(type, i);
                }
            }
            else
            {
                foreach (string id in actors)
                {
                    int index = Array.FindIndex(Globals1.NpcTypeData, data => data.IDString == id);
                    if (index == -1)
                    {
                        continue;
                    }

                    CreateActorDataInitializer(type, index);
                }
            }
        }

        private void CreateActorDataInitializer(Type type, int index)
        {
            _actorDataInitializers ??= new CoreDataInitializer<ICoreActor>[Globals1.NpcTypeData.Length];
            CoreDataInitializer<ICoreActor> initializer = _actorDataInitializers[index];
            if (initializer == null)
            {
                initializer = new CoreDataInitializer<ICoreActor>();
                _actorDataInitializers[index] = initializer;
            }
            initializer.AddType(type);
        }

        public void Update()
        {
            ModManager.ModUpdate();
            foreach (ICoreWorld world in _worlds)
            {
                world.Update();
                ModManager.ModUpdate(world);
                foreach (ICoreActor actor in world.ActorManager.Actors)
                {
                    ModManager.ModUpdate(actor);
                }
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

        public T GetData<T>() where T : ICoreData<ICoreGame> => _data.GetData<T>();
        public bool TryGetData<T>(out T result) where T : ICoreData<ICoreGame> => _data.TryGetData(out result);
        public void GetAllData(List<ICoreData<ICoreGame>> result) => _data.GetAllData(result);
        public bool HasData<T>() => _data.HasData<T>();
        public void SetData(ICoreData<ICoreGame> data) => _data.SetData(data);
        public void SetData<T>(T data) where T : ICoreData<ICoreGame> => _data.SetData(data);
        public T SetData<T>() where T : ICoreData<ICoreGame>, new() => _data.SetData<T>();
        public ICoreData<ICoreGame> SetDefaultData(ICoreData<ICoreGame> data) => _data.SetDefaultData(data);
        public T SetDefaultData<T>(T data) where T : ICoreData<ICoreGame> => _data.SetDefaultData(data);
        public T SetDefaultData<T>() where T : ICoreData<ICoreGame>, new() => _data.SetDefaultData<T>();

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (ICoreWorld world in _worlds)
                    {
                        world.Dispose();
                    }
                }

                _worlds = null;
                _playerDataInitializer = null;
                _actorDataInitializers = null;
                _data = null;
                _customParticles = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal CoreGame(ITMGame game)
        {
            TMGame = game;
            _tmWorld = game.World;
            _currentWorld = new CoreWorld(this, "Core.Overworld");
            _worlds = new ICoreWorld[1] { _currentWorld };
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
