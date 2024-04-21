using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    [Component("Core.ItemWeapon", "Weapon", "Item")]
    public sealed class ItemWeaponComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemWeapon";
        public float Damage => _damage.Value;
        public float Range => _range.Value;
        private float? _damage;
        private float? _range;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _damage = DeserializationHelper.GetSingleProperty(element, "Damage");
            _range = DeserializationHelper.GetSingleProperty(element, "Range");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemWeaponComponent)replacement;
            if (component._damage.HasValue) _damage = component._damage;
            if (component._range.HasValue) _range = component._range;
        }

        public void ReplaceXmlData(ItemDataXML data)
        {
            if (_damage.HasValue) data.StrikeDamage = Damage;
            if (_range.HasValue) data.StrikeReach = Range;
        }

        public override void SetDefaults()
        {
            _damage ??= 0;
            _range ??= 0;
        }

        public static ItemWeaponComponent FromXML(ItemDataXML data)
        {
            var component = new ItemWeaponComponent
            {
                _damage = data.StrikeDamage,
                _range = data.StrikeReach
            };

            return component;
        }
    }
}
