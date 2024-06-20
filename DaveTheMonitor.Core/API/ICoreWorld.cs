using DaveTheMonitor.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A world containing any world-specific data and information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike the Total Miner API, worlds and the main game instance in the Core API are separated.
    /// There is no way to access the "active" world from the <see cref="ICoreGame"/> instance, outside
    /// of <see cref="ICoreGame.GetLocalPlayer(PlayerIndex)"/>.World, which is useful in very specific
    /// situations.
    /// Instead, you can get the world an actor is in using <see cref="ICoreActor.World"/>.
    /// World data is saved and loaded through <see cref="ICoreData{T}"/> instead of a plugin
    /// method implementation.
    /// </para>
    /// <para>
    /// This is because support for multiple active worlds (dimensions) may be added in the future.
    /// Any existing mods using the API correctly should mostly continue to function even after this
    /// change is made.
    /// It's for this reason that using the game's <see cref="ITMWorld"/> implementation is highly discouraged,
    /// as it may behave in unexpected ways after this change.
    /// </para>
    /// </remarks>
    public interface ICoreWorld : IHasCoreData<ICoreWorld>, IScriptObject, IHasBinaryState, IDisposable
    {
        /// <summary>
        /// The game's ITMWorld implementation. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMWorld TMWorld { get; }

        /// <summary>
        /// The main game instance this world belongs to.
        /// </summary>
        ICoreGame Game { get; }

        /// <summary>
        /// The ID of this world.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The <see cref="ICoreActorManager"/> of this world.
        /// </summary>
        ICoreActorManager ActorManager { get; }

        /// <summary>
        /// The <see cref="ITMEntityManager"/> of this world.
        /// </summary>
        ITMEntityManager TMEntityManager { get; }

        /// <summary>
        /// The <see cref="ITMEnvManager"/> of this world.
        /// </summary>
        ITMEnvManager TMEnvironmentManager { get; }

        /// <summary>
        /// The <see cref="StudioForge.TotalMiner.GameMode"/> of this world.
        /// </summary>
        GameMode GameMode { get; }

        /// <summary>
        /// The <see cref="GameDifficulty"/> of this world.
        /// </summary>
        GameDifficulty Difficulty { get; }
        
        /// <summary>
        /// True if this world is in creative mode, otherwise false.
        /// </summary>
        bool IsCreativeMode { get; }

        /// <summary>
        /// True if this world has finite resources enabled, otherwise false.
        /// </summary>
        bool IsFiniteResources { get; }

        /// <summary>
        /// True if this world has a dynamic natural environment, otherwise false.
        /// </summary>
        bool IsDynamicNaturalEnvironment { get; }

        /// <summary>
        /// True if this world has skills enabled, otherwise false.
        /// </summary>
        bool IsSkillsEnabled { get; }

        /// <summary>
        /// True if this world has local skills enabled, otherwise false.
        /// </summary>
        bool IsLocalSkillsEnabled { get; }

        /// <summary>
        /// True if this world uses local skills, otherwise false.
        /// </summary>
        bool IsLocalSkills { get; }

        /// <summary>
        /// The current time, in hours, of this world.
        /// </summary>
        float CurrentHour { get; }

        /// <summary>
        /// True if it is currently day in this world, otherwise false.
        /// </summary>
        bool IsDayTime { get; }

        /// <summary>
        /// True if it is currently night in this world, otherwise false.
        /// </summary>
        bool IsNightTime { get; }

        /// <summary>
        /// The map currently in use by this world.
        /// </summary>
        ICoreMap Map { get; }

        /// <summary>
        /// The <see cref="SaveMapHead"/> of the world.
        /// </summary>
        SaveMapHead Header { get; }

        /// <summary>
        /// The full path of this world's save data.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// The <see cref="BiomeType"/> of this world.
        /// </summary>
        BiomeType CurrentBiome { get; }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of all map markers in the world.
        /// </summary>
        IEnumerable<MapMarker> MapMarkers { get; }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of all grave markers in the world.
        /// </summary>
        IEnumerable<MapMarker> GraveMarkers { get; }

        /// <summary>
        /// This world's history.
        /// </summary>
        History History { get; }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of all zones in this world.
        /// </summary>
        IEnumerable<Zone> Zones { get; }

        /// <summary>
        /// The velocity of the wind in this world.
        /// </summary>
        Vector3 WindVelocity { get; }

        /// <summary>
        /// The multiplier of the wind in this world.
        /// </summary>
        float WindFactor { get; }

        /// <summary>
        /// The actor renderer for this world used to render actors with custom models.
        /// </summary>
        ICoreActorRenderer ActorRenderer { get; }

        /// <summary>
        /// Get the <see cref="BoundingBox"/> of the block at the specified position.
        /// </summary>
        /// <param name="p">The position of the block.</param>
        /// <param name="block">The type of the block.</param>
        /// <returns>The block's <see cref="BoundingBox"/>.</returns>
        BoundingBox GetBlockBox(GlobalPoint3D p, CoreItem block);

        /// <summary>
        /// Adds a pickup at the specified position.
        /// </summary>
        /// <param name="p">The position to add the pickup at.</param>
        /// <param name="item">The item to drop.</param>
        /// <returns>True if a pickup was added, otherwise false.</returns>
        bool AddPickup(GlobalPoint3D p, InventoryItem item);

        /// <summary>
        /// Adds a pickup at the specified position.
        /// </summary>
        /// <param name="pos">The position to add the pickup at.</param>
        /// <param name="item">The item to drop.</param>
        /// <param name="velocity">The velocity of the pickup.</param>
        /// <param name="minPickupAge">The time, in seconds, the pickup must exist before it can be picked up.</param>
        /// <param name="player">The player that dropped the item.</param>
        /// <returns>True if a pickup was added, otherwise false.</returns>
        bool AddPickup(Vector3 pos, InventoryItem item, Vector2 velocity, float minPickupAge, ICorePlayer player);

        /// <summary>
        /// Creates a projectile using the texture and stats of the specified item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="position">The position of the projectile.</param>
        /// <param name="velocity">The velocity of the projectile.</param>
        /// <param name="player">The player who fired the projectile.</param>
        /// <param name="transmit">True if this projectile should be networked, otherwise false.</param>
        void AddProjectile(CoreItem item, Vector3 position, Vector3 velocity, ICorePlayer player, bool transmit);

        /// <summary>
        /// Adds a new map marker.
        /// </summary>
        /// <param name="p">The position of the marker.</param>
        /// <param name="text">The name of the marker.</param>
        /// <param name="type">The type of the marker.</param>
        /// <param name="transmit">True if this marker should be networked, otherwise false.</param>
        void AddMapMarker(GlobalPoint3D p, string text, MapMarkerType type, bool transmit);

        /// <summary>
        /// Performs a raycast for block collision.
        /// </summary>
        /// <param name="position">The position of the ray.</param>
        /// <param name="dir">The direction of the ray.</param>
        /// <param name="range">The range of the ray.</param>
        /// <returns>The result of the raycast.</returns>
        HitTest RayBlockTest(Vector3 position, Vector3 dir, float range);

        /// <summary>
        /// Returns true if the block at the specified position is delivering (transmitting) power.
        /// </summary>
        /// <param name="p">The position to test.</param>
        /// <returns>True if the block is delivering power, otherwise false.</returns>
        bool IsBlockDeliveringPower(GlobalPoint3D p);

        /// <summary>
        /// Returns true if the block at the specified position is receiving power.
        /// </summary>
        /// <param name="p">The position to test.</param>
        /// <returns>True if the block is receiving power, otherwise false.</returns>
        bool IsBlockReceivingPower(GlobalPoint3D p);

        /// <summary>
        /// Sets the power state of the block at the specified position.
        /// </summary>
        /// <param name="p">The position of the block.</param>
        /// <param name="power">The power state.</param>
        /// <param name="player">The player responsible for the change.</param>
        void SetPower(GlobalPoint3D p, bool power, ICorePlayer player);

        /// <summary>
        /// Returns the <see cref="AudioListener"/> of the local player closest to the specified position.
        /// </summary>
        /// <param name="position">The position of the sound.</param>
        /// <returns>The <see cref="AudioListener"/> of the closest local player.</returns>
        AudioListener GetClosestListener(Vector3 position);

        /// <summary>
        /// Broadcasts a sound.
        /// </summary>
        /// <param name="origin">The position of the sound.</param>
        /// <param name="actor">The actor that emitted the sound.</param>
        /// <param name="soundType">The type of sound to emit.</param>
        void BroadcastSound(Vector3 origin, ICoreActor actor, SoundType soundType);

        /// <summary>
        /// Returns true if a local player is within range of the specified position.
        /// </summary>
        /// <param name="pos">The position to test.</param>
        /// <param name="range">The radius to test.</param>
        /// <param name="eye">True if the player's eye position should be tested, otherwise false.</param>
        /// <returns></returns>
        bool IsAnyLocalPlayerInProximity(Vector3 pos, float range, bool eye);

        /// <summary>
        /// Creates a blast at the specified position.
        /// </summary>
        /// <param name="p">The position of the blast.</param>
        /// <param name="item">The item that created the blast.</param>
        /// <param name="strength">The strength of the blast.</param>
        /// <param name="radius">The radius of the blast.</param>
        /// <param name="player">The player that created the blast.</param>
        void CreateBlast(GlobalPoint3D p, CoreItem item, float strength, int radius, ICorePlayer player);

        /// <summary>
        /// Converts the block at the specified position to a falling block.
        /// </summary>
        /// <param name="p">The position to convert.</param>
        /// <param name="player">The player who caused the fall.</param>
        /// <param name="method">The update method that caused the block to fall.</param>
        /// <param name="onRest">Called when the block lands.</param>
        /// <param name="transmit">True if this block should be networked, otherwise false.</param>
        /// <returns>True if the falling block was created, otherwise false.</returns>
        bool CreateFallingBlock(GlobalPoint3D p, ICorePlayer player, UpdateBlockMethod method, Action<ItemParticle> onRest, bool transmit);

        /// <summary>
        /// Floods an area.
        /// </summary>
        /// <param name="p">The origin of the flood.</param>
        /// <param name="block">The block to flood with.</param>
        /// <param name="player">The player who caused the flood.</param>
        /// <param name="transmit">True if this flood should be networked, otherwise false.</param>
        void FloodPhysics(GlobalPoint3D p, CoreItem block, ICorePlayer player, bool transmit);

        /// <summary>
        /// Teleports all actors in an area.
        /// </summary>
        /// <param name="min">The minimum bounds of the area.</param>
        /// <param name="max">The maximum bounds of the area.</param>
        /// <param name="dest">The teleport destination.</param>
        /// <param name="relative">True if the destination is relative to the actors' position.</param>
        void TeleportActors(GlobalPoint3D min, GlobalPoint3D max, GlobalPoint3D dest, bool relative);

        /// <summary>
        /// Gets the smallest zone at the specified position.
        /// </summary>
        /// <param name="p">The position to test.</param>
        /// <returns>The smallest zone at the specified position, or null if no zones are found.</returns>
        Zone GetMainZone(GlobalPoint3D p);

        /// <summary>
        /// Gets the zone with the specified name.
        /// </summary>
        /// <param name="name">The name of the zone.</param>
        /// <returns>The zone with the specified name, or null if no zone is found.</returns>
        Zone GetZone(string name);

        /// <summary>
        /// Clears <paramref name="collection"/>, then fills it with all zones at the specified position.
        /// </summary>
        /// <param name="p">The position to test.</param>
        /// <param name="collection">The collection to fill.</param>
        void GetZones(GlobalPoint3D p, ICollection<Zone> collection);

        /// <summary>
        /// Runs the specified script for the specified actor.
        /// </summary>
        /// <param name="script">The name of the script to run.</param>
        /// <param name="actor">The actor to execute the script on.</param>
        /// <returns>True if the script was executed, otherwise false.</returns>
        bool RunScript(string script, ICoreActor actor);

        /// <summary>
        /// Runs the specified script command for the specified actor.
        /// </summary>
        /// <param name="command">The script command to run.</param>
        /// <param name="actor">The actor to execute the script command on.</param>
        void RunSingleScriptCommand(string command, ICoreActor actor);

        /// <summary>
        /// Spawns a particle with the specified ID at the specified position.
        /// </summary>
        /// <param name="id">The ID of the particle to spawn.</param>
        /// <param name="position">The position to spawn the particle at.</param>
        /// <param name="velocity">The velocity of the particle.</param>
        /// <returns>True if a particle was spawned, otherwise false.</returns>
        /// <remarks>
        /// <para>This method will use the Particles Module if it is active, or TM particle templates if it is not active or the particle isn't found.</para>
        /// </remarks>
        bool SpawnParticle(string id, Vector3 position, Vector3 velocity);

        /// <summary>
        /// Spawns a particle with the specified ID at the specified position.
        /// </summary>
        /// <param name="id">The ID of the particle to spawn.</param>
        /// <param name="position">The position to spawn the particle at.</param>
        /// <param name="velocity">The velocity of the particle.</param>
        /// <param name="particle">The particle instance object if a Particles Module particle was spawned.</param>
        /// <returns>True if a particle was spawned, otherwise false.</returns>
        /// <remarks>
        /// <para>This method will use the Particles Module if it is active, or TM particle templates if it is not active or the particle isn't found.</para>
        /// </remarks>
        bool SpawnParticle(string id, Vector3 position, Vector3 velocity, out object particle);

        /// <summary>
        /// Spawns a TM particle at the specified position.
        /// </summary>
        /// <param name="position">The position to spawn the particle at.</param>
        /// <param name="data">The particle data to spawn.</param>
        /// <returns>True if a particle was spawned, otherwise false.</returns>
        bool SpawnParticle(Vector3 position, ref ParticleData data);

        /// <summary>
        /// Destroys the specified Particles Module particle instance.
        /// </summary>
        /// <param name="particle">The particle to destroy.</param>
        /// <remarks>
        /// <para>This method doesn't do anything if the Particles Module is not active.</para>
        /// </remarks>
        void DestroyParticle(object particle);
        
        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Returns true if this world has any state to save.
        /// </summary>
        /// <returns>True if this world has any state to save, otherwise false.</returns>
        bool ShouldSaveState();
    }
}
