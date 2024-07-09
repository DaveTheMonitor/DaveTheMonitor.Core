using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches
{
    [Patch]
    internal static class ActorDeathPatch
    {
        public static IEnumerable<MethodInfo> TargetMethods()
        {
            return new MethodInfo[]
            {
                AccessTools.Method("StudioForge.TotalMiner.Actor:Die"),
                AccessTools.Method("StudioForge.TotalMiner.Player:Die")
            };
        }

        public static void Postfix(object __instance, DamageType deathType, ITMActor attacker, Item weaponID, float damage)
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreAttacker = attacker?.GetCoreActor();
            CoreItem item = actor.Game.ItemRegistry[weaponID];
            AttackInfo attack = new AttackInfo(damage, deathType, Vector3.Zero);

            var enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData data)
                {
                    data.PostDeath(coreAttacker, item, attack);
                }
            }

            if (coreAttacker != null)
            {
                enumerator = coreAttacker.GetDataEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is ActorData data)
                    {
                        data.PostKillTarget(actor, item, attack);
                    }
                }
            }
        }
    }
}
