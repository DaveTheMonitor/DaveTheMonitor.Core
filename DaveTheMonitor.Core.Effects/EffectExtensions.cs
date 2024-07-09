using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Effects
{
    public static class EffectExtensions
    {
        public static EffectData Effects(this ICoreActor actor)
        {
            return actor.GetData<EffectData>();
        }

        public static ActorEffect AddEffect(this ICoreActor actor, string id, float duration, bool allowMultiple)
        {
            return actor.Effects().Add(id, duration, allowMultiple);
        }

        public static ActorEffect AddEffect(this ICoreActor actor, ActorEffectDefinition definition, float duration, bool allowMultiple)
        {
            return actor.Effects().Add(definition, duration, allowMultiple);
        }

        public static bool RemoveEffect(this ICoreActor actor, ActorEffect effect)
        {
            return actor.Effects().Remove(effect);
        }

        public static bool RemoveEffect(this ICoreActor actor, string id)
        {
            return actor.Effects().Remove(id);
        }

        public static void ClearEffects(this ICoreActor actor)
        {
            actor.Effects().Clear();
        }

        public static bool HasEffect(this ICoreActor actor, string id)
        {
            return actor.Effects().HasEffect(id);
        }

        public static bool HasEffect(this ICoreActor actor, ActorEffectDefinition definition)
        {
            return actor.Effects().HasEffect(definition);
        }

        public static ActorEffect GetEffect(this ICoreActor actor, string id)
        {
            return actor.Effects().GetEffect(id);
        }

        public static ActorEffect GetEffect(this ICoreActor actor, ActorEffectDefinition definition)
        {
            return actor.Effects().GetEffect(definition);
        }

        public static bool TryGetEffect(this ICoreActor actor, string id, out ActorEffect effect)
        {
            return actor.Effects().TryGetEffect(id, out effect);
        }

        public static bool TryGetEffect(this ICoreActor actor, ActorEffectDefinition definition, out ActorEffect effect)
        {
            return actor.Effects().TryGetEffect(definition, out effect);
        }


        public static ActorEffectRegistry EffectRegistry(this ICoreGame game)
        {
            return game.GetData<EffectGameData>().EffectRegistry;
        }
    }
}
