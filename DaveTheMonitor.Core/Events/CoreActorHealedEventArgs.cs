using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorHealedEventArgs
    {
        public ICoreActor Actor { get; private set; }
        public ICoreActor Healer { get; private set; }
        public CoreItem Item { get; private set; }
        public float Health { get; private set; }

        public CoreActorHealedEventArgs(ICoreActor actor, ICoreActor healer, CoreItem item, float health)
        {
            Actor = actor;
            Healer = healer;
            Item = item;
            Health = health;
        }
    }
}
