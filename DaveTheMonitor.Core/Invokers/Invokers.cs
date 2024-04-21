using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Invokers
{
    internal delegate Rectangle GetHudPosInvoker(ITMPlayer player);
    internal delegate void ActorDieInvoker(ITMActor actor, DamageType deathType, ITMActor attacker, Item weaponID, float damage);
}
