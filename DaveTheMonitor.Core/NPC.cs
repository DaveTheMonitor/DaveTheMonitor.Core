using DaveTheMonitor.Core.API;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core
{
    internal sealed class NPC : Actor, ICoreNpc
    {
        public override GamerID Id => ((IActorBehaviour)TMActor).GamerID;

        public NPC(ICoreGame game, ICoreWorld world, ITMActor actor) : base(game, world, actor)
        {

        }
    }
}
