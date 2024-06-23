namespace DaveTheMonitor.Core.Gui
{
    /// <summary>
    /// Flags for <see cref="CoreInventorySlotWindow"/> display.
    /// </summary>
    public enum CoreInventorySlotWindowFlags
    {
        // Must exactly match StudioForge.TotalMiner.Screens2.InventorySlotWinFlags

        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// Shows the quantity of the item.
        /// </summary>
        ShowQuantity = 1,

        /// <summary>
        /// Shows the buy price of the item in the tooltip.
        /// </summary>
        ShowBuyPrice = 2,

        /// <summary>
        /// Shows the sell price of the item in the tooltip.
        /// </summary>
        ShowSellPrice = 4,

        /// <summary>
        /// Hides the lock icon on this slot if the item is locked.
        /// </summary>
        HideLock = 16,

        /// <summary>
        /// Hides the durability bar even if it would normally be visible.
        /// </summary>
        HideDurability = 32
    }
}
