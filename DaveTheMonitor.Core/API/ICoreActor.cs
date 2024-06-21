using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// An actor in the world, such as a player or NPC.
    /// </summary>
    public interface ICoreActor : IHasCoreData<ICoreActor>, IScriptObject
    {
        /// <summary>
        /// The game's <see cref="ITMActor"/> implementation for this actor. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMActor TMActor { get; }

        /// <summary>
        /// The <see cref="ICoreGame"/> instance this actor belongs to.
        /// </summary>
        ICoreGame Game { get; }

        /// <summary>
        /// The <see cref="ICoreWorld"/> this actor is in.
        /// </summary>
        ICoreWorld World { get; }

        /// <summary>
        /// This actor's ID.
        /// </summary>
        GamerID Id { get; }

        /// <summary>
        /// This actor's <see cref="Core.CoreActor"/> definition.
        /// </summary>
        CoreActor CoreActor { get; }

        /// <summary>
        /// This actor's current state.
        /// </summary>
        ActorState ActorState { get; }

        /// <summary>
        /// This actor's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// This actor's position.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// This actor's eye offset in blocks.
        /// </summary>
        Vector3 EyeOffset { get; set; }

        /// <summary>
        /// This actor's eye (camera) position.
        /// </summary>
        Vector3 EyePosition { get; }

        /// <summary>
        /// This actor's FOV in radians.
        /// </summary>
        float FOV { get; set; }

        /// <summary>
        /// This actor's velocity in m/s.
        /// </summary>
        Vector3 Velocity { get; set; }

        /// <summary>
        /// This actor's max velocity in m/s.
        /// </summary>
        Vector2 MaxVelocity { get; set; }

        /// <summary>
        /// This actor's view direction.
        /// </summary>
        Vector3 ViewDirection { get; set; }

        /// <summary>
        /// This actor's view matrix, used for world space rendering.
        /// </summary>
        Matrix ViewMatrix { get; }

        /// <summary>
        /// This actor's local view matrix, used for local space rendering.
        /// </summary>
        Matrix ViewMatrixLocal { get; }

        /// <summary>
        /// This actor's projection matrix.
        /// </summary>
        Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// This actor's bounding frustum in world space.
        /// </summary>
        BoundingFrustum Frustum { get; }

        /// <summary>
        /// This actor's bounding box.
        /// </summary>
        BoundingBox HitBoundingBox { get; }

        /// <summary>
        /// This actor's speed override.
        /// </summary>
        float SpeedOverride { get; set; }

        /// <summary>
        /// This actor's gravity override.
        /// </summary>
        float GravityOverride { get; set; }

        /// <summary>
        /// Set to true to override all movement and view input.
        /// </summary>
        bool OverrideControlInput { get; set; }

        /// <summary>
        /// This actor's left hand.
        /// </summary>
        ICoreHand LeftHand { get; }

        /// <summary>
        /// This actor's right hand.
        /// </summary>
        ICoreHand RightHand { get; }

        /// <summary>
        /// This actor's inventory.
        /// </summary>
        ITMInventory Inventory { get; }

        /// <summary>
        /// This actor's audio emitter.
        /// </summary>
        AudioEmitter AudioEmitter { get; }

        /// <summary>
        /// This actor's current oxygen.
        /// </summary>
        float Oxygen { get; set; }

        /// <summary>
        /// This actor's max oxygen.
        /// </summary>
        float MaxOxygen { get; }

        /// <summary>
        /// This actor's current stamina.
        /// </summary>
        float Stamina { get; set; }

        /// <summary>
        /// This actor's max stamina.
        /// </summary>
        float MaxStamina { get; }

        /// <summary>
        /// This actor's current health. Consider calling <see cref="ChangeHealth(float, bool)"/> instead of changing this value directly.
        /// </summary>
        float Health { get; set; }

        /// <summary>
        /// This actor's max health.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// This actor's <see cref="StudioForge.TotalMiner.FlyMode"/>.
        /// </summary>
        FlyMode FlyMode { get; set; }

        /// <summary>
        /// This actor's place/mine reach in blocks.
        /// </summary>
        int Reach { get; set; }

        /// <summary>
        /// True if this actor is a player, otherwise false.
        /// </summary>
        /// <remarks>If true, the actor must implement <see cref="ICorePlayer"/>.</remarks>
        bool IsPlayer { get; }

        /// <summary>
        /// True if this actor is on the ground with no coyote time. Due to a TM bug, this may switch between true and false for a few frames after touching the ground. Using <see cref="IsOnGround(float)"/> will avoid this.
        /// </summary>
        bool Grounded { get; }

        /// <summary>
        /// True if this actor is crouching.
        /// </summary>
        bool IsCrouching { get; }

        /// <summary>
        /// True if this actor is current active in the world.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Custom actor model for use with rigidbody animations. This may be null if the actor does not have a custom model and instead uses a vanilla model.
        /// </summary>
        ActorModel Model { get; }

        /// <summary>
        /// The animation state machine for this actor.
        /// </summary>
        AnimationController Animation { get; }

        /// <summary>
        /// Adds an item to this actor's inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The quantity of items added.</returns>
        int AddToInventory(InventoryItem item);

        /// <summary>
        /// Adds an item to this actor's inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="slotId">The slot ID of the added item.</param>
        /// <returns>The quantity of items added.</returns>
        int AddToInventory(InventoryItem item, out int slotId);

        /// <summary>
        /// Equips an item from this actor's inventory into the default slot.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        /// <returns>True if the item was successfully equipped, otherwise false.</returns>
        bool EquipFromInventory(CoreItem item);

        /// <summary>
        /// Equips an item from this actor's inventory into the specified hand.
        /// </summary>
        /// <param name="hand">The hand to equip to.</param>
        /// <param name="item">The item to equip.</param>
        /// <returns>True if the item was successfully equipped, otherwise false.</returns>
        bool EquipFromInventory(ICoreHand hand, CoreItem item);

        /// <summary>
        /// Unequip an item from this actor.
        /// </summary>
        /// <param name="equipIndex">The slot to unequip the item from.</param>
        /// <returns>True if the item was successfully unequipped, otherwise false.</returns>
        bool UnequipToInventory(EquipIndex equipIndex);

        /// <summary>
        /// Returns true if the specified item is equipped.
        /// </summary>
        /// <param name="item">The item to test for.</param>
        /// <returns>True if the item is equipped, otherwise false.</returns>
        bool IsItemEquipped(CoreItem item);

        /// <summary>
        /// Returns true if the specified item is equipped and currently usable (eg. due to skill requirements).
        /// </summary>
        /// <param name="item">The item to test for.</param>
        /// <returns>True if the item is equipped and usable, otherwise false.</returns>
        bool IsItemEquippedAndUsable(CoreItem item);

        /// <summary>
        /// Gets the slot ID the specified item should be equipped in.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>The slot ID the item should be equipped in.</returns>
        int GetItemEquippedSlot(CoreItem item);

        /// <summary>
        /// Drops the item in the specified slot.
        /// </summary>
        /// <param name="slotId">The slot to drop the item from.</param>
        void DropItem(int slotId);

        /// <summary>
        /// Change this actor's state.
        /// </summary>
        /// <param name="newState">The state to change to.</param>
        /// <returns>True if the state was changed, otherwise fale.</returns>
        bool ChangeState(ActorState newState);

        /// <summary>
        /// Deal damage to this actor and display any damage effects.
        /// </summary>
        /// <param name="damageType">The type of damage to deal. Some damage types don't display damage numbers.</param>
        /// <param name="damage">The amount of damage to deal.</param>
        /// <param name="knockForce">The knockback from the attack.</param>
        /// <returns>The amount of damage taken.</returns>
        float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce);

        /// <summary>
        /// Deal damage to this actor and display any damage effects.
        /// </summary>
        /// <param name="damageType">The type of damage to deal. Some damage types don't display damage numbers.</param>
        /// <param name="damage">The amount of damage to deal.</param>
        /// <param name="knockForce">The knockback from the attack.</param>
        /// <param name="attacker">The attacker dealing the damage.</param>
        /// <param name="weapon">The weapon used.</param>
        /// <param name="attackType">The skill type used in the attack.</param>
        /// <returns>The amount of damage taken.</returns>
        float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce, ICoreActor attacker, CoreItem weapon, SkillType attackType);

        /// <summary>
        /// Returns true if this actor has all of the specified or'd permission(s).
        /// </summary>
        /// <param name="permissions">The permission(s) to test for.</param>
        /// <returns>True if the actor has all of the specified permission(s), otherwise false.</returns>
        bool HasPermission(Permissions permissions);

        /// <summary>
        /// Returns true if this actor has any of the specified or'd permission(s).
        /// </summary>
        /// <param name="permissions">The permission(s) to test for.</param>
        /// <returns>True if the actor has any of the specified permission(s), otherwise false.</returns>
        bool HasPermissionAny(Permissions permissions);

        /// <summary>
        /// Adds or removed the specified permission from this actor.
        /// </summary>
        /// <param name="permission">The permission to toggle.</param>
        /// <param name="enable">True if the permission is enabled, or false if disabled.</param>
        void TogglePermission(Permissions permission, bool enable);

        /// <summary>
        /// Returns true if this actor's history with the specified value != 0.
        /// </summary>
        /// <param name="key">The history to test.</param>
        /// <returns>True if this actor has the specified permission, otherwise false.</returns>
        bool HasHistory(string key);

        /// <summary>
        /// Performs a raycast from this actor's eye position and returns true if the ray intersects a solid block.
        /// </summary>
        /// <param name="dir">The direction of the ray.</param>
        /// <param name="distance">The maximum distance of the ray.</param>
        /// <returns>True if the ray intersects a solid block, otherwise false.</returns>
        bool LineOfSightTest(Vector3 dir, float distance);

        /// <summary>
        /// Teleports this actor to the specified position if the target position is not blocked.
        /// </summary>
        /// <param name="pos">The position to teleport this actor to.</param>
        void TeleportTo(Vector3 pos);

        /// <summary>
        /// Updates this actor's matrices.
        /// </summary>
        void UpdateMatrices();

        /// <summary>
        /// Checks if a <see cref="Ray"/> intersects this actor's hit bounds.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="maxDistance">The maximum distance of the ray.</param>
        /// <param name="distance">The distance between the start of the ray and the hit bound.</param>
        /// <param name="damageMultiplier">The damage multiplier of the hit bound.</param>
        /// <param name="isCritical">True if the hitbound is critical, otherwise false.</param>
        /// <returns>True if the ray intersects this actor's hit bounds, otherwise false.</returns>
        bool CheckHit(Ray ray, float maxDistance, out float distance, out float damageMultiplier, out bool isCritical);

        /// <summary>
        /// Checks if a <see cref="BoundingBox"/> intersects this actor's hit bounds.
        /// </summary>
        /// <param name="box">The bounding box to test.</param>
        /// <param name="damageMultiplier">The damage multiplier of the hit bound.</param>
        /// <param name="isCritical">True if the hitbound is critical, otherwise false.</param>
        /// <returns>True if the bounding box intersects this actor's hit bounds, otherwise false.</returns>
        bool CheckHit(BoundingBox box, out float damageMultiplier, out bool isCritical);

        /// <summary>
        /// Checks if a <see cref="BoundingSphere"/> intersects this actor's hit bounds.
        /// </summary>
        /// <param name="sphere">The bounding sphere to test.</param>
        /// <param name="damageMultiplier">The damage multiplier of the hit bound.</param>
        /// <param name="isCritical">True if the hitbound is critical, otherwise false.</param>
        /// <returns>True if the bounding sphere intersects this actor's hit bounds, otherwise false.</returns>
        bool CheckHit(BoundingSphere sphere, out float damageMultiplier, out bool isCritical);

        /// <summary>
        /// Changes this actor's health.
        /// </summary>
        /// <param name="amount">The amount of health to add or remove.</param>
        /// <param name="triggerEvents">True if hurt and heal events should trigger.</param>
        void ChangeHealth(float amount, bool triggerEvents);

        /// <summary>
        /// Kills this actor.
        /// </summary>
        void Kill();

        /// <summary>
        /// Kills this actor.
        /// </summary>
        /// <param name="deathType">The type of damage that killed this actor.</param>
        /// <param name="attacker">The attacker that killed this actor.</param>
        /// <param name="weapon">The weapon the attacker used to kill this actor.</param>
        /// <param name="damage">The amount of damage dealt to kill this actor.</param>
        void Kill(DamageType deathType, ICoreActor attacker, CoreItem weapon, float damage);

        /// <summary>
        /// Called when this actor is killed.
        /// </summary>
        /// <param name="deathType">The type of damage dealt.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="weapon">The attacker's weapon.</param>
        /// <param name="damage">The damage dealt.</param>
        void OnDeath(DamageType deathType, ICoreActor attacker, CoreItem weapon, float damage);

        /// <summary>
        /// Called when this actor kills their target.
        /// </summary>
        /// <param name="deathType">The type of damage dealt.</param>
        /// <param name="target">The target.</param>
        /// <param name="weapon">This actor's weapon.</param>
        /// <param name="damage">The damage dealt.</param>
        void OnKill(DamageType deathType, ICoreActor target, CoreItem weapon, float damage);

        /// <summary>
        /// Called when this actor is hurt.
        /// </summary>
        /// <param name="damageType">The type of damage dealt.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="weapon">The attacker's weapon.</param>
        /// <param name="damage">The damage dealt.</param>
        void OnHurt(DamageType damageType, ICoreActor attacker, CoreItem weapon, float damage);

        /// <summary>
        /// Called when this actor attacks their target.
        /// </summary>
        /// <param name="damageType">The type of damage dealt.</param>
        /// <param name="target">The target.</param>
        /// <param name="weapon">This actor's weapon.</param>
        /// <param name="damage">The damage dealt.</param>
        void OnAttack(DamageType damageType, ICoreActor target, CoreItem weapon, float damage);

        /// <summary>
        /// Called when this actor is healed.
        /// </summary>
        /// <param name="health">The amount of health gained.</param>
        /// <param name="healer">The healer.</param>
        /// <param name="item">The item used.</param>
        void OnHeal(float health, ICoreActor healer, CoreItem item);

        /// <summary>
        /// Called when this actor's swing starts.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        void OnSwingStart(ICoreHand hand, CoreItem item);

        /// <summary>
        /// Called when this actor's swing fully extends.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        void OnSwingExtended(ICoreHand hand, CoreItem item);

        /// <summary>
        /// Called when this actor's swing ends.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        void OnSwingEnd(ICoreHand hand, CoreItem item);

        /// <summary>
        /// <para>Plays the specified animation state for this actor, cancelling the current animation.</para>
        /// <para>The animation state must exist on the animation controller.</para>
        /// </summary>
        /// <param name="id">The ID of the animation to play.</param>
        /// <returns>True if the animation is started successfully, otherwise false.</returns>
        bool PlayAnimation(string id);

        /// <summary>
        /// Tests if the actor is on the ground with coyote time.
        /// </summary>
        /// <param name="time">The coyote time, in seconds.</param>
        /// <returns>True if the actor is on the ground, otherwise false.</returns>
        bool IsOnGround(float time);

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update();
    }
}
