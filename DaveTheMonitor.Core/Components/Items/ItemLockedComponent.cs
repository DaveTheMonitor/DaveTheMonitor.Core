using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemLocked", "Locked", "Item")]
    public sealed class ItemLockedComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemLocked";
        public bool LockedDD => _lockedDD.Value;
        public bool LockedSU => _lockedSU.Value;
        public bool LockedCR => _lockedCR.Value;
        private bool? _lockedDD;
        private bool? _lockedSU;
        private bool? _lockedCR;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _lockedDD = DeserializationHelper.GetBoolProperty(element, "DigDeep");
            _lockedSU = DeserializationHelper.GetBoolProperty(element, "Survival");
            _lockedCR = DeserializationHelper.GetBoolProperty(element, "Creative");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemLockedComponent)replacement;
            if (component._lockedDD.HasValue) _lockedDD = component._lockedDD;
            if (component._lockedSU.HasValue) _lockedSU = component._lockedSU;
            if (component._lockedCR.HasValue) _lockedCR = component._lockedCR;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_lockedDD.HasValue) data.LockedDD = LockedDD;
            if (_lockedSU.HasValue) data.LockedSU = LockedSU;
            if (_lockedCR.HasValue) data.LockedCR = LockedCR;
        }

        public override void SetDefaults()
        {
            _lockedDD ??= true;
            _lockedSU ??= false;
            _lockedCR ??= false;
        }

        public static ItemLockedComponent FromXML(ItemDataXML data)
        {
            var component = new ItemLockedComponent
            {
                _lockedDD = data.LockedDD,
                _lockedSU = data.LockedSU,
                _lockedCR = data.LockedCR
            };

            return component;
        }
    }
}
