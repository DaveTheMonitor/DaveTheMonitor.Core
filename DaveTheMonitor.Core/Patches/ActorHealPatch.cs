using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Actor", "Heal")]
    internal static class ActorHealPatch
    {
        public static void Postfix(object __instance, bool __result, ITMActor healer, Item itemID)
        {
            if (!__result)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreHealer = healer?.GetCoreActor();
            float health = Globals1.ItemData[(int)itemID].HealPower;
            actor.OnHeal(health, coreHealer, actor.Game.ItemRegistry.GetItem(itemID));
        }
    }
}
