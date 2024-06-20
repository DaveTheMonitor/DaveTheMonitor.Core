using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner;
using System.Collections.Generic;

namespace DaveTheMonitor.Core
{
    internal sealed class ItemRegistry : DefinitionRegistry<CoreItem>, ICoreItemRegistry
    {
        public void InitializeAllItems(IEnumerable<ItemDataXML> data)
        {
            foreach (ItemDataXML xml in data)
            {
                // *Icon items use the same IDString as the block, so
                // we convert the enum to a string for vanilla items.
                // Modded items will show as a number when converted,
                // so we use the IDString for those.
                string id = xml.ItemID <= Item.zLastItemID ? xml.ItemID.ToString() : xml.IDString;
                if (HasDefinition(id))
                {
#if DEBUG
                    Debugger.Break();
                    CorePlugin.Warn($"Duplicate item Id: {id}");
#endif
                    continue;
                }

                CoreItem item = CoreItem.FromItemDataXML(xml);
                ICoreMod mod = Game.ModManager.GetDefiningMod(item.ItemType);
                RegisterDefinition(item, mod);
            }
        }

        public void UpdateGlobalItemData()
        {
            foreach (CoreItem item in this)
            {
                SetItemData(Globals1.ItemData[item.NumId], item);
            }
        }

        private void SetItemData(ItemDataXML data, CoreItem item)
        {
            item.Display?.ReplaceXmlData(data);
            item.Durability?.ReplaceXmlData(data);
            item.Locked?.ReplaceXmlData(data);
            item.Stackable?.ReplaceXmlData(data);
            item.Tradeable?.ReplaceXmlData(data);
            item.Weapon?.ReplaceXmlData(data);
            item.TypeComponent?.ReplaceXmlData(ref Globals1.ItemTypeData[(int)data.ItemID]);
        }

        protected override void OnRegister(CoreItem definition)
        {
            definition._game = Game;
        }

        public CoreItem GetItem(Item item)
        {
            return GetDefinition((int)item);
        }

        public CoreItem GetBlock(Block block)
        {
            return GetDefinition((int)block);
        }

        public ItemRegistry(ICoreGame game) : base(game, null)
        {

        }
    }
}
