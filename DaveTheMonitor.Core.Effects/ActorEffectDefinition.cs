using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DaveTheMonitor.Core.Effects
{
    public abstract class ActorEffectDefinition : IDefinition
    {
        public abstract string Id { get; }
        public int NumId { get; set; }
        public abstract ActorEffectType Type { get; }
        public abstract Texture2D BackgroundTexture { get; }
        public abstract Texture2D IconTexture { get; }
        public bool ShouldDisplay => BackgroundTexture != null && IconTexture != null;
        protected ICoreGame Game { get; private set; }

        public virtual void OnRegister(ICoreMod mod)
        {

        }

        public void SetGame(ICoreGame game)
        {
            Game = game;
        }

        public abstract string GetName(ActorEffect effect);
        public abstract string GetDescription(ActorEffect effect);
        public abstract Rectangle GetBackgroundSrc(ActorEffect effect);
        public abstract Rectangle GetIconSrc(ActorEffect effect);

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
