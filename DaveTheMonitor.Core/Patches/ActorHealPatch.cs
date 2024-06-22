using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
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
            if (!__result || !CorePlugin.IsValid)
            {
                return;
            }

            ICoreActor actor = ((ITMActor)__instance).GetCoreActor();
            ICoreActor coreHealer = healer?.GetCoreActor();
            float health = Globals1.ItemData[(int)itemID].HealPower;
            CoreItem item = actor.Game.ItemRegistry[itemID];

            var enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData data)
                {
                    data.PostHeal(coreHealer, item, health);
                }
            }
        }
    }
}
