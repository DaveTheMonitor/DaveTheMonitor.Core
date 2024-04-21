using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Effects
{
    public sealed class ActorEffectRegistry : DefinitionRegistry<ActorEffectDefinition>
    {
        protected override void OnRegister(ActorEffectDefinition definition)
        {
            definition.SetGame(Game);
        }

        public ActorEffectRegistry(ICoreGame game) : base(game, typeof(ActorEffectRegisterIgnoreAttribute))
        {

        }
    }
}
