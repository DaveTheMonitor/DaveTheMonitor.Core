using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorCombat", "Combat", "Actor")]
    public sealed class ActorCombatComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorCombat";
        public int Damage => _damage.Value;
        private int? _damage;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _damage = DeserializationHelper.GetInt32Property(element, "Damage");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorCombatComponent)replacement;
            if (component._damage.HasValue) _damage = component._damage;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (_damage.HasValue) data.HandMaxHit = Damage;
        }

        public override void SetDefaults()
        {
            _damage ??= 0;
        }

        public static ActorCombatComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorCombatComponent()
            {
                _damage = data.HandMaxHit,
            };

            return component;
        }
    }
}
