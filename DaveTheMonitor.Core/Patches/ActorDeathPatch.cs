using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
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
            actor.OnDeath(deathType, coreAttacker, actor.Game.ItemRegistry.GetItem(weaponID), damage);
        }
    }
}
