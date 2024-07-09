using DaveTheMonitor.Core.Events;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
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
        /// A list of all active actors. This list is not thread safe; it must be locked if you loop over it.
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
        /// Clears <paramref name="list"/>, then fills it with all actors in <paramref name="box"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="box">The area to search.</param>
        /// <param name="list">The list to fill.</param>
        /// <param name="predicate">The predicate to test.</param>
        void GetActors(BoundingBox box, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate);

        /// <summary>
        /// Clears <paramref name="list"/>, then fills it with all actors in <paramref name="sphere"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="sphere">The area to search.</param>
        /// <param name="list">The list to fill.</param>
        /// <param name="predicate">The predicate to test.</param>
        void GetActors(BoundingSphere sphere, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate);

        /// <summary>
        /// Clears <paramref name="list"/>, then fills it with all actors hit by <paramref name="ray"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="maxDistance">The maximum distance of the ray.</param>
        /// <param name="list">The list to fill.</param>
        /// <param name="predicate">The predicate to test.</param>
        void GetActors(Ray ray, float maxDistance, ICollection<ICoreActor> list, Predicate<ICoreActor> predicate);

        /// <summary>
        /// Returns a list with all actors in <paramref name="box"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(BoundingBox, ICollection{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="box">The area to search.</param>
        /// <returns>A list of all actors in the specified <see cref="BoundingBox"/>.</returns>
        List<ICoreActor> GetActors(BoundingBox box);

        /// <summary>
        /// Returns a list with all actors in <paramref name="sphere"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(BoundingSphere, ICollection{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="sphere">The area to search.</param>
        /// <returns>A list of all actors in the specified <see cref="BoundingSphere"/>.</returns>
        List<ICoreActor> GetActors(BoundingSphere sphere);

        /// <summary>
        /// Returns a list with all actors hit by <paramref name="ray"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(Ray, float, ICollection{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="ray">The ray to test.</param>
        /// <param name="maxDistance">The maximum distance of the ray.</param>
        /// <returns>A list of all actors hit by the specified <see cref="Ray"/>.</returns>
        List<ICoreActor> GetActors(Ray ray, float maxDistance);

        /// <summary>
        /// Returns a list with all actors in <paramref name="box"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(BoundingBox, ICollection{ICoreActor}, Predicate{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="box">The area to search.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns>A list of all actors in the specified <see cref="BoundingBox"/>.</returns>
        List<ICoreActor> GetActors(BoundingBox box, Predicate<ICoreActor> predicate);

        /// <summary>
        /// Returns a list with all actors in <paramref name="sphere"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(BoundingSphere, ICollection{ICoreActor}, Predicate{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="sphere">The area to search.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns>A list of all actors in the specified <see cref="BoundingSphere"/>.</returns>
        List<ICoreActor> GetActors(BoundingSphere sphere, Predicate<ICoreActor> predicate);

        /// <summary>
        /// Returns a list with all actors hit by <paramref name="ray"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>
        /// This method creates and returns a new list every time it is called, even if no actors are found. If calling this method often, consider using <see cref="GetActors(Ray, float, ICollection{ICoreActor}, Predicate{ICoreActor})"/> instead.
        /// </remarks>
        /// <param name="ray">The ray to test.</param>
        /// <param name="maxDistance">The maximum distance of the ray.</param>
        /// <param name="predicate">The predicate to test.</param>
        /// <returns>A list of all actors hit by the specified <see cref="Ray"/>.</returns>
        List<ICoreActor> GetActors(Ray ray, float maxDistance, Predicate<ICoreActor> predicate);

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
        /// Adds the specified actor to this <see cref="ICoreActorManager"/>.
        /// </summary>
        /// <param name="actor">The actor to add.</param>
        /// <param name="coreActor">The <see cref="ICoreActor"/> for the <see cref="ITMActor"/>.</param>
        /// <returns>True if the actor was added, otherwise false.</returns>
        bool AddActor(ITMActor actor, out ICoreActor coreActor);

        /// <summary>
        /// Removes the specified actor from this <see cref="ICoreActorManager"/>.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        /// <returns>True if the actor was removed, otherwise false.</returns>
        bool RemoveActor(ITMActor actor);

        /// <summary>
        /// Spawns an NPC at the specified position with default behavior, loot, and stats.
        /// </summary>
        /// <param name="actor">The actor to spawn.</param>
        /// <param name="position">The position.</param>
        /// <returns>The spawned NPC.</returns>
        ICoreActor SpawnNpc(CoreActor actor, Vector3 position);

        /// <summary>
        /// Spawns an NPC at the specified position.
        /// </summary>
        /// <param name="actor">The actor to spawn.</param>
        /// <param name="position">The position.</param>
        /// <param name="spawnAngle">The angle of the spawn.</param>
        /// <param name="behavior">The behavior tree of the NPC.</param>
        /// <param name="activeTime">The time of day this NPC is active during. They will despawn outside of this time.</param>
        /// <param name="killScript">The script executed when this NPC is killed.</param>
        /// <param name="loot">The loot dropped by this NPC.</param>
        /// <param name="stats">The stats of this NPC.</param>
        /// <returns>The spawned NPC.</returns>
        ICoreActor SpawnNpc(CoreActor actor, Vector3 position, float spawnAngle = 0, string behavior = @"System\AI\Default", DayOrNight activeTime = DayOrNight.None, string killScript = null, LootTable loot = null, CombatStats? stats = null);

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update();
    }
}
