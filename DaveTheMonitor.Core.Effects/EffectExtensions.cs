using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Effects
{
    public static class EffectExtensions
    {
        public static EffectData Effects(this ICoreActor actor)
        {
            return actor.GetData<EffectData>();
        }

        public static ActorEffectRegistry EffectRegistry(this ICoreGame game)
        {
            return game.GetData<EffectGameData>().EffectRegistry;
        }
    }
}
