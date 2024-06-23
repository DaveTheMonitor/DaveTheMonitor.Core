using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Gui
{
    /// <summary>
    /// A slot that shows a single item in an inventory.
    /// </summary>
    public sealed class CoreInventorySlotWindow : Window
    {
        private static ConstructorInfo _slotWinCtor =
            AccessTools.Constructor(AccessTools.TypeByName("StudioForge.TotalMiner.Screens2.InventorySlotWin"), new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.Player"),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int),
                AccessTools.TypeByName("StudioForge.TotalMiner.Screens2.InventorySlotWinFlags"),
                AccessTools.TypeByName("StudioForge.TotalMiner.Inventory"),
                typeof(int),
                typeof(bool)
            });
        private static ConstructorInfo _inventoryCtor =
            AccessTools.Constructor(AccessTools.TypeByName("StudioForge.TotalMiner.Inventory"), new Type[]
            {
                typeof(int)
            });
        private static Action<object, ITMPlayer> _refresh =
            AccessTools.Method("StudioForge.TotalMiner.Screens2.InventorySlotWin:Refresh").CreateInvoker<Action<object, ITMPlayer>>();
        private static Func<object, ITMInventory> _inventoryGetter =
            AccessTools.Method("StudioForge.TotalMiner.Screens2.InventorySlotWin:get_Inventory").CreateInvoker<Func<object, ITMInventory>>();
        private static Func<object, int> _slotIdGetter =
            AccessTools.Method("StudioForge.TotalMiner.Screens2.InventorySlotWin:get_SlotID").CreateInvoker<Func<object, int>>();
        private static Func<object, InventoryItem> _invItemGetter =
            AccessTools.Method("StudioForge.TotalMiner.Screens2.InventorySlotWin:get_InvItem").CreateInvoker<Func<object, InventoryItem>>();
        private static Action<ITMInventory, InventoryItem, int> _flagItemChanged =
            AccessTools.Method("StudioForge.TotalMiner.Inventory:FlagItemChanged").CreateInvoker<Action<ITMInventory, InventoryItem, int>>();
        private static AccessTools.FieldRef<object, bool> _hasItemsChanged =
            AccessTools.FieldRefAccess<bool>("StudioForge.TotalMiner.Inventory:HasItemsChanged");
        private static Func<ITMInventory, InventoryItem, int, int, bool, int> _transfer =
            AccessTools.Method("StudioForge.TotalMiner.Inventory:TransferTo", new Type[]
            {
                typeof(InventoryItem),
                typeof(int),
                typeof(int),
                typeof(bool)
            }).CreateInvoker<Func<ITMInventory, InventoryItem, int, int, bool, int>>();

        /// <summary>
        /// The item currently in this slot.
        /// </summary>
        public InventoryItem Item
        {
            get => _inventory.Items.Count > _slot ? _inventory.Items[_slot] : InventoryItem.Empty;
            set
            {
                _inventory.Items.EnsureCapacity(_slot);
                _inventory.Items[_slot] = value;
            }
        }

        /// <summary>
        /// Used to only allow specific items to be placed in this slot.
        /// </summary>
        public CanMoveItemFunc IsItemValidFunc { get; set; }

        private ICorePlayer _player;
        private ITMInventory _inventory;
        private int _slot;
        private TextBox _slotWin;

        /// <summary>
        /// Refreshes this slot display. Should be called if the items in the inventory change.
        /// </summary>
        public void Refresh()
        {
            _refresh(_slotWin, _player.TMPlayer);
        }

        private bool DragEnd(object sender, WindowDragEventArgs e)
        {
            Window targetWin = e.Hovered;
            if (targetWin.GetType() != _slotWin.GetType())
            {
                Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
                return true;
            }

            if (targetWin == e.Window)
            {
                Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
                return true;
            }

            InventoryItem item = Item;
            if (targetWin.Parent is CoreInventorySlotWindow targetInvSlotWin)
            {
                if (!targetInvSlotWin.IsItemValidForSlot(item))
                {
                    Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
                    return true;
                }
            }

            bool split = e.RightButton;
            bool placeOne = e.CTDRightClick;
            int countToTransfer;
            if (split)
            {
                countToTransfer = item.Count / 2;
                if (countToTransfer == 0 && item.Count > 0)
                {
                    countToTransfer = 1;
                }
            }
            else if (placeOne)
            {
                countToTransfer = 1;
            }
            else
            {
                countToTransfer = item.Count;
            }

            ITMInventory targetInventory = _inventoryGetter(targetWin);

            InventoryItem targetItem = _invItemGetter(targetWin);
            int targetSlot = _slotIdGetter(targetWin);
            if (split)
            {
                if (!CanStackWith(targetItem))
                {
                    Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
                    return true;
                }
            }
            else if (!CanStackWith(targetItem))
            {
                targetInventory.Items[targetSlot] = item;
                Item = targetItem;
                _refresh(_slotWin, _player.TMPlayer);
                _refresh(targetWin, _player.TMPlayer);
                _flagItemChanged(_inventory, item, _slot);
                _hasItemsChanged(_inventory) = true;
                _flagItemChanged(targetInventory, targetItem, targetSlot);
                _hasItemsChanged(targetInventory) = true;
                Sounds.PlaySound(ItemSoundGroup.GuiTransfer);
                return true;
            }

            InventoryItem oldItem = item;
            int count = _transfer(targetInventory, item, countToTransfer, targetSlot, false);
            item.Count -= count;
            Item = item;
            _refresh(_slotWin, _player.TMPlayer);
            _refresh(targetWin, _player.TMPlayer);
            _flagItemChanged(_inventory, oldItem, _slot);
            _hasItemsChanged(_inventory) = true;
            _flagItemChanged(targetInventory, targetItem, targetSlot);
            _hasItemsChanged(targetInventory) = true;
            Sounds.PlaySound(ItemSoundGroup.GuiTransfer);
            return item.Count <= 0 || split;
        }

        /// <summary>
        /// Returns true if the specified item can be placed in this slot.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>True if the item can be placed in this slot, otherwise false.</returns>
        public bool IsItemValidForSlot(InventoryItem item)
        {
            return IsItemValidFunc?.Invoke(this, item, Item) ?? true;
        }

        private bool CanStackWith(InventoryItem item)
        {
            return item.ItemID == StudioForge.TotalMiner.Item.None || item.ItemID == Item.ItemID;
        }

        /// <summary>
        /// Creates a new inventory slot window that can hold a single <see cref="InventoryItem"/>. The item will be destroyed when the slot is removed.
        /// </summary>
        /// <param name="x">The X position of this slot.</param>
        /// <param name="y">The Y position of this slot.</param>
        /// <param name="width">The width of this slot.</param>
        /// <param name="height">The height of this slot.</param>
        /// <param name="player">The player that opened the menu.</param>
        public CoreInventorySlotWindow(int x, int y, int width, int height, ICorePlayer player) : this(x, y, width, height, player, (ITMInventory)_inventoryCtor.Invoke(new object[] { 1 }), 0)
        {

        }

        /// <summary>
        /// Creates a new inventory slot window that can hold a single <see cref="InventoryItem"/>. The item will be destroyed when the slot is removed.
        /// </summary>
        /// <param name="x">The X position of this slot.</param>
        /// <param name="y">The Y position of this slot.</param>
        /// <param name="width">The width of this slot.</param>
        /// <param name="height">The height of this slot.</param>
        /// <param name="player">The player that opened the menu.</param>
        /// <param name="inventory">The inventory that this slot is for.</param>
        /// <param name="slot">The ID of the slot to show.</param>
        public CoreInventorySlotWindow(int x, int y, int width, int height, ICorePlayer player, ITMInventory inventory, int slot) : this(x, y, width, height, CoreInventorySlotWindowFlags.ShowQuantity, player, inventory, slot)
        {
            
        }

        /// <summary>
        /// Creates a new inventory slot window that can hold a single <see cref="InventoryItem"/>. The item will be destroyed when the slot is removed.
        /// </summary>
        /// <param name="x">The X position of this slot.</param>
        /// <param name="y">The Y position of this slot.</param>
        /// <param name="width">The width of this slot.</param>
        /// <param name="height">The height of this slot.</param>
        /// <param name="flags">The flags for this slot.</param>
        /// <param name="player">The player that opened the menu.</param>
        /// <param name="inventory">The inventory that this slot is for.</param>
        /// <param name="slot">The ID of the slot to show.</param>
        public CoreInventorySlotWindow(int x, int y, int width, int height, CoreInventorySlotWindowFlags flags, ICorePlayer player, ITMInventory inventory, int slot)
            : base(x, y, width, height)
        {
            if (slot >= inventory.TotalSize)
            {
                throw new ArgumentOutOfRangeException(nameof(slot));
            }
            _player = player;
            _inventory = inventory;
            _slot = slot;
            _slotWin = (TextBox)_slotWinCtor.Invoke(new object[]
            {
                player.TMPlayer,
                0,
                0,
                width,
                height,
                (byte)flags,
                inventory,
                slot,
                true
            });
            _slotWin.DragEndHandler += DragEnd;
            Colors = TransparentColorProfile;
            AddChild(_slotWin);
            _slotWin.Colors = StudioForge.TotalMiner.Colors.InvIcon;
            RenderProfile = new RenderProfile()
            {
                Sampler = SamplerState.PointClamp
            };
            Refresh();
        }
    }
}
