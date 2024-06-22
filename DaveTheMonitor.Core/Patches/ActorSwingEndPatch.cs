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
            owner.OnSwingEnd(hand, owner.Game.ItemRegistry.GetItem(___ItemID));
        }
    }
}
