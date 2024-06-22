using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    // We patch DoItemSpecificSwingComplete instead of OnSwingComplete,
    // since OnSwingComplete can change the item equipped, and we want
    // to pass the item originally swung to the event.
    [Patch("StudioForge.TotalMiner.Hand", "DoItemSpecificSwingComplete")]
    internal static class ActorSwingEndPatch
    {
        public static void Postfix(object __instance, InventoryHand ___HandType, Item ___ItemID)
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            ICoreActor owner = ((ITMHand)__instance).Owner.GetCoreActor();
            ICoreHand hand = ___HandType == InventoryHand.Left ? owner.LeftHand : owner.RightHand;
            CoreItem item = owner.Game.ItemRegistry[___ItemID];
            SwingTime time = item.GetSwingTime(SwingState.Complete);

            var enumerator = owner.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData data)
                {
                    data.PostSwingEnd(hand, item, time);
                }
            }
        }
    }
}
