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
            foreach (ICoreActor actor in _actorsList)
            {
                if (box.Intersects(actor.HitBoundingBox))
                {
                    list.Add(actor);
                }
            }
        }

        public void GetActors(BoundingSphere sphere, ICollection<ICoreActor> list)
        {
            list.Clear();
            foreach (ICoreActor actor in _actorsList)
            {
                if (sphere.Intersects(actor.HitBoundingBox))
                {
                    list.Add(actor);
                }
            }
        }

        public void GetActors(Ray ray, float maxDistance, ICollection<ICoreActor> list)
        {
            list.Clear();
            foreach (ICoreActor actor in _actorsList)
            {
                float? d = ray.Intersects(actor.HitBoundingBox);
                if (d.HasValue && d.Value <= maxDistance)
                {
                    list.Add(actor);
                }
            }
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

        public bool AddActor(ITMActor actor)
        {
            GamerID id = ((IActorBehaviour)actor).GamerID;
            if (_actors.ContainsKey(((IActorBehaviour)actor).GamerID.ID))
            {
#if DEBUG
                CorePlugin.Warn($"Actor {actor.ActorType} already added: {id}");
#endif
                return false;
            }

            Actor newActor = actor is ITMPlayer player ? new Player(_game, _world, player) : new NPC(_game, _world, actor);
            _actors.Add(id.ID, newActor);
            _actorsList.Add(newActor);
            Raise_ActorAdded(newActor);

            if (newActor is Player p)
            {
                _game.LoadPlayerState(p);
            }
#if DEBUG
            CorePlugin.Log($"Actor {actor.ActorType} added: {id}");
#endif
            return true;
        }

        public bool RemoveActor(ITMActor actor)
        {
            GamerID id = ((IActorBehaviour)actor).GamerID;

            bool removed = _actors.Remove(id.ID, out ICoreActor newActor);
            if (removed)
            {
                _actorsList.Remove(newActor);
                Raise_ActorRemoved(newActor);
            }

#if DEBUG
            if (removed)
            {
                CorePlugin.Log($"Actor {actor.ActorType} removed: {id}");
            }
            else
            {
                CorePlugin.Warn($"Actor {actor.ActorType} removal failed: {id}");
            }
#endif            
            return removed;
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
