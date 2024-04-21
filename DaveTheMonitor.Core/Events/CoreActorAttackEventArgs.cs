using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorAttackEventArgs
    {
        public ICoreActor Actor { get; private set; }
        public ICoreActor Target { get; private set; }
        public CoreItem Weapon { get; private set; }
        public float Damage { get; private set; }
        public bool Fatal { get; private set; }

        public CoreActorAttackEventArgs(ICoreActor actor, ICoreActor target, CoreItem weapon, float damage, bool fatal)
        {
            Actor = actor;
            Target = target;
            Weapon = weapon;
            Damage = damage;
            Fatal = fatal;
        }
    }
}
