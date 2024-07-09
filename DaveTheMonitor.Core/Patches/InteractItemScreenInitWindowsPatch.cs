using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Screens2.InteractItemScreen", "InitWindows")]
    internal static class InteractItemScreenInitWindowsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            list.ModifyFirst((ins, i) =>
            {
                if (ins.Calls(AccessTools.Method(AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.GuiHelper"), "MakeButton")))
                {
                    if (list[i - 3].LoadsConstant(8192))
                    {
                        return true;
                    }
                }
                return false;
            }, i =>
            {
                list.InsertRange(i + 1, new CodeInstruction[]
                {
                    CodeInstruction.LoadArgument(0),
                    CodeInstruction.LoadLocal(0),
                    CodeInstruction.LoadArgument(0),
                    CodeInstruction.LoadField(AccessTools.TypeByName("StudioForge.TotalMiner.Screens2.InteractItemScreen"), "player"),
                    CodeInstruction.LoadArgument(0),
                    CodeInstruction.LoadField(AccessTools.TypeByName("StudioForge.TotalMiner.Screens2.InteractItemScreen"), "item"),
                    CodeInstruction.LoadLocal(17),
                    CodeInstruction.LoadLocal(3),
                    CodeInstruction.LoadLocal(4),
                    CodeInstruction.LoadLocal(5),
                    CodeInstruction.Call(typeof(InteractItemScreenInitWindowsPatch), nameof(AddCustomActions))
                });
            });

            return list;
        }

        public static void AddCustomActions(GameScreen screen, Window container, ITMPlayer player, InventoryItem item, int x, int y, int w, int h)
        {
            if (!CorePlugin.IsValid)
            {
                return;
            }

            List<CoreItem.InspectAction> actions = item.CoreItem()._inspectActions;
            if (actions == null)
            {
                return;
            }

            // There's no way to know if the item is in
            // the player's inventory or a shop, so we
            // instead prevent executing the action if
            // we're in the shop screen and the player.
            // doesn't have the item in their inventory.
            GameScreen[] screens = screen.ScreenManager.GetScreens(player.PlayerIndex);
            Type shopScreenType = AccessTools.TypeByName("StudioForge.TotalMiner.Screens2.ShopMenu");
            foreach (GameScreen gameScreen in screens)
            {
                if (gameScreen is not INewGuiMenuScreen)
                {
                    continue;
                }

                if (new Traverse(gameScreen).Property("CurrentMenu").GetValue().GetType() != shopScreenType)
                {
                    continue;
                }

                bool found = false;
                foreach (InventoryItem invItem in player.Inventory.Items)
                {
                    if (invItem.ItemID == item.ItemID && invItem.Count == item.Count && invItem.Durability == item.Durability)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return;
                }
            }

            y += h + 4;
            foreach (CoreItem.InspectAction action in actions)
            {
                TextBox textbox = new TextBox(action.Text, x, y, w, h, 1, WinTextAlignX.Center, WinTextAlignY.Center);
                textbox.Colors = Colors.ButtonColors;
                textbox.ClickHandler += (object sender, WindowEventArgs e) =>
                {
                    action.Action(player.GetCorePlayer(), item);
                    screen.ExitScreen();
                };
                container.AddChild(textbox);
                y += h + 4;
            }
        }
    }
}
