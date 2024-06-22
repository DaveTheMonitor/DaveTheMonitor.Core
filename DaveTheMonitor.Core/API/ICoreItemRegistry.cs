using StudioForge.TotalMiner;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A <see cref="IDefinitionRegistry{T}"/> containing <see cref="CoreItem"/> definitions.
    /// </summary>
    public interface ICoreItemRegistry : IDefinitionRegistry<CoreItem>
    {
        /// <summary>
        /// Indexer for items, equivalent to <see cref="GetItem(Item)"/>.
        /// </summary>
        /// <param name="item">The item type.</param>
        /// <returns>The <see cref="CoreItem"/> definition for the specified <see cref="Item"/>.</returns>
        CoreItem this[Item item] { get; }

        /// <summary>
        /// Initializes all item definitions from <see cref="ItemDataXML"/>.
        /// </summary>
        /// <param name="data">The data to initialize from.</param>
        void InitializeAllItems(IEnumerable<ItemDataXML> data);

        /// <summary>
        /// Updates global item data to use the item definitions from this registry.
        /// </summary>
        void UpdateGlobalItemData();

        /// <summary>
        /// Gets the <see cref="CoreItem"/> definition for an <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The item type.</param>
        /// <returns>The definition for the specified <see cref="Item"/>.</returns>
        CoreItem GetItem(Item item);

        /// <summary>
        /// Gets the <see cref="CoreItem"/> definition for a <see cref="Block"/>.
        /// </summary>
        /// <param name="block">The block type.</param>
        /// <returns>The definition for the specified <see cref="Block"/>.</returns>
        CoreItem GetBlock(Block block);
    }
}
