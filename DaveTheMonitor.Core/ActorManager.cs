using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Events;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core
{
    internal sealed class ActorManager : ICoreActorManager
    {
        public ITMNpcManager NpcManager { get; private set; }
        public IEnumerable<ICoreActor> Actors => _actorsList;
        public IEnumerable<ICorePlayer> Players => _playersList;
        public int ActorCount => _actorsList.Count;
        public int PlayerCount => _playersList.Count;
        public event EventHandler<CoreActorEventArgs> ActorAdded;
        public event EventHandler<CoreActorEventArgs> ActorRemoved;
        private ICoreGame _game;
        private ICoreWorld _world;
        private List<ICoreActor> _actorsList;
        private List<ICorePlayer> _playersList;
        private Dictionary<ulong, ICoreActor> _actors;

        public ICoreActor GetActor(ITMActor actor)
        {
            GamerID id = ((IActorBehaviour)actor).GamerID;
            _actors.TryGetValue(id.ID, out ICoreActor result);
#if DEBUG
            if (result == null)
            {
                CorePlugin.Warn($"Null actor {actor.ActorType}: {id}");
            }
#endif
            return result;
        }

        public ICoreActor GetActor(GamerID id)
        {
            _actors.TryGetValue(id.ID, out ICoreActor actor);
#if DEBUG
            if (actor == null)
            {
                CorePlugin.Warn($"Null actor ID {id}");
            }
#endif
            return actor;
        }

        public void GetActors(BoundingBox box, ICollection<ICoreActor> list)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (actor.IsActive && box.Intersects(actor.HitBoundingBox))
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public void GetActors(BoundingSphere sphere, ICollection<ICoreActor> list)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (actor.IsActive && sphere.Intersects(actor.HitBoundingBox))
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public void GetActors(Ray ray, float maxDistance, ICollection<ICoreActor> list)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (!actor.IsActive)
                    {
                        continue;
                    }

                    float? d = ray.Intersects(actor.HitBoundingBox);
                    if (d.HasValue && d.Value <= maxDistance)
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public void GetActors(BoundingBox box, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (actor.IsActive && box.Intersects(actor.HitBoundingBox) && predicate(actor))
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public void GetActors(BoundingSphere sphere, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (actor.IsActive && sphere.Intersects(actor.HitBoundingBox) && predicate(actor))
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public void GetActors(Ray ray, float maxDistance, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate)
        {
            list.Clear();
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    if (!actor.IsActive)
                    {
                        continue;
                    }

                    float? d = ray.Intersects(actor.HitBoundingBox);
                    if (d.HasValue && d.Value <= maxDistance && predicate(actor))
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        public List<ICoreActor> GetActors(BoundingBox box)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(box, list);
            return list;
        }

        public List<ICoreActor> GetActors(BoundingSphere sphere)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(sphere, list);
            return list;
        }

        public List<ICoreActor> GetActors(Ray ray, float maxDistance)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(ray, maxDistance, list);
            return list;
        }

        public List<ICoreActor> GetActors(BoundingBox box, Predicate<ICoreActor> predicate)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(box, list, predicate);
            return list;
        }

        public List<ICoreActor> GetActors(BoundingSphere sphere, Predicate<ICoreActor> predicate)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(sphere, list, predicate);
            return list;
        }

        public List<ICoreActor> GetActors(Ray ray, float maxDistance, Predicate<ICoreActor> predicate)
        {
            List<ICoreActor> list = new List<ICoreActor>();
            GetActors(ray, maxDistance, list, predicate);
            return list;
        }

        public ICorePlayer GetPlayer(ITMPlayer player)
        {
            return (ICorePlayer)GetActor(player);
        }

        public ICorePlayer GetPlayer(GamerID id)
        {
            // We use as instead of a cast here in case id refers
            // to an NPC instead of a player
            return GetActor(id) as ICorePlayer;
        }

        public bool IsActive(ITMActor actor)
        {
            return _actors.TryGetValue(((IActorBehaviour)actor).GamerID.ID, out ICoreActor a) && a.IsActive;
        }

        public void Update()
        {
            lock (_actorsList)
            {
                foreach (ICoreActor actor in _actorsList)
                {
                    actor.Update();
                }
            }
        }

        public bool AddActor(ITMActor tmActor)
        {
            return AddActor(tmActor, out _);
        }

        public bool AddActor(ITMActor tmActor, out ICoreActor coreActor)
        {
            GamerID id = ((IActorBehaviour)tmActor).GamerID;
            coreActor = null;
            lock (_actorsList)
            {
                if (_actors.ContainsKey(id.ID))
                {
#if DEBUG
                    CorePlugin.Warn($"Actor {tmActor.ActorType} already added: {id}");
#endif
                    return false;
                }

                coreActor = tmActor is ITMPlayer player ? new Player(_game, _world, player) : new NPC(_game, _world, tmActor);
                _actors.Add(coreActor.Id.ID, coreActor);
                _actorsList.Add(coreActor);
                _game.InitializeDefaultData(coreActor);
                Raise_ActorAdded(coreActor);
            }

            if (coreActor is ICorePlayer p)
            {
                _game.LoadPlayerState(p);
            }
#if DEBUG
            CorePlugin.Log($"Actor {coreActor.CoreActor.Id} added: {coreActor.Id}");
#endif
            return true;
        }

        public bool RemoveActor(ITMActor tmActor)
        {
            GamerID id = ((IActorBehaviour)tmActor).GamerID;

            bool removed;
            lock (_actorsList)
            {
                removed = _actors.Remove(id.ID, out ICoreActor newActor);
                if (removed)
                {
                    _actorsList.Remove(newActor);
                    Raise_ActorRemoved(newActor);
                }
            }

#if DEBUG
            if (removed)
            {
                CorePlugin.Log($"Actor {tmActor.ActorType} removed: {id}");
            }
            else
            {
                CorePlugin.Warn($"Actor {tmActor.ActorType} removal failed: {id}");
            }
#endif
            return removed;
        }

        public ICoreActor SpawnNpc(CoreActor actor, Vector3 position)
        {
            return SpawnNpc(actor, position, 0, @"System\AI\Default", DayOrNight.None, null, null, null);
        }

        public ICoreActor SpawnNpc(CoreActor actor, Vector3 position, float spawnAngle = 0, string behavior = @"System\AI\Default", DayOrNight activeTime = DayOrNight.None, string killScript = null, LootTable loot = null, CombatStats? stats = null)
        {
            behavior ??= @"System\AI\Default";
            ITMActor tmActor = NpcManager.SpawnNpc(actor.ActorType, position, spawnAngle, behavior, activeTime, killScript, loot, stats);
            AddActor(tmActor, out ICoreActor coreActor);
            return coreActor;
        }

        private void Raise_ActorAdded(ICoreActor actor)
        {
            if (ActorAdded != null)
            {
                ActorAdded(this, new CoreActorEventArgs(actor));
            }
        }

        private void Raise_ActorRemoved(ICoreActor actor)
        {
            if (ActorRemoved != null)
            {
                ActorRemoved(this, new CoreActorEventArgs(actor));
            }
        }

        public ActorManager(ICoreGame game, ICoreWorld world, ITMNpcManager npcManager)
        {
            NpcManager = npcManager;
            _game = game;
            _world = world;
            _actors = new Dictionary<ulong, ICoreActor>();
            _actorsList = new List<ICoreActor>();
        }
    }
}
