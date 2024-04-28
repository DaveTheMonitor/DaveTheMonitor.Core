using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Scripts;
using DaveTheMonitor.Core.Wrappers;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DaveTheMonitor.Core
{
    [DebuggerDisplay("Id = {Id}")]
    internal sealed class CoreWorld : ICoreWorld
    {
        public ITMWorld TMWorld => Game.TMGame.World;
        public WorldOptions WorldOptions { get; private set; }
        public ICoreGame Game { get; private set; }
        public string Id { get; private set; }
        public int NumId { get; private set; }
        public WorldDrawOptions DrawOptions { get; set; }
        public ICoreActorManager ActorManager => _actorManager;
        public ITMEntityManager TMEntityManager => TMWorld.EntityManager;
        public ITMEnvManager TMEnvironmentManager => TMWorld.EnvironManager;
        public GameMode GameMode => TMWorld.GameMode;
        public GameDifficulty Difficulty => TMWorld.Difficulty;
        public bool IsCreativeMode => TMWorld.IsCreativeMode;
        public bool IsFiniteResources => TMWorld.IsFiniteResources;
        public bool IsDynamicNaturalEnvironment => TMWorld.IsDynamicNaturalEnvironment;
        public bool IsSkillsEnabled => TMWorld.IsSkillsEnabled;
        public bool IsLocalSkillsEnabled => TMWorld.IsLocalSkillsEnabled;
        public bool IsLocalSkills => TMWorld.IsLocalSkills;
        public float CurrentHour => TMWorld.CurrentHour;
        public bool IsDayTime => TMWorld.IsDayTime;
        public bool IsNightTime => TMWorld.IsNightTime;
        public ICoreMap Map => _map;
        public SaveMapHead Header => TMWorld.Header;
        public string FullPath { get; private set; }
        public BiomeType CurrentBiome => WorldOptions.Biome.Value;
        public IEnumerable<MapMarker> MapMarkers => TMWorld.MapMarkers;
        public IEnumerable<MapMarker> GraveMarkers => TMWorld.GraveMarkers;
        public History History => TMWorld.History;
        public IEnumerable<Zone> Zones => TMWorld.Zones;
        public Vector3 WindVelocity => Game.TMGame.GetWindVelocity();
        public float WindFactor => Game.TMGame.GetWindFactor();
        public MapRenderer? Renderer => _hasRenderer ? _renderer : null;
        public SkyCurtain SkyCurtain => _skyCurtain;
        public Starfield Starfield => _starfield;
        public event ComponentEventHandler ComponentPasted;
        private ActorManager _actorManager;
        private ICoreMap _map;
        private CoreDataCollection<ICoreWorld> _data;
        private bool _hasChunkLoader;
        private bool _hasRenderer;
        private ChunkLoader _chunkLoader;
        private ChunkLoaderPriority _chunkLoaderPriority;
        private MapRenderer _renderer;
        private SkyCurtain _skyCurtain;
        private Starfield _starfield;

        #region Scripts

#pragma warning disable IDE0051

        [ScriptProperty(Name = "ActorCount", Access = ScriptPropertyAccess.Get)]
        private int ScriptActorCount => ActorManager.ActorCount;

        [ScriptProperty(Name = "PlayerCount", Access = ScriptPropertyAccess.Get)]
        private int ScriptPlayerCount => ActorManager.PlayerCount;

        [ScriptProperty(Name = "Time", Access = ScriptPropertyAccess.Get)]
        private double ScriptTime => TMWorld.CurrentHour;

        [ScriptMethod(Name = "GetBlock")]
        private string ScriptGetBlock(int x, int y, int z)
        {
            return Globals1.ItemData[(int)Map.GetBlockID(new GlobalPoint3D(x, y, z))].IDString;
        }

        [ScriptMethod(Name = "GetBlockAux")]
        private int ScriptGetBlockAux(int x, int y, int z)
        {
            return Map.GetAuxData(new GlobalPoint3D(x, y, z));
        }

        [ScriptMethod(Name = "GetBlockLowAux")]
        private int ScriptGetBlockLowAux(int x, int y, int z)
        {
            return (byte)(Map.GetAuxData(new GlobalPoint3D(x, y, z)) << 4);
        }

        [ScriptMethod(Name = "GetBlockHighAux")]
        private int ScriptGetBlockHighAux(int x, int y, int z)
        {
            return Map.GetAuxHighData(new GlobalPoint3D(x, y, z));
        }

        [ScriptMethod(Name = "GetBlockLight")]
        private int ScriptGetBlockLight(int x, int y, int z)
        {
            return Map.GetBlockLight(new GlobalPoint3D(x, y, z));
        }

        [ScriptMethod(Name = "HasChanged")]
        private bool ScriptHasChanged(int x, int y, int z)
        {
            return Map.HasChanged(new GlobalPoint3D(x, y, z));
        }

        [ScriptMethod(Name = "SetBlock")]
        private void ScriptSetBlock(int x, int y, int z, string block, int aux)
        {
            Item id = ItemData.ConvertItemIDToBlockID(Globals1.ItemData.First(d => d.IDString == block)?.ItemID ?? Item.None);
            if (!IsValidBlock(id))
            {
                return;
            }
            Block blockId = (Block)id;

            Map.SetBlockData(new GlobalPoint3D(x, y, z), blockId, (byte)aux, UpdateBlockMethod.Player, StudioForge.Engine.GamerServices.GamerID.Sys2, false);
        }

        [ScriptMethod(Name = "SetBlockAux")]
        private void ScriptSetBlockAux(int x, int y, int z, int aux)
        {
            Map.SetAuxData(new GlobalPoint3D(x, y, z), (byte)aux, UpdateBlockMethod.Player, StudioForge.Engine.GamerServices.GamerID.Sys2, false);
        }

        [ScriptMethod(Name = "SetRegion")]
        private void ScriptSetRegion(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, string block, int aux)
        {
            Item id = ItemData.ConvertItemIDToBlockID(Globals1.ItemData.First(d => d.IDString == block)?.ItemID ?? Item.None);
            if (!IsValidBlock(id))
            {
                return;
            }
            Block blockId = (Block)id;

            minX = Math.Min(minX, maxX);
            minY = Math.Min(minY, maxY);
            minZ = Math.Min(minZ, maxZ);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        Map.SetBlockData(new GlobalPoint3D(x, y, z), blockId, (byte)aux, UpdateBlockMethod.Player, StudioForge.Engine.GamerServices.GamerID.Sys2, false);
                    }
                }
            }
        }

        private bool IsValidBlock(Item id)
        {
            return id switch
            {
                Item.Bedrock => false,
                _ => id < Item.zLastBlockID
            };
        }

        [ScriptMethod(Name = "Commit")]
        private void ScriptCommit()
        {
            Map.Commit();
        }

        [ScriptMethod(Name = "GetActorsInRegion")]
        private ScriptArrayActor ScriptGetActorsInRegion(IScriptRuntime runtime, double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            ScriptArrayActor arr = null;
            BoundingBox box = new BoundingBox(new Vector3((float)minX, (float)minY, (float)minZ), new Vector3((float)maxX, (float)maxY, (float)maxZ));

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (actor.HitBoundingBox.Intersects(box))
                {
                    arr ??= new ScriptArrayActor();
                    arr.Add(runtime, actor);
                }
            }

            if (arr == null)
            {
                return ScriptArrayActor.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetActorsInRadius")]
        private ScriptArrayActor ScriptGetActorsInRadius(IScriptRuntime runtime, double x, double y, double z, double radius)
        {
            ScriptArrayActor arr = null;
            Vector3 v = new Vector3((float)x, (float)y, (float)z);

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (Vector3.Distance(actor.Position, v) <= radius)
                {
                    arr ??= new ScriptArrayActor();
                    arr.Add(runtime, actor);
                }
            }

            if (arr == null)
            {
                return ScriptArrayActor.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetActorsInZone")]
        private ScriptArrayActor ScriptGetActorsInZone(IScriptRuntime runtime, string zoneName)
        {
            ScriptArrayActor arr = null;

            Zone zone = GetZone(zoneName);
            if (zone == null)
            {
                return ScriptArrayActor.EmptyReadOnly;
            }

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (zone.IsInZone((Map)_map.TMMap, actor.HitBoundingBox))
                {
                    arr ??= new ScriptArrayActor();
                    arr.Add(runtime, actor);
                }
            }

            if (arr == null)
            {
                return ScriptArrayActor.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetActorCountInRegion")]
        private int ScriptGetActorCountInRegion(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            int count = 0;
            BoundingBox box = new BoundingBox(new Vector3((float)minX, (float)minY, (float)minZ), new Vector3((float)maxX, (float)maxY, (float)maxZ));

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (actor.HitBoundingBox.Intersects(box))
                {
                    count++;
                }
            }

            return count;
        }

        [ScriptMethod(Name = "GetActorCountInRadius")]
        private int ScriptGetActorCountInRadius(double x, double y, double z, double radius)
        {
            int count = 0;
            Vector3 v = new Vector3((float)x, (float)y, (float)z);

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (Vector3.Distance(actor.Position, v) <= radius)
                {
                    count++;
                }
            }

            return count;
        }

        [ScriptMethod(Name = "GetActorCountInZone")]
        private int ScriptGetActorCountInZone(string zoneName)
        {
            int count = 0;

            Zone zone = GetZone(zoneName);
            if (zone == null)
            {
                return 0;
            }

            foreach (ICoreActor actor in ActorManager.Actors)
            {
                if (zone.IsInZone((Map)_map.TMMap, actor.HitBoundingBox))
                {
                    count++;
                }
            }

            return count;
        }

        [ScriptMethod(Name = "GetPlayersInRegion")]
        private ScriptArrayPlayer ScriptGetPlayersInRegion(IScriptRuntime runtime, double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            ScriptArrayPlayer arr = null;
            BoundingBox box = new BoundingBox(new Vector3((float)minX, (float)minY, (float)minZ), new Vector3((float)maxX, (float)maxY, (float)maxZ));

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (player.HitBoundingBox.Intersects(box))
                {
                    arr ??= new ScriptArrayPlayer();
                    arr.Add(runtime, player);
                }
            }

            if (arr == null)
            {
                return ScriptArrayPlayer.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetPlayersInRadius")]
        private ScriptArrayPlayer ScriptGetPlayersInRadius(IScriptRuntime runtime, double x, double y, double z, double radius)
        {
            ScriptArrayPlayer arr = null;
            Vector3 v = new Vector3((float)x, (float)y, (float)z);

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (Vector3.Distance(player.Position, v) <= radius)
                {
                    arr ??= new ScriptArrayPlayer();
                    arr.Add(runtime, player);
                }
            }

            if (arr == null)
            {
                return ScriptArrayPlayer.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetPlayersInZone")]
        private ScriptArrayPlayer ScriptGetPlayersInZone(IScriptRuntime runtime, string zoneName)
        {
            ScriptArrayPlayer arr = null;

            Zone zone = GetZone(zoneName);
            if (zone == null)
            {
                return ScriptArrayPlayer.EmptyReadOnly;
            }

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (zone.IsInZone((Map)_map.TMMap, player.HitBoundingBox))
                {
                    arr ??= new ScriptArrayPlayer();
                    arr.Add(runtime, player);
                }
            }

            if (arr == null)
            {
                return ScriptArrayPlayer.EmptyReadOnly;
            }
            else
            {
                arr.MakeReadOnly();
                return arr;
            }
        }

        [ScriptMethod(Name = "GetPlayerCountInRegion")]
        private int ScriptGetPlayerCountInRegion(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            int count = 0;
            BoundingBox box = new BoundingBox(new Vector3((float)minX, (float)minY, (float)minZ), new Vector3((float)maxX, (float)maxY, (float)maxZ));

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (player.HitBoundingBox.Intersects(box))
                {
                    count++;
                }
            }

            return count;
        }

        [ScriptMethod(Name = "GetPlayerCountInRadius")]
        private int ScriptGetPlayerCountInRadius(double x, double y, double z, double radius)
        {
            int count = 0;
            Vector3 v = new Vector3((float)x, (float)y, (float)z);

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (Vector3.Distance(player.Position, v) <= radius)
                {
                    count++;
                }
            }

            return count;
        }

        [ScriptMethod(Name = "GetPlayerCountInZone")]
        private int ScriptGetPlayerCountInZone(string zoneName)
        {
            int count = 0;

            Zone zone = GetZone(zoneName);
            if (zone == null)
            {
                return 0;
            }

            foreach (ICorePlayer player in ActorManager.Players)
            {
                if (zone.IsInZone((Map)_map.TMMap, player.HitBoundingBox))
                {
                    count++;
                }
            }

            return count;
        }

#pragma warning restore IDE0051

        #endregion

        public void AddMapMarker(GlobalPoint3D p, string text, MapMarkerType type, bool transmit)
        {
            TMWorld.AddMapMarker(p, text, type, transmit);
        }

        public bool AddPickup(GlobalPoint3D p, InventoryItem item)
        {
            return TMWorld.AddPickup(p, item);
        }

        public bool AddPickup(Vector3 pos, InventoryItem item, Vector2 velocity, float minPickupAge, ICorePlayer player)
        {
            return TMWorld.AddPickup(pos, item, velocity, minPickupAge, player.TMPlayer);
        }

        public void AddProjectile(CoreItem item, Vector3 position, Vector3 velocity, ICorePlayer player, bool transmit)
        {
            TMWorld.AddProjectile(item.ItemType, position, velocity, player.TMPlayer, transmit);
        }

        public void BroadcastSound(Vector3 origin, ICoreActor actor, SoundType soundType)
        {
            TMWorld.BroadcastSound(origin, actor.TMActor, soundType);
        }

        public void CreateBlast(GlobalPoint3D p, CoreItem item, float strength, int radius, ICorePlayer player)
        {
            TMWorld.CreateBlast(p, item.ItemType, strength, radius, player.TMPlayer);
        }

        public bool CreateFallingBlock(GlobalPoint3D p, ICorePlayer player, UpdateBlockMethod method, Action<ItemParticle> onRest, bool transmit)
        {
            return TMWorld.CreateFallingBlock(p, player.TMPlayer, method, onRest, transmit);
        }

        public void FloodPhysics(GlobalPoint3D p, CoreItem block, ICorePlayer player, bool transmit)
        {
            TMWorld.FloodPhysics(p, block.BlockType, player.TMPlayer, transmit);
        }

        public BoundingBox GetBlockBox(GlobalPoint3D p, CoreItem block)
        {
            return TMWorld.GetBlockBox(p, block.BlockType);
        }

        public AudioListener GetClosestListener(Vector3 position)
        {
            return TMWorld.GetClosestListener(position);
        }

        public Zone GetMainZone(GlobalPoint3D p)
        {
            Zone smallest = null;
            float smallestSize = float.MaxValue;
            foreach (Zone zone in Zones)
            {
                float size = zone.Volume();
                if (size < smallestSize)
                {
                    smallest = zone;
                    smallestSize = size;
                }
            }
            return smallest;
        }

        public Zone GetZone(string name)
        {
            foreach (Zone zone in Zones)
            {
                if (zone.Name == name)
                {
                    return zone;
                }
            }
            return null;
        }

        public void GetZones(GlobalPoint3D p, ICollection<Zone> collection)
        {
            collection.Clear();
            foreach (Zone zone in Zones)
            {
                if (zone.IsInZone((Map)_map.TMMap, p))
                {
                    collection.Add(zone);
                }
            }
        }

        public bool IsAnyLocalPlayerInProximity(Vector3 pos, float range, bool eye)
        {
            return TMWorld.IsAnyLocalPlayerInProximity(pos, range, eye);
        }

        public bool IsBlockDeliveringPower(GlobalPoint3D p)
        {
            return TMWorld.IsBlockDeliveringPower(p);
        }

        public bool IsBlockReceivingPower(GlobalPoint3D p)
        {
            return TMWorld.IsBlockReceivingPower(p);
        }

        public HitTest RayBlockTest(Vector3 position, Vector3 dir, float range)
        {
            return TMWorld.RayBlockTest(position, dir, range);
        }

        public bool RunScript(string script, ICoreActor actor)
        {
            return Game.TMGame.RunScript(script, actor.TMActor);
        }

        public void RunSingleScriptCommand(string command, ICoreActor actor)
        {
            Game.TMGame.RunSingleScriptCommand(command, actor.TMActor);
        }

        public void OnEnter(ICoreActor actor)
        {
            if (actor is not ICorePlayer player || !player.IsLocalPlayer)
            {
                return;
            }

            if (!_hasChunkLoader)
            {
                _chunkLoader = new ChunkLoader();
                _chunkLoader.Initialize(Game, Map, player, true);
                _chunkLoaderPriority = new ChunkLoaderPriority(Game, Map);
                _hasChunkLoader = true;
            }

            CancelAllRelatedThreadWorkers();

            Traverse tgame = new Traverse(Game.TMGame);
            tgame.Field("chunkLoader").SetValue(_chunkLoader.ChunkLoaderObject);
            tgame.Field("chunkLoaderPriority").SetValue(_chunkLoaderPriority.ChunkLoaderPriorityObject);
            tgame.Field("map").SetValue(Map.TMMap);
            tgame.Property("CreativeModeHelper").SetValue(new CreativeModeHelper(Game, Map).CreativeModeHelperObject);
            tgame.Field("ParticleModifiers").SetValue(new ParticleModifiers(Game, Map).ParticleModifiersObject);

            ParticleManager particleManager = new ParticleManager(Game, Map);
            particleManager.ParticleManagerObject.Initialize(null);
            particleManager.ParticleManagerObject.LoadContent(null);
            tgame.Field("particleManager").GetValue<GameObjectBase>().UnloadContent();
            tgame.Field("particleManager").SetValue(particleManager.ParticleManagerObject);

            EmitterParticleSystem emitterSystem = new EmitterParticleSystem();
            emitterSystem.Initialize(Map);
            emitterSystem.LoadContent();
            tgame.Field("EmitterParticleSystem").SetValue(emitterSystem.EmitterParticleSystemObject);

            SetAsDrawnWorld();

            Traverse tplayer = new Traverse(player.TMPlayer);
            tplayer.Field("map").SetValue(Map.TMMap);
            tplayer.Field("LeftHand").Field("map").SetValue(Map.TMMap);
            tplayer.Field("RightHand").Field("map").SetValue(Map.TMMap);

            ThreadQueueManager.Instance.QueueWorkItem(_chunkLoader.ChunkLoaderObject, false, PriorityLevel.Normal);
            ThreadQueueManager.Instance.QueueWorkItem(_chunkLoaderPriority.ChunkLoaderPriorityObject, false, PriorityLevel.Priority);
            ThreadQueueManager.Instance.QueueWorkItem(new FireUpdateWorker(Game, PriorityLevel.Normal).FireUpdateWorkerObject, false, PriorityLevel.Normal);
            ThreadQueueManager.Instance.QueueWorkItem(new NpcSpawnWorker(Game, PriorityLevel.Priority).NpcSpawnWorkerObject, false, PriorityLevel.Priority);
            ThreadQueueManager.Instance.QueueWorkItem(new PlayerSurroundings(Game, PriorityLevel.Priority).PlayerSurroundingsObject, false, PriorityLevel.Priority);
            ParticleEmitterWorker particleWorker = new ParticleEmitterWorker(Game, PriorityLevel.Normal);
            ThreadQueueManager.Instance.QueueWorkItem(particleWorker.ParticleEmitterWorkerObject, false, PriorityLevel.Normal);
            tgame.Field("particleEmitterWorker").SetValue(particleWorker.ParticleEmitterWorkerObject);
        }

        public void SetAsDrawnWorld()
        {
            Traverse tgame = new Traverse(Game.TMGame);
            DrawableGameObjectBase oldRenderer = tgame.Field("MapRenderer").GetValue<DrawableGameObjectBase>();
            oldRenderer.IsEnabled = false;

            if (!_hasRenderer)
            {
                _renderer = new MapRenderer(Game, null);
                _renderer.Map = Map.TMMap;
                _renderer.MapRendererObject.Initialize(null);
                _renderer.MapRendererObject.LoadContent(null);
                _hasRenderer = true;
            }

            _renderer.MapRendererObject.IsEnabled = true;
            tgame.Field("MapRenderer").SetValue(_renderer.MapRendererObject);

            tgame.Field("skyCurtain").SetValue(_skyCurtain.SkyCurtainObject);
            tgame.Field("starMap").SetValue(_starfield.StarfieldObject);
            DrawGlobals.WorldDrawOptions = DrawOptions;
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {
            if (!_hasRenderer)
            {
                _renderer = new MapRenderer(Game, null);
                _renderer.Map = Map.TMMap;
                _renderer.MapRendererObject.Initialize(null);
                _renderer.MapRendererObject.LoadContent(null);
                _hasRenderer = true;
            }

            CoreGlobals.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            CoreGlobals.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            WorldDrawOptions originalOptions = DrawGlobals.WorldDrawOptions;
            DrawGlobals.WorldDrawOptions = options;
            Traverse tgame = new Traverse(Game.TMGame);
            object oldSkyCurtain = tgame.Field("skyCurtain").GetValue();
            object oldStarfield = tgame.Field("starMap").GetValue();

            tgame.Field("skyCurtain").SetValue(_skyCurtain.SkyCurtainObject);
            tgame.Field("starMap").SetValue(_starfield.StarfieldObject);
            _renderer.Draw(player, virtualPlayer);
            tgame.Field("skyCurtain").SetValue(oldSkyCurtain);
            tgame.Field("starMap").SetValue(oldStarfield);

            DrawGlobals.WorldDrawOptions = originalOptions;
        }

        private void CancelAllRelatedThreadWorkers()
        {
            IThreadWorkItem[] items = ThreadQueueManager.Instance.PriorityWorkItems;
            foreach (IThreadWorkItem item in items)
            {
                if (ShouldCancel(item))
                {
                    ThreadQueueManager.Instance.CancelQueueItem(item, PriorityLevel.Priority);
                }
            }

            items = ThreadQueueManager.Instance.MainWorkItems;
            foreach (IThreadWorkItem item in items)
            {
                if (ShouldCancel(item))
                {
                    ThreadQueueManager.Instance.CancelQueueItem(item, PriorityLevel.Normal);
                }
            }
        }

        private bool ShouldCancel(IThreadWorkItem item)
        {
            Type type = item.GetType();
            return type == ChunkLoader.Type
                || type == ChunkLoaderPriority.Type
                || type == FireUpdateWorker.Type
                || type == NpcSpawnWorker.Type
                || type == PlayerSurroundings.Type;
        }

        public void SetPower(GlobalPoint3D p, bool power, ICorePlayer player)
        {
            TMWorld.SetPower(p, power, player.TMPlayer);
        }

        public bool SpawnParticle(string id, Vector3 position, Vector3 velocity)
        {
            return SpawnParticle(id, position, velocity, out _);
        }

        public bool SpawnParticle(string id, Vector3 position, Vector3 velocity, out object particle)
        {
            particle = null;
            if (CoreGame._spawnParticle != null)
            {
                object p = CoreGame._spawnParticle(this, id, position, velocity);
                if (p != null)
                {
                    particle = (IHasMovement)p;
                    return true;
                }
            }

            ParticleData[] systemParticles = CoreGame._systemParticles();
            for (int i = 0; i < systemParticles.Length; i++)
            {
                ParticleData template = systemParticles[i];
                template.Velocity += velocity;
                if (template.Name == id)
                {
                    return SpawnParticle(position, ref template);
                }
            }

            List<ParticleData> customParticles = CoreGame._customParticles();
            for (int i = 0; i < customParticles.Count; i++)
            {
                ParticleData template = systemParticles[i];
                template.Velocity += velocity;
                if (template.Name == id)
                {
                    return SpawnParticle(position, ref template);
                }
            }

            return false;
        }

        public bool SpawnParticle(Vector3 position, ref ParticleData data)
        {
            return TMWorld.AddParticle(position, ref data);
        }

        public void DestroyParticle(object particle)
        {
            CoreGame._destroyParticle?.Invoke(this, particle);
        }

        public void TeleportActors(GlobalPoint3D min, GlobalPoint3D max, GlobalPoint3D dest, bool relative)
        {
            TMWorld.TeleportEntities(min, max, dest, relative);
        }

        public T GetData<T>(ICoreMod mod) where T : ICoreData<ICoreWorld>
        {
            return _data.GetData<T>(mod);
        }

        public void SetData(ICoreMod mod, ICoreData<ICoreWorld> data)
        {
            _data.SetData(mod, data);
        }

        public T SetDefaultData<T>(ICoreMod mod) where T : ICoreData<ICoreWorld>, new()
        {
            return _data.SetDefaultData<T>(mod);
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            if (reader.ReadBoolean())
            {
                _data.ReadState(Game.ModManager, reader, tmVersion, coreVersion);
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
        }

        public bool ShouldSaveState()
        {
            return _data.ShouldSaveState();
        }

        public void LoadContent()
        {
            _skyCurtain = new SkyCurtain(Game);
            _skyCurtain.Map = (Map)Map.TMMap;
            _skyCurtain.SkyCurtainObject.LoadContent(null);
            _starfield = new Starfield(Game);
            _starfield.StarfieldObject.LoadContent(null);
        }

        internal CoreWorld(ICoreGame game, string id, int numId, ICoreMap map, WorldOptions options)
        {
            Game = game;
            Id = id;
            NumId = numId;
            WorldOptions = options;
            _actorManager = new ActorManager(game, this, game.TMGame.World.NpcManager);
            _map = map;
            string path = TMWorld.WorldPath;
            if (Utils.StartsWithDriveLetter(path))
            {
                FullPath = path;
            }
            else
            {
                FullPath = Path.Combine(FileSystem.RootPath, path);
            }
            _data = new CoreDataCollection<ICoreWorld>(this);
            DrawOptions = options.DrawOptions;
        }
    }
}
