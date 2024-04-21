﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Hand", "OnSwingStart")]
    internal static class ActorSwingStartPatch
    {
        public static void Postfix(object __instance, InventoryHand ___HandType, Item ___ItemID, object sender, EventArgs e)
        {
            ICoreActor owner = ((ITMHand)__instance).Owner.GetCoreActor();
            ICoreHand hand = ___HandType == InventoryHand.Left ? owner.LeftHand : owner.RightHand;
            owner.OnSwingStart(hand, owner.Game.ItemRegistry.GetItem(___ItemID));
        }
    }
}
