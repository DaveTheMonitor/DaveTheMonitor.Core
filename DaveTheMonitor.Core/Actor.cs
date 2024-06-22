using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Invokers;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Attributes;
using DaveTheMonitor.Scripts.Objects;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core
{
    internal abstract class Actor : ICoreActor, IHasMovement
    {
        public ITMActor TMActor { get; private set; }
        public ICoreGame Game { get; private set; }
        public ICoreWorld World { get; private set; }
        public Hand LeftHand { get; private set; }
        public Hand RightHand { get; private set; }
        public virtual CoreActor CoreActor => Game.ActorRegistry.GetActor(TMActor.ActorType);

        #region Invokers

        private static ActorDieInvoker _dieInvoker;

        #endregion

        protected CoreDataCollection<ICoreActor> Data;

        private float _timeOffGround;

        static Actor()
        {
            Type actor = AccessTools.TypeByName("StudioForge.TotalMiner.Actor");
            _dieInvoker = AccessTools.Method(actor, "Die").CreateInvoker<ActorDieInvoker>();
        }

        #region Scripts

#pragma warning disable IDE0051

        [ScriptProperty(Name = "World", Access = ScriptPropertyAccess.Get)]
        private ICoreWorld ScriptWorld => World;

        [ScriptProperty(Name = "IsPlayer", Access = ScriptPropertyAccess.Get)]
        private bool ScriptIsPlayer => IsPlayer;

        [ScriptProperty(Name = "IsNpc", Access = ScriptPropertyAccess.Get)]
        private bool ScriptIsNpc => !IsPlayer;

        [ScriptProperty(Name = "PositionX", Access = ScriptPropertyAccess.Get)]
        private double ScriptPositionX => Position.X;

        [ScriptProperty(Name = "PositionY", Access = ScriptPropertyAccess.Get)]
        private double ScriptPositionY => Position.Y;

        [ScriptProperty(Name = "PositionZ", Access = ScriptPropertyAccess.Get)]
        private double ScriptPositionZ => Position.Z;

        [ScriptProperty(Name = "VelocityX", Access = ScriptPropertyAccess.Get)]
        private double ScriptVelocityX => Velocity.X;

        [ScriptProperty(Name = "VelocityY", Access = ScriptPropertyAccess.Get)]
        private double ScriptVelocityY => Velocity.Y;

        [ScriptProperty(Name = "VelocityZ", Access = ScriptPropertyAccess.Get)]
        private double ScriptVelocityZ => Velocity.Z;

        [ScriptProperty(Name = "EyeX", Access = ScriptPropertyAccess.Get)]
        private double ScriptEyeX => EyePosition.X;

        [ScriptProperty(Name = "EyeY", Access = ScriptPropertyAccess.Get)]
        private double ScriptEyeY => EyePosition.Y;

        [ScriptProperty(Name = "EyeZ", Access = ScriptPropertyAccess.Get)]
        private double ScriptEyeZ => EyePosition.Z;

        [ScriptProperty(Name = "ViewX", Access = ScriptPropertyAccess.Get)]
        private double ScriptViewX => ViewDirection.X;

        [ScriptProperty(Name = "ViewY", Access = ScriptPropertyAccess.Get)]
        private double ScriptViewY => ViewDirection.Y;

        [ScriptProperty(Name = "ViewZ", Access = ScriptPropertyAccess.Get)]
        private double ScriptViewZ => ViewDirection.Z;

        [ScriptProperty(Name = "CursorX", Access = ScriptPropertyAccess.Get)]
        protected virtual int ScriptCursorX => 0;

        [ScriptProperty(Name = "CursorY", Access = ScriptPropertyAccess.Get)]
        protected virtual int ScriptCursorY => 0;

        [ScriptProperty(Name = "CursorZ", Access = ScriptPropertyAccess.Get)]
        protected virtual int ScriptCursorZ => 0;

        [ScriptProperty(Name = "Health", Access = ScriptPropertyAccess.Get)]
        private double ScriptHealth => Health;

        [ScriptProperty(Name = "HealthPercent", Access = ScriptPropertyAccess.Get)]
        private double ScriptHealthPercent => ((double)Health / (double)MaxHealth) * 100;

        [ScriptProperty(Name = "MaxHealth", Access = ScriptPropertyAccess.Get)]
        private double ScriptMaxHealth => MaxHealth;

        [ScriptProperty(Name = "Oxygen", Access = ScriptPropertyAccess.Get)]
        private double ScriptOxygen => Oxygen;

        [ScriptProperty(Name = "OxygenPercent", Access = ScriptPropertyAccess.Get)]
        private double ScriptOxygenPercent => ((double)Oxygen / (double)MaxOxygen) * 100;

        [ScriptProperty(Name = "MaxOxygen", Access = ScriptPropertyAccess.Get)]
        private double ScriptMaxOxygen => MaxOxygen;

        [ScriptProperty(Name = "Stamina", Access = ScriptPropertyAccess.Get)]
        private double ScriptStamina => Stamina;

        [ScriptProperty(Name = "StaminaPercent", Access = ScriptPropertyAccess.Get)]
        private double ScriptStaminaPercent => ((double)Stamina / (double)MaxStamina) * 100;

        [ScriptProperty(Name = "MaxStamina", Access = ScriptPropertyAccess.Get)]
        private double ScriptMaxStamina => MaxStamina;

        [ScriptProperty(Name = "Name", Access = ScriptPropertyAccess.Get)]
        protected virtual string ScriptName => Name;

        [ScriptProperty(Name = "Clan", Access = ScriptPropertyAccess.Get)]
        protected virtual string ScriptClanName => null;

        [ScriptProperty(Name = "Reach", Access = ScriptPropertyAccess.Get)]
        private double ScriptReach => Reach;

        [ScriptMethod(Name = "GetAction")]
        protected virtual int ScriptGetAction(string item, string action)
        {
            return 0;
        }

        [ScriptMethod(Name = "GetEquippedItem")]
        private string ScriptGetEquippedItem(string slot)
        {
            return slot switch
            {
                "left" => LeftHand.Item.Definition.ItemId,
                "right" => RightHand.Item.Definition.ItemId,
                "head" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart].ItemID].IDString,
                "neck" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 1].ItemID].IDString,
                "body" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 2].ItemID].IDString,
                "legs" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 3].ItemID].IDString,
                "feet" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 4].ItemID].IDString,
                "leftside" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 5].ItemID].IDString,
                "rightside" => Globals1.ItemData[(int)Inventory.Items[Inventory.EquipIndexStart + 6].ItemID].IDString,
                _ => null
            };
        }

        [ScriptMethod(Name = "HasPermission")]
        protected bool ScriptHasPermission(string permission)
        {
            return permission switch
            {
                "Adventure" => TMActor.HasPermission(Permissions.Adventure),
                "Edit" => TMActor.HasPermission(Permissions.Edit),
                "Creative" => TMActor.HasPermission(Permissions.Creative),
                "Fly" => TMActor.HasPermission(Permissions.Fly),
                "Map" => TMActor.HasPermission(Permissions.Map),
                "Save" => TMActor.HasPermission(Permissions.Save),
                "Admin" => TMActor.HasPermission(Permissions.Admin),
                "Grief" => TMActor.HasPermission(Permissions.Grief),
                "VoiceChat" => TMActor.HasPermission(Permissions.VoiceChat),
                "Spectate" => TMActor.HasPermission(Permissions.Spectate),
                "SystemShops" => TMActor.HasPermission(Permissions.SystemShops),
                "ViewScripts" => TMActor.HasPermission(Permissions.ViewScripts),
                "TextChat" => TMActor.HasPermission(Permissions.TextChat),
                _ => false
            };
        }

        [ScriptMethod(Name = "HasAnyPermission")]
        protected bool ScriptHasAnyPermission(IScriptRuntime runtime, ScriptArrayVar permissions)
        {
            foreach (ScriptVar permission in permissions)
            {
                bool result = permission.GetStringValue(runtime.Reference) switch
                {
                    "Adventure" => TMActor.HasPermission(Permissions.Adventure),
                    "Edit" => TMActor.HasPermission(Permissions.Edit),
                    "Creative" => TMActor.HasPermission(Permissions.Creative),
                    "Fly" => TMActor.HasPermission(Permissions.Fly),
                    "Map" => TMActor.HasPermission(Permissions.Map),
                    "Save" => TMActor.HasPermission(Permissions.Save),
                    "Admin" => TMActor.HasPermission(Permissions.Admin),
                    "Grief" => TMActor.HasPermission(Permissions.Grief),
                    "VoiceChat" => TMActor.HasPermission(Permissions.VoiceChat),
                    "Spectate" => TMActor.HasPermission(Permissions.Spectate),
                    "SystemShops" => TMActor.HasPermission(Permissions.SystemShops),
                    "ViewScripts" => TMActor.HasPermission(Permissions.ViewScripts),
                    "TextChat" => TMActor.HasPermission(Permissions.TextChat),
                    _ => false
                };

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

#pragma warning restore IDE0051

        #endregion

        #region ICoreActor

        public ActorState ActorState => TMActor.ActorState;
        public string Name => TMActor.Name;
        public abstract GamerID Id { get; }
        public Vector3 Position { get => TMActor.Position; set => TMActor.Position = value; }
        public Vector3 EyeOffset { get => TMActor.EyeOffset; set => TMActor.EyeOffset = value; }
        public Vector3 EyePosition => TMActor.EyePosition;
        public float FOV { get => TMActor.FOV; set => TMActor.FOV = value; }
        public Vector3 Velocity { get => TMActor.Velocity; set => TMActor.Velocity = value; }
        public Vector2 MaxVelocity { get => TMActor.MaxVelocity; set => TMActor.MaxVelocity = value; }
        public Vector3 ViewDirection { get => TMActor.ViewDirection; set => TMActor.ViewDirection = value; }
        public Matrix ViewMatrix => TMActor.ViewMatrix;
        public Matrix ViewMatrixLocal => TMActor.ViewMatrixLocal;
        public Matrix ProjectionMatrix { get => TMActor.ProjectionMatrix; set => TMActor.ProjectionMatrix = value; }
        public BoundingFrustum Frustum => TMActor.Frustum;
        public BoundingBox HitBoundingBox => TMActor.Box;
        public float SpeedOverride { get => TMActor.SpeedOverride; set => TMActor.SpeedOverride = value; }
        public float GravityOverride { get => TMActor.GravityOverride; set => TMActor.GravityOverride = value; }
        public bool OverrideControlInput { get => TMActor.OverrideControlInput; set => TMActor.OverrideControlInput = value; }
        ICoreHand ICoreActor.LeftHand => LeftHand;
        ICoreHand ICoreActor.RightHand => RightHand;
        public ITMInventory Inventory => TMActor.Inventory;
        public AudioEmitter AudioEmitter => TMActor.AudioEmitter;
        public float Oxygen { get => TMActor.Oxygen; set => TMActor.Oxygen = value; }
        public float MaxOxygen => TMActor.MaxOxygen;
        public float Stamina { get => TMActor.Stamina; set => TMActor.Stamina = value; }
        public float MaxStamina => TMActor.MaxStamina;
        public float Health { get => TMActor.Health; set => TMActor.Health = value; }
        public float MaxHealth => TMActor.MaxHealth;
        public FlyMode FlyMode { get => TMActor.FlyMode; set => TMActor.FlyMode = value; }
        public int Reach { get => TMActor.Reach; set => TMActor.Reach = value; }
        public bool IsPlayer => TMActor.IsPlayer;
        public bool Grounded => TMActor.IsOnGround;
        public bool IsCrouching => TMActor.IsCrouching;
        public bool IsActive => !TMActor.IsDeadOrInactiveOrDisabled;
        public int AddToInventory(InventoryItem item) => TMActor.AddToInventory(item);
        public int AddToInventory(InventoryItem item, out int slotId) => TMActor.AddToInventory(item, out slotId);
        public bool EquipFromInventory(CoreItem item) => TMActor.EquipFromInventory(item.ItemType);
        public bool EquipFromInventory(ICoreHand hand, CoreItem item) => TMActor.EquipFromInventory(hand.TMHand, item.ItemType);
        public bool UnequipToInventory(EquipIndex equipIndex) => TMActor.UnequipToInventory(equipIndex);
        public bool IsItemEquipped(CoreItem item) => TMActor.IsItemEquipped(item.ItemType);
        public bool IsItemEquippedAndUsable(CoreItem item) => TMActor.IsItemEquippedAndUsable(item.ItemType);
        public int GetItemEquippedSlot(CoreItem item) => TMActor.GetItemEquippedSlot(item.ItemType);
        public void DropItem(int slotId) => TMActor.DropItem(slotId);
        public bool ChangeState(ActorState newState) => TMActor.ChangeState(newState);
        public float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce) => TMActor.TakeDamageAndDisplay(damageType, damage, knockForce);
        public float TakeDamageAndDisplay(DamageType damageType, float damage, Vector3 knockForce, ICoreActor attacker, CoreItem weapon, SkillType attackType) => TMActor.TakeDamageAndDisplay(damageType, damage, knockForce, attacker.TMActor, weapon.ItemType, attackType);
        public bool HasPermission(Permissions permissions) => TMActor.HasPermission(permissions);
        public bool HasPermissionAny(Permissions permissions) => TMActor.HasPermissionAny(permissions);
        public void TogglePermission(Permissions permission, bool enable) => TMActor.TogglePermission(permission, enable);
        public bool HasHistory(string key) => TMActor.HasHistory(key);
        public bool LineOfSightTest(Vector3 dir, float distance) => TMActor.LineOfSightTest(dir, distance);
        public void TeleportTo(Vector3 pos) => TMActor.TeleportTo(pos);
        public void UpdateMatrices() => TMActor.UpdateMatrices();

        #endregion

        public ActorModel Model { get; protected set; }
        public AnimationController Animation { get; protected set; }

        public bool CheckHit(Ray ray, float maxDistance, out float distance, out float damageMultiplier, out bool isCritical)
        {
            float? d = ray.Intersects(TMActor.Box);
            if (d.HasValue && d.Value <= maxDistance)
            {
                distance = d.Value;
                damageMultiplier = 1;
                isCritical = false;
                return true;
            }
            distance = 0;
            damageMultiplier = 0;
            isCritical = false;
            return false;
        }

        public bool CheckHit(BoundingBox box, out float damageMultiplier, out bool isCritical)
        {
            if (box.Intersects(TMActor.Box))
            {
                damageMultiplier = 1;
                isCritical = false;
                return true;
            }
            damageMultiplier = 0;
            isCritical = false;
            return false;
        }

        public bool CheckHit(BoundingSphere sphere, out float damageMultiplier, out bool isCritical)
        {
            if (sphere.Intersects(TMActor.Box))
            {
                damageMultiplier = 1;
                isCritical = false;
                return true;
            }
            damageMultiplier = 0;
            isCritical = false;
            return false;
        }

        public void ChangeHealth(float amount, bool triggerEvents)
        {
            if (amount > 0 && Health < MaxHealth)
            {
                TMActor.Health += amount;
                if (TMActor.Health > MaxHealth)
                {
                    TMActor.Health = MaxHealth;
                }

                if (triggerEvents)
                {
                    OnHeal(amount, null, TMItems.None);
                }
            }
            else if (amount < 0)
            {
                TMActor.Health += amount;
                if (TMActor.Health <= 0)
                {
                    _dieInvoker(TMActor, DamageType.Unknown, null, Item.None, -amount);
                }
                
                if (triggerEvents)
                {
                    OnHurt(DamageType.Unknown, null, TMItems.None, 0);
                }
            }
        }

        public void Kill()
        {
            Kill(DamageType.Unknown, null, TMItems.None, 0);
        }

        public void Kill(DamageType deathType, ICoreActor attacker, CoreItem weapon, float damage)
        {
            _dieInvoker(TMActor, deathType, attacker.TMActor, weapon.ItemType, damage);
        }

        public virtual void OnDeath(DamageType deathType, ICoreActor attacker, CoreItem weapon, float damage)
        {
            if (attacker != null)
            {
                attacker.OnKill(deathType, this, weapon, damage);
            }

            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostKilled(attacker, deathType, weapon, damage);
                }
            }
        }

        public virtual void OnKill(DamageType deathType, ICoreActor target, CoreItem weapon, float damage)
        {
            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostKillTarget(target, deathType, weapon, damage);
                }
            }
        }

        public virtual void OnHurt(DamageType damageType, ICoreActor attacker, CoreItem weapon, float damage)
        {
            if (!Game.CombatEnabled)
            {
                return;
            }

            if (attacker != null)
            {
                attacker.OnAttack(damageType, this, weapon, damage);
                foreach (ICoreData<ICoreActor> data in Data)
                {
                    if (data is ActorData actorData)
                    {
                        actorData.PostAttacked(attacker, damageType, weapon, damage, true);
                    }
                }
            }

            if (damage > 0)
            {
                foreach (ICoreData<ICoreActor> data in Data)
                {
                    if (data is ActorData actorData)
                    {
                        actorData.PostHurt(attacker, damageType, weapon, damage, Health <= 0);
                    }
                }
            }
        }

        public virtual void OnAttack(DamageType damageType, ICoreActor target, CoreItem weapon, float damage)
        {
            if (!Game.CombatEnabled)
            {
                return;
            }

            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostAttackTarget(target, damageType, weapon, damage, target.Health <= 0);
                }
            }
        }

        public virtual void OnHeal(float health, ICoreActor healer, CoreItem item)
        {
            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostHeal(healer, item, health);
                }
            }
        }

        public void OnSwingStart(ICoreHand hand, CoreItem item)
        {
            SwingTime time = item.GetSwingTime(SwingState.None);
            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostSwingStart(hand, item, time);
                }
            }
        }

        public void OnSwingExtended(ICoreHand hand, CoreItem item)
        {
            SwingTime time = item.GetSwingTime(SwingState.Extended);
            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostSwingExtend(hand, item, time);
                }
            }
        }

        public void OnSwingEnd(ICoreHand hand, CoreItem item)
        {
            SwingTime time = item.GetSwingTime(SwingState.Complete);
            foreach (ICoreData<ICoreActor> data in Data)
            {
                if (data is ActorData actorData)
                {
                    actorData.PostSwingEnd(hand, item, time);
                }
            }
        }

        public bool PlayAnimation(string id)
        {
            if (Model == null)
            {
                return false;
            }

            return Animation.PlayAnimation(id);
        }

        public bool IsOnGround(float coyoteTime)
        {
            return Grounded || _timeOffGround < coyoteTime;
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (Model != null)
            {
                World.ActorRenderer.AddActorToRender(this);
            }

            if (!Grounded)
            {
                _timeOffGround += Services.ElapsedTime;
            }
            else
            {
                _timeOffGround = 0;
            }
            Animation?.Update();
            UpdateCore();
        }

        protected virtual void UpdateCore()
        {

        }

        public T GetData<T>() where T : ICoreData<ICoreActor> => Data.GetData<T>();
        public bool TryGetData<T>(out T result) where T : ICoreData<ICoreActor> => Data.TryGetData(out result);
        public void GetAllData(List<ICoreData<ICoreActor>> result) => Data.GetAllData(result);
        public bool HasData<T>() => Data.HasData<T>();
        public void SetData(ICoreData<ICoreActor> data) => Data.SetData(data);
        public void SetData<T>(T data) where T : ICoreData<ICoreActor> => Data.SetData(data);
        public T SetData<T>() where T : ICoreData<ICoreActor>, new() => Data.SetData<T>();
        public ICoreData<ICoreActor> SetDefaultData(ICoreData<ICoreActor> data) => Data.SetDefaultData(data);
        public T SetDefaultData<T>(T data) where T : ICoreData<ICoreActor> => Data.SetDefaultData(data);
        public T SetDefaultData<T>() where T : ICoreData<ICoreActor>, new() => Data.SetDefaultData<T>();
        public IEnumerator<ICoreData<ICoreActor>> GetDataEnumerator() => ((IHasCoreData<ICoreActor>)Data).GetDataEnumerator();

        public Actor(ICoreGame game, ICoreWorld world, ITMActor actor)
        {
            TMActor = actor;
            Game = game;
            World = world;
            LeftHand = new Hand(this, actor.LeftHand);
            RightHand = new Hand(this, actor.RightHand);
            Data = new CoreDataCollection<ICoreActor>(this);
        }
    }
}
