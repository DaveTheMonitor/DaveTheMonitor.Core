using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreActorSwingEventArgs
    {
        public ICoreActor Actor { get; private set; }
        public SwingState SwingState { get; private set; }
        public ICoreHand Hand { get; private set; }
        public CoreItem Item { get; private set; }
        public SwingTime SwingTime { get; private set; }

        public CoreActorSwingEventArgs(ICoreActor actor, SwingState swingState, ICoreHand hand, CoreItem item, SwingTime swingTime)
        {
            Actor = actor;
            SwingState = swingState;
            Hand = hand;
            Item = item;
            SwingTime = swingTime;
        }
    }
}
