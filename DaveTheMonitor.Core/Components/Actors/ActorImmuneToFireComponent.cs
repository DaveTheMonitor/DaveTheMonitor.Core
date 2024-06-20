using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorImmuneToFire", "ImmuneToFire", "Actor")]
    public sealed class ActorImmuneToFireComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorImmuneToFire";
        public bool IsImmune => _isImmune.Value;
        private bool? _isImmune;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _isImmune = DeserializationHelper.GetBoolProperty(element, "IsImmune");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorImmuneToFireComponent)replacement;
            if (component._isImmune.HasValue) _isImmune = component._isImmune;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (_isImmune.HasValue) data.IsImmuneToFire = IsImmune;
        }

        public override void SetDefaults()
        {
            _isImmune ??= true;
        }

        public static ActorImmuneToFireComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorImmuneToFireComponent()
            {
                _isImmune = data.IsImmuneToFire,
            };

            return component;
        }
    }
}
