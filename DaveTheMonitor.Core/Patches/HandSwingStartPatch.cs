﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Hand", "OnSwingStart")]
    internal static class HandSwingStartPatch
    {
        public static void Postfix(object __instance, InventoryHand ___HandType, Item ___ItemID, object sender, EventArgs e)
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            ICoreActor owner = ((ITMHand)__instance).Owner.GetCoreActor();
            ICoreHand hand = ___HandType == InventoryHand.Left ? owner.LeftHand : owner.RightHand;
            CoreItem item = owner.Game.ItemRegistry[___ItemID];
            SwingTime time = item.GetSwingTime(SwingState.None);

            var enumerator = owner.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData data)
                {
                    data.PostSwingStart(hand, item, time);
                }
            }
        }
    }
}
