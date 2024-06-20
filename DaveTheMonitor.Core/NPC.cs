using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core
{
    internal sealed class NPC : Actor, ICoreNpc
    {
        public override GamerID Id => ((IActorBehaviour)TMActor).GamerID;
        private ActorAnimation _swing;
        private ActorAnimation _idle;
        private ActorAnimation _walk;

        protected override void UpdateCore()
        {
            
        }

        private void Damage()
        {
            Vector3 dir = ViewDirection;
            dir.Y = 0;
            dir.Normalize();
            BoundingSphere area = new BoundingSphere(Position + new Vector3(0, 1, 0) + (dir * 1.5f), 1f);
            List<ICoreActor> list = new List<ICoreActor>();
            World.ActorManager.GetActors(area, list);
            foreach (ICoreActor actor in list)
            {
                if (actor == this)
                {
                    continue;
                }

                if (actor.LeftHand.Item.TypeComponent.SubType == ItemSubType.Shield && actor.LeftHand.IsSwinging)
                {
                    actor.TakeDamageAndDisplay(DamageType.ShieldDeflect, 0, Vector3.Zero, this, TMItems.Hand, SkillType.Attack);
                }
                else
                {
                    actor.TakeDamageAndDisplay(DamageType.Combat, 30, Vector3.Zero, this, TMItems.Hand, SkillType.Attack);
                }
            }
        }

        public NPC(ICoreGame game, ICoreWorld world, ITMActor actor) : base(game, world, actor)
        {
            if (CoreActor.Model != null)
            {
                Model = CoreActor.Model;
                Animation = CoreActor.AnimationController.ToAnimationController(this);
                PlayAnimation(CoreActor.AnimationController.DefaultState);
            }
        }
    }
}
