using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemStackable", "Stackable", "Item")]
    public sealed class ItemStackableComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemStackable";
        public int StackSize => _stackSize.Value;
        private int? _stackSize;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _stackSize = DeserializationHelper.GetInt32Property(element, "Max");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemStackableComponent)replacement;
            if (component._stackSize != null) _stackSize = component._stackSize;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_stackSize.HasValue) data.StackSize = StackSize;
        }

        public override void SetDefaults()
        {
            _stackSize ??= 100;
        }

        public static ItemStackableComponent FromXML(ItemDataXML data)
        {
            var component = new ItemStackableComponent
            {
                _stackSize = data.StackSize
            };

            return component;
        }
    }
}
