using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorBreatheUnderwater", "BreatheUnderwater", "Actor")]
    public sealed class ActorBreatheUnderwaterComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorBreatheUnderwater";
        public bool CanBreatheUnderwater => _canBreatheUnderwater.Value;
        private bool? _canBreatheUnderwater;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _canBreatheUnderwater = DeserializationHelper.GetBoolProperty(element, "CanBreatheUnderwater");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorBreatheUnderwaterComponent)replacement;
            if (component._canBreatheUnderwater.HasValue) _canBreatheUnderwater = component._canBreatheUnderwater;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (_canBreatheUnderwater.HasValue) data.IsImmuneToFire = CanBreatheUnderwater;
        }

        public override void SetDefaults()
        {
            _canBreatheUnderwater ??= true;
        }

        public static ActorBreatheUnderwaterComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorBreatheUnderwaterComponent()
            {
                _canBreatheUnderwater = data.CanBreatheUnderWater,
            };

            return component;
        }
    }
}
