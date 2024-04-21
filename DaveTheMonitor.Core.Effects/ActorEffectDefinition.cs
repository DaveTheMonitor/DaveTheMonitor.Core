using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DaveTheMonitor.Core.Effects
{
    public abstract class ActorEffectDefinition : IDefinition
    {
        public abstract string Id { get; }
        public int NumId { get; set; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ActorEffectType Type { get; }
        public abstract Texture2D BackgroundTexture { get; }
        public abstract Texture2D IconTexture { get; }
        public abstract Rectangle BackgroundSrc { get; }
        public abstract Rectangle IconSrc { get; }
        public bool ShouldDisplay => BackgroundTexture != null && IconTexture != null;
        protected ICoreGame Game { get; private set; }

        public virtual void OnRegister(ICoreMod mod)
        {

        }

        public void SetGame(ICoreGame game)
        {
            Game = game;
        }

        public virtual void Update(ActorEffect effect)
        {
            
        }

        public virtual void EffectAdded(ActorEffect effect)
        {

        }

        public virtual void EffectRemoved(ActorEffect effect)
        {

        }
    }
}
