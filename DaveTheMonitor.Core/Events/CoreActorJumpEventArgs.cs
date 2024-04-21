using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorJumpEventArgs
    {
        public ICoreActor Actor { get; private set; }

        public CoreActorJumpEventArgs(ICoreActor actor)
        {
            Actor = actor;
        }
    }
}
