using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core
{
    internal sealed class NPC : Actor, ICoreNpc
    {
        public override GamerID Id => ((IActorBehaviour)TMActor).GamerID;

        public override void EnterWorld(ICoreWorld world, Vector3 position)
        {
            if (World == world || world == null)
            {
                return;
            }

            // TODO: NPC support for other worlds?
            ChangeState(ActorState.InActive);
        }

        public override void EnterWorld(ICoreWorld world, Vector3 position, Vector3 viewDirection)
        {
            if (World == world || world == null)
            {
                return;
            }

            // TODO: NPC support for other worlds?
            ChangeState(ActorState.InActive);
        }

        public NPC(ICoreGame game, ICoreWorld world, ITMActor actor) : base(game, world, actor)
        {

        }
    }
}
