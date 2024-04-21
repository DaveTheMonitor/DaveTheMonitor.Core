using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorHurtEventArgs
    {
        public ICoreActor Actor { get; private set; }
        public ICoreActor Attacker { get; private set; }
        public DamageType DamageType { get; private set; }
        public CoreItem Weapon { get; private set; }
        public float Damage { get; private set; }
        public bool Fatal { get; private set; }

        public CoreActorHurtEventArgs(ICoreActor actor, ICoreActor attacker, DamageType damageType, CoreItem weapon, float damage, bool fatal)
        {
            Actor = actor;
            Attacker = attacker;
            DamageType = damageType;
            Weapon = weapon;
            Damage = damage;
            Fatal = fatal;
        }
    }
}
