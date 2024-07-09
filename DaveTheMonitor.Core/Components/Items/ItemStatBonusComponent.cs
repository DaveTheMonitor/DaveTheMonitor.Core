using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Items
{
    [Component("Core.ItemStatBonus", "StatBonus", "Item")]
    public sealed class ItemStatBonusComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ItemStatBonus";
        public int CombatId { get; private set; }
        public int Health => _health.Value;
        public int Attack => _attack.Value;
        public int Strength => _strength.Value;
        public int Defense => _defense.Value;
        public int Ranged => _ranged.Value;
        public int Looting => _looting.Value;
        private int? _health;
        private int? _attack;
        private int? _strength;
        private int? _defense;
        private int? _ranged;
        private int? _looting;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            CombatId = -1;
            _health = DeserializationHelper.GetInt32Property(element, "Health");
            _attack = DeserializationHelper.GetInt32Property(element, "Attack");
            _strength = DeserializationHelper.GetInt32Property(element, "Strength");
            _defense = DeserializationHelper.GetInt32Property(element, "Defense");
            _ranged = DeserializationHelper.GetInt32Property(element, "Ranged");
            _looting = DeserializationHelper.GetInt32Property(element, "Looting");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ItemStatBonusComponent)replacement;
            CombatId = component.CombatId;
            if (component._health.HasValue) _health = component._health;
            if (component._attack.HasValue) _attack = component._attack;
            if (component._strength.HasValue) _strength = component._strength;
            if (component._defense.HasValue) _defense = component._defense;
            if (component._ranged.HasValue) _ranged = component._ranged;
            if (component._looting.HasValue) _looting = component._looting;
        }

        public void ReplaceXmlData(ref ItemCombatDataXML data)
        {
            if (CombatId <= 0)
            {
                return;
            }

            if (_health.HasValue) data.Health = (short)Health;
            if (_attack.HasValue) data.Attack = (short)Attack;
            if (_strength.HasValue) data.Strength = (short)Strength;
            if (_defense.HasValue) data.Defence = (short)Defense;
            if (_ranged.HasValue) data.Ranged = (short)Ranged;
            if (_looting.HasValue) data.Looting = (short)Looting;
        }

        public override void SetDefaults()
        {
            _health ??= 0;
            _attack ??= 0;
            _strength ??= 0;
            _defense ??= 0;
            _ranged ??= 0;
            _looting ??= 0;
        }

        public void Initialize()
        {
            CombatId = Globals1.ItemCombatData.Length;
            Array.Resize(ref Globals1.ItemCombatData, Globals1.ItemCombatData.Length + 1);
            Globals1.ItemCombatData[CombatId] = new ItemCombatDataXML()
            {
                CombatID = (CombatItem)CombatId,
                Health = (short)Health,
                Attack = (short)Attack,
                Strength = (short)Strength,
                Defence = (short)Defense,
                Ranged = (short)Ranged,
                Looting = (short)Looting
            };
        }

        public static ItemStatBonusComponent FromXML(ItemCombatDataXML data)
        {
            var component = new ItemStatBonusComponent
            {
                CombatId = (int)data.CombatID,
                _health = data.Health,
                _attack = data.Attack,
                _strength = data.Strength,
                _defense = data.Defence,
                _ranged = data.Ranged,
                _looting = data.Looting
            };

            return component;
        }
    }
}
