﻿using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Scripts;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaveTheMonitor.Core
{
    internal sealed class CoreWorld : ICoreWorld
    {
        public ITMWorld TMWorld => Game.TMGame.World;
        public ICoreGame Game { get; private set; }
        public string Id { get; private set; }
        public ICoreActorManager ActorManager => _actorManager;
        public ITMEntityManager TMEntityManager => TMWorld.EntityManager;
        public ITMEnvManager TMEnvironmentManager => TMWorld.EnvironManager;
        public ICoreActorRenderer ActorRenderer { get; private set; }
        public bool IsDynamicNaturalEnvironment => TMWorld.IsDynamicNaturalEnvironment;
        public float CurrentHour => TMWorld.CurrentHour;
        public bool IsDayTime => TMWorld.IsDayTime;
        public bool IsNightTime => TMWorld.IsNightTime;
        public ICoreMap Map => _map;
        public SaveMapHead Header => TMWorld.Header;
        public string FullPath { get; private set; }
        public BiomeType CurrentBiome => TMWorld.CurrentBiome;
        public IEnumerable<MapMarker> MapMarkers => TMWorld.MapMarkers;
        public IEnumerable<MapMarker> GraveMarkers => TMWorld.GraveMarkers;
        public History History => TMWorld.History;
        public IEnumerable<Zone> Zones => TMWorld.Zones;
        public Vector3 WindVelocity => TMWorld.GetWindVelocity();
        public float WindFactor => TMWorld.GetWindFactor();
        public event ComponentEventHandler ComponentPasted;
        private ActorManager _actorManager;
        private CoreMap _map;
        private CoreDataCollection<ICoreWorld> _data;
        private SoundManager _soundManager;
        private bool _disposedValue;

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
                if (zone.IsInZone(_map.BWMap, actor.HitBoundingBox))
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
                if (zone.IsInZone(_map.BWMap, actor.HitBoundingBox))
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
                if (zone.IsInZone(_map.BWMap, player.HitBoundingBox))
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
                if (zone.IsInZone(_map.BWMap, player.HitBoundingBox))
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
                if (zone.IsInZone(_map.BWMap, p))
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

        public SoundEffectInstance PlaySound(string sound, Vector3 position)
        {
            return PlaySound(sound, position, 1, 0);
        }

        public SoundEffectInstance PlaySound(string sound, Vector3 position, float volume, float pitch)
        {
            SoundEffect sfx = Game.ModManager.LoadSound(CorePlugin.CoreMod, sound);
            if (sfx == null)
            {
                return null;
            }

            return PlaySound(sfx, position, volume, pitch);
        }

        public SoundEffectInstance PlaySound(string sound)
        {
            return PlaySound(sound, 1, 0, 0);
        }

        public SoundEffectInstance PlaySound(string sound, float volume, float pitch, float pan)
        {
            SoundEffect sfx = Game.ModManager.LoadSound(CorePlugin.CoreMod, sound);
            if (sfx == null)
            {
                return null;
            }

            return PlaySound(sfx, volume, pitch, pan);
        }

        public SoundEffectInstance PlaySound(SoundEffect sound)
        {
            return PlaySound(sound, 1, 0, 0);
        }

        public SoundEffectInstance PlaySound(SoundEffect sound, float volume, float pitch, float pan)
        {
            return _soundManager.PlaySound(sound, volume, pitch, pan);
        }

        public SoundEffectInstance PlaySound(SoundEffect sound, Vector3 position)
        {
            return PlaySound(sound, position, 1, 0);
        }

        public SoundEffectInstance PlaySound(SoundEffect sound, Vector3 position, float volume, float pitch)
        {
            AudioListener listener = GetClosestListener(position);
            if (listener == null)
            {
                return null;
            }

            AudioEmitter emitter = new AudioEmitter()
            {
                Position = position,
                Forward = Vector3.Forward,
                Up = Vector3.Up,
                Velocity = Vector3.Zero,
                DopplerScale = 1
            };

            return _soundManager.PlaySound(sound, listener, emitter, volume, pitch);
        }

        public void Update()
        {
            _soundManager.Update();
            ActorRenderer.Update();
            ActorManager.Update();
        }

        public T GetData<T>() where T : ICoreData<ICoreWorld> => _data.GetData<T>();
        public bool TryGetData<T>(out T result) where T : ICoreData<ICoreWorld> => _data.TryGetData(out result);
        public void GetAllData(List<ICoreData<ICoreWorld>> result) => _data.GetAllData(result);
        public bool HasData<T>() => _data.HasData<T>();
        public void SetData(ICoreData<ICoreWorld> data) => _data.SetData(data);
        public void SetData<T>(T data) where T : ICoreData<ICoreWorld> => _data.SetData(data);
        public T SetData<T>() where T : ICoreData<ICoreWorld>, new() => _data.SetData<T>();
        public ICoreData<ICoreWorld> SetDefaultData(ICoreData<ICoreWorld> data) => _data.SetDefaultData(data);
        public T SetDefaultData<T>(T data) where T : ICoreData<ICoreWorld> => _data.SetDefaultData(data);
        public T SetDefaultData<T>() where T : ICoreData<ICoreWorld>, new() => _data.SetDefaultData<T>();
        public IEnumerator<ICoreData<ICoreWorld>> GetDataEnumerator() => ((IHasCoreData<ICoreWorld>)_data).GetDataEnumerator();

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

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _soundManager.Dispose();
                    ActorRenderer.Dispose();
                }

                ActorRenderer = null;
                _map = null;
                _data = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal CoreWorld(ICoreGame game, string id)
        {
            Game = game;
            _actorManager = new ActorManager(game, this, game.TMGame.World.NpcManager);
            _map = new CoreMap(TMWorld.Map);
            Id = id;
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
            ActorRenderer = new ActorRenderer();
            _soundManager = new SoundManager();
        }
    }
}
