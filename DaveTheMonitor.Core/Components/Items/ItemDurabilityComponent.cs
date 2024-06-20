using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemDurability", "Durability", "Item")]
    public sealed class ItemDurabilityComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemDurability";
        public ushort Max => _max.Value;
        private ushort? _max;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _max = DeserializationHelper.GetUInt16Property(element, "Max");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemDurabilityComponent)replacement;
            if (component._max.HasValue) _max = component._max;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_max.HasValue) data.Durability = Max;
        }

        public override void SetDefaults()
        {
            _max ??= 0;
        }

        public static ItemDurabilityComponent FromXML(ItemDataXML data)
        {
            var component = new ItemDurabilityComponent
            {
                _max = data.Durability
            };

            return component;
        }
    }
}
