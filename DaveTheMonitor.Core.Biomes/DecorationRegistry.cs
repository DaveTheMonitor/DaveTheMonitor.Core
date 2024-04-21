using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class DecorationRegistry : DefinitionRegistry<DecorationDefinition>
    {
        protected override void OnRegister(DecorationDefinition definition)
        {
            definition.SetGame(Game);
        }

        public DecorationRegistry(ICoreGame game) : base(game, null)
        {
            
        }
    }
}
