using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Actor", "TakeDamageAndDisplay",
        "StudioForge.TotalMiner.DamageType",
        "System.Single",
        "Microsoft.Xna.Framework.Vector3",
        "StudioForge.TotalMiner.Actor",
        "StudioForge.TotalMiner.Item",
        "StudioForge.BlockWorld.SkillType")]
    internal static class ActorTakeDamagePatch
    {
        public static void Postfix(object __instance, DamageType damageType, float damage, Vector3 knockForce, ITMActor attacker, Item weaponID, SkillType attackType)
        {
            if (!CorePlugin.IsValid || !CorePlugin.Instance.Game.CombatEnabled)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreAttacker = attacker?.GetCoreActor();
            actor.OnHurt(damageType, coreAttacker, actor.Game.ItemRegistry.GetItem(weaponID), damage);
        }
    }
}
