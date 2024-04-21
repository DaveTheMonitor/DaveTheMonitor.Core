using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    [Component("Core.ItemDefinition", "Definition", "Item")]
    public sealed class ItemDefinitionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemDefinition";
        public string ItemId { get; private set; }
        public int NumId { get; set; }
        public bool IsValid => _isValid.Value;
        public bool IsEnabled => _isEnabled.Value;
        private bool? _isValid;
        private bool? _isEnabled;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ItemId = DeserializationHelper.GetStringProperty(element, "ID");
            _isValid = DeserializationHelper.GetBoolProperty(element, "IsValid");
            _isEnabled = DeserializationHelper.GetBoolProperty(element, "IsEnabled");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemDefinitionComponent)replacement;
            if (component.ItemId != null) ItemId = component.ItemId;
            if (component._isValid.HasValue) _isValid = component._isValid;
            if (component._isEnabled.HasValue) _isEnabled = component._isEnabled;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_isValid.HasValue) data.IsValid = IsValid;
            if (_isEnabled.HasValue) data.IsEnabled = IsEnabled;
        }

        public override void SetDefaults()
        {
            ItemId ??= null;
            _isValid ??= true;
            _isEnabled ??= true;
        }

        public static ItemDefinitionComponent FromXML(ItemDataXML data)
        {
            // *Icon items use the same IDString as the block, so
            // we convert the enum to a string for vanilla items.
            // Modded items will show as a number when converted,
            // so we use the IDString for those.
            var component = new ItemDefinitionComponent()
            {
                ItemId = data.ItemID <= Item.zLastItemID ? data.ItemID.ToString() : data.IDString,
                NumId = (int)data.ItemID,
                _isValid = data.IsValid,
                _isEnabled = data.IsEnabled,
            };

            return component;
        }
    }
}
