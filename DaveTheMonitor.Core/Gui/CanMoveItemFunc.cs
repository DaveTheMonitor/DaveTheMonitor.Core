using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Gui
{
    /// <summary>
    /// Used by a <see cref="CoreInventorySlotWindow"/> to allow only specific items to be placed in the slot.
    /// </summary>
    /// <param name="win">The slot window.</param>
    /// <param name="item">The item being placed into the slot.</param>
    /// <param name="current">The item currently in the slot.</param>
    /// <returns>True if the item can be placed in the slot, otherwise false.</returns>
    public delegate bool CanMoveItemFunc(CoreInventorySlotWindow win, InventoryItem item, InventoryItem current);
}
