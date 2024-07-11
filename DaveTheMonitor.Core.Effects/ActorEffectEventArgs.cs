namespace DaveTheMonitor.Core.Effects
{
    public readonly struct ActorEffectEventArgs
    {
        public ActorEffect Effect { get; private init; }

        public ActorEffectEventArgs(ActorEffect effect)
        {
            Effect = effect;
        }
    }
}
