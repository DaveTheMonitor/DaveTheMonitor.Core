using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    [Component("Core.ActorPassive", "Passive", "Actor")]
    public sealed class ActorPassiveComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorPassive";
        public bool IsPassive => _isPassive.Value;
        private bool? _isPassive;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _isPassive = DeserializationHelper.GetBoolProperty(element, "IsPassive");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorPassiveComponent)replacement;
            if (component._isPassive.HasValue) _isPassive = component._isPassive;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (_isPassive.HasValue) data.IsPassive = IsPassive;
        }

        public override void SetDefaults()
        {
            _isPassive ??= true;
        }

        public static ActorPassiveComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorPassiveComponent()
            {
                _isPassive = data.IsPassive,
            };

            return component;
        }
    }
}
