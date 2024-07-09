using DaveTheMonitor.Core.Plugin;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Contains several extensions to make working with Core data easier.
    /// </summary>
    public static class APIExtensions
    {
        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> of this <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="InventoryItem"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> of this <see cref="InventoryItem"/>.</returns>
        public static CoreItem CoreItem(this InventoryItem item) => ItemRegistry[item.ItemID];

        /// <summary>
        /// Gets the raw <see cref="Core.CoreItem"/> of this <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="InventoryItem"/>.</param>
        /// <returns>The raw <see cref="Core.CoreItem"/> of this <see cref="InventoryItem"/>.</returns>
        public static CoreItem CoreItemRaw(this InventoryItem item) => ItemRegistry[item.ItemID_Raw];

        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> of this <see cref="InventoryItemXML"/>.
        /// </summary>
        /// <param name="item">The <see cref="InventoryItemXML"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> of this <see cref="InventoryItemXML"/>.</returns>
        public static CoreItem CoreItem(this InventoryItemXML item) => ItemRegistry[item.ItemID];

        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> of this <see cref="InventoryItemNDXML"/>.
        /// </summary>
        /// <param name="item">The <see cref="InventoryItemNDXML"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> of this <see cref="InventoryItemNDXML"/>.</returns>
        public static CoreItem CoreItem(this InventoryItemNDXML item) => ItemRegistry[item.ItemID];

        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> that represents this <see cref="ItemDataXML"/>.
        /// </summary>
        /// <param name="data">The <see cref="ItemDataXML"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> that represents this <see cref="ItemDataXML"/>.</returns>
        public static CoreItem CoreItem(this ItemDataXML data) => ItemRegistry[data.ItemID];

        /// <summary>
        /// Gets the <see cref="Core.CoreActor"/> that represents this <see cref="ActorTypeDataXML"/>.
        /// </summary>
        /// <param name="data">The <see cref="ActorTypeDataXML"/>.</param>
        /// <returns>The <see cref="Core.CoreActor"/> that represents this <see cref="ActorTypeDataXML"/>.</returns>
        public static CoreActor CoreActor(this ActorTypeDataXML data) => ActorRegistry[data.ActorType];

        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> for this <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> for this <see cref="Item"/>.</returns>
        public static CoreItem CoreItem(this Item item) => ItemRegistry[item];

        /// <summary>
        /// Gets the <see cref="Core.CoreItem"/> for this <see cref="Block"/>.
        /// </summary>
        /// <param name="block">The <see cref="Block"/>.</param>
        /// <returns>The <see cref="Core.CoreItem"/> for this <see cref="Block"/>.</returns>
        public static CoreItem CoreItem(this Block block) => ItemRegistry[block];

        private static ICoreItemRegistry ItemRegistry => CorePlugin.Instance.Game.ItemRegistry;
        private static ICoreActorRegistry ActorRegistry => CorePlugin.Instance.Game.ActorRegistry;
    }
}
