using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemFuel", "Fuel", "Item")]
    public sealed class ItemFuelComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.Fuel";
        public ushort Duration => _duration.Value;
        private ushort? _duration;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _duration = DeserializationHelper.GetUInt16Property(element, "Duration");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemFuelComponent)replacement;
            if (component._duration.HasValue) _duration = component._duration;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_duration.HasValue) data.BurnTime = Duration;
        }

        public override void SetDefaults()
        {
            _duration ??= 0;
        }

        public static ItemFuelComponent FromXML(ItemDataXML data)
        {
            var component = new ItemFuelComponent
            {
                _duration = data.Durability
            };

            return component;
        }
    }
}
