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
        public static void Prefix(object __instance, ref DamageType damageType, ref float damage, ref Vector3 knockForce, ITMActor attacker, Item weaponID, SkillType attackType)
        {
            if (!CorePlugin.IsValid || !CorePlugin.Instance.Game.CombatEnabled)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreAttacker = attacker?.GetCoreActor();
            CoreItem item = actor.Game.ItemRegistry[weaponID];
            AttackInfo attack = new AttackInfo(damage, damageType, knockForce);

            var enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData actorData)
                {
                    AttackInfo? result = actorData.PreHurt(actor, item, attack, attack.Damage >= actor.Health);
                    if (result.HasValue)
                    {
                        attack = result.Value;
                    }
                }
            }

            damage = attack.Damage;
            damageType = attack.DamageType;
            knockForce = attack.KnockForce;
        }

        public static void Postfix(object __instance, DamageType damageType, float damage, Vector3 knockForce, ITMActor attacker, Item weaponID, SkillType attackType)
        {
            if (!CorePlugin.IsValid || !CorePlugin.Instance.Game.CombatEnabled)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreAttacker = attacker?.GetCoreActor();
            CoreItem item = actor.Game.ItemRegistry[weaponID];
            AttackInfo attack = new AttackInfo(damage, damageType, knockForce);
            bool fatal = actor.Health <= 0;

            var enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData data)
                {
                    data.PostHurt(coreAttacker, item, attack, fatal);
                }
            }
        }
    }
}
