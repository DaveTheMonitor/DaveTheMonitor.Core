using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Hand", "OnSwingFullyExtended")]
    internal static class ActorSwingExtendedPatch
    {
        public static void Postfix(object __instance, InventoryHand ___HandType, Item ___ItemID, object sender, EventArgs e)
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            ICoreActor owner = ((ITMHand)__instance).Owner.GetCoreActor();
            ICoreHand hand = ___HandType == InventoryHand.Left ? owner.LeftHand : owner.RightHand;
            owner.OnSwingExtended(hand, owner.Game.ItemRegistry.GetItem(___ItemID));
        }
    }
}
