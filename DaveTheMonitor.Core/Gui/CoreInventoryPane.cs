using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.Gui
{
    /// <summary>
    /// An inventory pane that displays and allows moving items between inventories.
    /// </summary>
    public sealed class CoreInventoryPane : Window
    {
        /// <summary>
        /// The number of columns in this <see cref="CoreInventoryPane"/>.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// The number of rows in this <see cref="CoreInventoryPane"/>.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The inventory for this <see cref="CoreInventoryPane"/>.
        /// </summary>
        public ITMInventory Inventory { get; private set; }

        /// <summary>
        /// The number of items this <see cref="CoreInventoryPane"/> shows.
        /// </summary>
        public int ItemCount { get; private set; }
        private int _start;
        private int _end;
        private int _spacing;
        private ICorePlayer _player;
        private Window[] _windows;
        private CoreInventorySlotWindowFlags _flags;

        private void InitWindows(int slotWidth, int slotHeight)
        {
            _windows = new CoreInventorySlotWindow[ItemCount];
            int x = 0;
            int y = 0;
            int width = (Columns * slotWidth) + (_spacing * (Columns - 1));
            int height = (Rows * slotWidth) + (_spacing * (Rows - 1));
            Size = new Point(width, height);

            for (int i = 0; i < ItemCount; i++)
            {
                CoreInventorySlotWindow window = new CoreInventorySlotWindow(x, y, slotWidth, slotHeight, _flags, _player, Inventory, i);
                AddChild(window);
                _windows[i] = window;
                x += slotWidth + _spacing;
                if (i > 0 && ((i + 1) % Columns) == 0)
                {
                    x = 0;
                    y += slotHeight + _spacing;
                }
            }
        }

        /// <summary>
        /// Refreshes all slot windows in this inventory pane.
        /// </summary>
        public void Refresh()
        {
            foreach (CoreInventorySlotWindow win in _windows)
            {
                win.Refresh();
            }
        }

        /// <summary>
        /// Creates a new <see cref="CoreInventoryPane"/> for the specified inventory.
        /// </summary>
        /// <param name="x">The X position of the pane.</param>
        /// <param name="y">The Y position of the pane.</param>
        /// <param name="slotWidth">The width of the item slots, in pixels.</param>
        /// <param name="slotHeight">The height of the item slots, in pixels.</param>
        /// <param name="columns">The number of columns. The number of rows is automatically determined based on the inventory size.</param>
        /// <param name="spacing">The spacing between slots, in pixels.</param>
        /// <param name="player">The player that opened the menu.</param>
        /// <param name="inventory">The inventory this pane is for.</param>
        public CoreInventoryPane(int x, int y, int slotWidth, int slotHeight, int columns, int spacing, ICorePlayer player, ITMInventory inventory)
            : this(x, y, slotWidth, slotHeight, columns, spacing, player, inventory, CoreInventorySlotWindowFlags.ShowQuantity, 0, inventory.TotalSize)
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="CoreInventoryPane"/> for the specified inventory.
        /// </summary>
        /// <param name="x">The X position of the pane.</param>
        /// <param name="y">The Y position of the pane.</param>
        /// <param name="slotWidth">The width of the item slots, in pixels.</param>
        /// <param name="slotHeight">The height of the item slots, in pixels.</param>
        /// <param name="columns">The number of columns. The number of rows is automatically determined based on the inventory size.</param>
        /// <param name="spacing">The spacing between slots, in pixels.</param>
        /// <param name="player">The player that opened the menu.</param>
        /// <param name="inventory">The inventory this pane is for.</param>
        /// <param name="flags">The flags for the slots in this inventory pane.</param>
        public CoreInventoryPane(int x, int y, int slotWidth, int slotHeight, int columns, int spacing, ICorePlayer player, ITMInventory inventory, CoreInventorySlotWindowFlags flags)
            : this(x, y, slotWidth, slotHeight, columns, spacing, player, inventory, flags, 0, inventory.TotalSize)
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="CoreInventoryPane"/> for the specified inventory.
        /// </summary>
        /// <param name="x">The X position of the pane.</param>
        /// <param name="y">The Y position of the pane.</param>
        /// <param name="slotWidth">The width of the item slots, in pixels.</param>
        /// <param name="slotHeight">The height of the item slots, in pixels.</param>
        /// <param name="columns">The number of columns. The number of rows is automatically determined based on the inventory size.</param>
        /// <param name="spacing">The spacing between slots, in pixels.</param>
        /// <param name="player">The player that opened the menu.</param>
        /// <param name="inventory">The inventory this pane is for.</param>
        /// <param name="flags">The flags for the slots in this inventory pane.</param>
        /// <param name="start">The start slot index of the inventory.</param>
        /// <param name="end">The end slot index of the the inventory.</param>
        public CoreInventoryPane(int x, int y, int slotWidth, int slotHeight, int columns, int spacing, ICorePlayer player, ITMInventory inventory, CoreInventorySlotWindowFlags flags, int start, int end)
            : base(x, y, 0, 0)
        {
            _start = start;
            _end = end;
            ItemCount = end - start;
            Columns = columns;
            Rows = (int)MathF.Ceiling(ItemCount / (float)columns);
            Inventory = inventory;
            _spacing = spacing;
            _player = player;
            InitWindows(slotWidth, slotHeight);
            Colors = TransparentColorProfile;
        }
    }
}
