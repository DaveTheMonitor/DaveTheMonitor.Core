using StudioForge.TotalMiner;
using System.IO;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Base class for actor data that can listen to events. If your data doesn't require any events, consider using <see cref="ICoreData{T}"/> instead.
    /// </summary>
    public abstract class ActorData : ICoreData<ICoreActor>
    {
        /// <inheritdoc/>
        public abstract bool ShouldSave { get; }
        /// <inheritdoc/>
        public virtual int Priority => 100;
        
        /// <summary>
        /// The actor this data belongs to.
        /// </summary>
        public ICoreActor Actor { get; private set; }

        /// <summary>
        /// The main game instance.
        /// </summary>
        public ICoreGame Game => Actor.Game;

        /// <summary>
        /// The world this actor is in.
        /// </summary>
        public ICoreWorld World => Actor.World;

        /// <inheritdoc/>
        public void Initialize(ICoreActor item)
        {
            Actor = item;
            InitializeCore();
        }

        /// <summary>
        /// Called when this data is initialized.
        /// </summary>
        public abstract void InitializeCore();

        /// <inheritdoc/>
        public virtual void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            
        }

        /// <inheritdoc/>
        public virtual void WriteState(BinaryWriter writer)
        {
            
        }

        public virtual AttackInfo? PreAttacked(ICoreActor attacker, CoreItem weapon, AttackInfo attack, bool fatal)
        {
            return null;
        }

        /// <summary>
        /// Called after this actor is attacked by another actor.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="weapon">The weapon used.</param>
        /// <param name="attack">The attack.</param>
        /// <param name="fatal">True if this damage resulted in the death of the actor.</param>
        public virtual void PostAttacked(ICoreActor attacker, CoreItem weapon, AttackInfo attack, bool fatal)
        {

        }

        public virtual AttackInfo? PreHurt(ICoreActor attacker, CoreItem weapon, AttackInfo attack, bool fatal)
        {
            return null;
        }

        /// <summary>
        /// Called after this actor is hurt by any damage.
        /// </summary>
        /// <param name="attacker">The attacker, or null if there is no attacker.</param>
        /// <param name="weapon">The weapon used, or <see cref="TMItems.None"/> if no weapon was used.</param>
        /// <param name="attack">The attack.</param>
        /// <param name="fatal">True if this damage resulted in the death of the actor.</param>
        public virtual void PostHurt(ICoreActor attacker, CoreItem weapon, AttackInfo attack, bool fatal)
        {

        }

        /// <summary>
        /// Called after this actor is killed.
        /// </summary>
        /// <param name="attacker">The attacker, or null if there is no attacker.</param>
        /// <param name="weapon">The weapon used, or <see cref="TMItems.None"/> if no weapon was used.</param>
        /// <param name="attack">The attack.</param>
        public virtual void PostDeath(ICoreActor attacker, CoreItem weapon, AttackInfo attack)
        {

        }

        public virtual AttackInfo? PreAttackTarget(ICoreActor target, CoreItem weapon, AttackInfo attack, bool fatal)
        {
            return null;
        }

        /// <summary>
        /// Called after this actor attacks another actor.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="weapon">The weapon used.</param>
        /// <param name="attack">The attack.</param>
        /// <param name="fatal">True if this damage resulted in the death of the target.</param>
        public virtual void PostAttackTarget(ICoreActor target, CoreItem weapon, AttackInfo attack, bool fatal)
        {

        }

        /// <summary>
        /// Called after this actor kills another actor.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="weapon">The weapon used, or <see cref="TMItems.None"/> if no weapon was used.</param>
        /// <param name="attack">The attack.</param>
        public virtual void PostKillTarget(ICoreActor target, CoreItem weapon, AttackInfo attack)
        {

        }

        /// <summary>
        /// Called after this actor is healed.
        /// </summary>
        /// <param name="healer">The actor that healed this actor, or null if the actor wasn't heald by another actor. The healer may be itself.</param>
        /// <param name="item">The item used to heal this actor, or <see cref="TMItems.None"/> if no item was used.</param>
        /// <param name="health">The total health healed. This is not capped by the actor's max health.</param>
        public virtual void PostHeal(ICoreActor healer, CoreItem item, float health)
        {

        }

        /// <summary>
        /// Called after this actor starts a swing.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        /// <param name="swingTime">The current swing time.</param>
        public virtual void PostSwingStart(ICoreHand hand, CoreItem item, SwingTime swingTime)
        {

        }

        /// <summary>
        /// Called after this actor's swing is fully extended. This is when damage is dealt and swing events are triggered.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        /// <param name="swingTime">The current swing time.</param>
        public virtual void PostSwingExtend(ICoreHand hand, CoreItem item, SwingTime swingTime)
        {

        }

        /// <summary>
        /// Called after this actor's swing ends.
        /// </summary>
        /// <param name="hand">The hand that is swinging.</param>
        /// <param name="item">The item being swung.</param>
        /// <param name="swingTime">The current swing time.</param>
        public virtual void PostSwingEnd(ICoreHand hand, CoreItem item, SwingTime swingTime)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ActorData"/> instance.
        /// </summary>
        public ActorData()
        {

        }
    }
}
