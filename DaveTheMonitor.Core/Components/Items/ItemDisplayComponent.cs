using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemDisplay", "Display", "Item")]
    public sealed class ItemDisplayComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemDisplay";
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PluralType Plural => _plural.Value;
        private PluralType? _plural;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            Name = DeserializationHelper.GetStringProperty(element, "Name");
            Description = DeserializationHelper.GetStringProperty(element, "Description");
            _plural = DeserializationHelper.GetEnumProperty<PluralType>(element, "Plural");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemDisplayComponent)replacement;
            if (component.Name != null) Name = component.Name;
            if (component.Description != null) Description = component.Description;
            if (component._plural.HasValue) _plural = component._plural;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (Name != null) data.Name = Name;
            if (Description != null) data.Desc = Description;
            if (_plural.HasValue) data.Plural = Plural;
        }

        public override void SetDefaults()
        {
            Name ??= "Unknown";
            Description ??= Name;
            _plural ??= PluralType.S;
        }

        public static ItemDisplayComponent FromXML(ItemDataXML data)
        {
            var component = new ItemDisplayComponent
            {
                Name = data.Name,
                Description = data.Desc,
                _plural = data.Plural
            };

            return component;
        }
    }
}
