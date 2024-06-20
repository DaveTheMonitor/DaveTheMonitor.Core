using DaveTheMonitor.Core.Events;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Manages all actors in the world.
    /// </summary>
    public interface ICoreActorManager
    {
        /// <summary>
        /// The game's <see cref="ITMNpcManager"/> implementation. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMNpcManager NpcManager { get; }

        /// <summary>
        /// A list of all active actors.
        /// </summary>
        IEnumerable<ICoreActor> Actors { get; }

        /// <summary>
        /// A list of all active players.
        /// </summary>
        IEnumerable<ICorePlayer> Players { get; }

        /// <summary>
        /// The number of active actors.
        /// </summary>
        int ActorCount { get; }

        /// <summary>
        /// The number of active players.
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Called when an actor is added to the world.
        /// </summary>
        event EventHandler<CoreActorEventArgs> ActorAdded;

        /// <summary>
        /// Called when an actor is removed from the world.
        /// </summary>
        event EventHandler<CoreActorEventArgs> ActorRemoved;

        /// <summary>
        /// Gets the <see cref="ICoreActor"/> for the specified <see cref="ITMActor"/>.
        /// </summary>
        /// <param name="actor">The actor to get.</param>
        /// <returns>The <see cref="ICoreActor"/> for the specified <see cref="ITMActor"/>.</returns>
        ICoreActor GetActor(ITMActor actor);

        /// <summary>
        /// Gets the actor with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the actor.</param>
        /// <returns>The actor with the specified ID, or null if they don't exist.</returns>
        ICoreActor GetActor(GamerID id);

        /// <summary>
        /// Clears <paramref name="list"/>, then fills it with all actors in <paramref name="box"/>.
        /// </summary>
        /// <param name="box">The area to search.</param>
        /// <param name="list">The list to fill.</param>
        void GetActors(BoundingBox box, ICollection<ICoreActor> list);

        /// <summary>
        /// Clears <paramref name="list"/>, then fills it with all actors in <paramref name="sphere"/>.
        /// </summary>
        /// <param name="sphere">The area to search.</param>
        /// <param name="list">The list to fill.</param>
        void GetActors(BoundingSphere sphere, ICollection<ICoreActor> list);

        /// <summary>
        /// Clears <paramref name="list"/>, then fills it with all actors hit by <paramref name="ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="maxDistance">The maximum distance of the ray.</param>
        /// <param name="list">The list to fill.</param>
        void GetActors(Ray ray, float maxDistance, ICollection<ICoreActor> list);

        /// <summary>
        /// Gets the <see cref="ICorePlayer"/> for the specified <see cref="ITMPlayer"/>.
        /// </summary>
        /// <param name="player">The player to get.</param>
        /// <returns>The <see cref="ICorePlayer"/> for the specified <see cref="ITMPlayer"/>.</returns>
        ICorePlayer GetPlayer(ITMPlayer player);

        /// <summary>
        /// Gets the player with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        /// <returns>The player with the specified ID, or null if they don't exist.</returns>
        ICorePlayer GetPlayer(GamerID id);

        /// <summary>
        /// Returns true if the specified actor exists in this <see cref="ICoreActorManager"/>.
        /// </summary>
        /// <param name="actor">The actor to test.</param>
        /// <returns>True if the specified actor exists, otherwise false.</returns>
        bool IsActive(ITMActor actor);

        /// <summary>
        /// Adds the specified actor to this <see cref="ICoreActorManager"/>.
        /// </summary>
        /// <param name="actor">The actor to add.</param>
        /// <returns>True if the actor was added, otherwise false.</returns>
        bool AddActor(ITMActor actor);

        /// <summary>
        /// Removes the specified actor from this <see cref="ICoreActorManager"/>.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        /// <returns>True if the actor was removed, otherwise false.</returns>
        bool RemoveActor(ITMActor actor);

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update();
    }
}
