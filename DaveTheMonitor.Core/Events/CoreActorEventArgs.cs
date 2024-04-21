using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorEventArgs
    {
        public ICoreActor Actor { get; private set; }

        public CoreActorEventArgs(ICoreActor actor)
        {
            Actor = actor;
        }
    }
}
