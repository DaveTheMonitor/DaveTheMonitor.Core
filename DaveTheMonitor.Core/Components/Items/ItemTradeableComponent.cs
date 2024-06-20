using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemTradeable", "Tradeable", "Item")]
    public sealed class ItemTradeableComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemTradeable";
        public int SellPrice => _sellPrice.Value;
        private int? _sellPrice;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _sellPrice = DeserializationHelper.GetInt32Property(element, "SellPrice");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemTradeableComponent)replacement;
            if (component._sellPrice.HasValue) _sellPrice = component._sellPrice;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_sellPrice.HasValue) data.MinCSPrice = SellPrice;
        }

        public override void SetDefaults()
        {
            _sellPrice ??= 0;
        }

        public static ItemTradeableComponent FromXML(ItemDataXML data)
        {
            var component = new ItemTradeableComponent
            {
                _sellPrice = data.MinCSPrice
            };

            return component;
        }
    }
}
