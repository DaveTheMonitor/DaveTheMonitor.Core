using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    [Component("Core.ActorDefinition", "Definition", "Actor")]
    public sealed class ActorDefinitionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorDefinition";
        public string ActorId { get; private set; }
        public bool IsValid => _isValid.Value;
        private bool? _isValid;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ActorId = DeserializationHelper.GetStringProperty(element, "ID");
            _isValid = DeserializationHelper.GetBoolProperty(element, "IsValid");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorDefinitionComponent)replacement;
            if (component.ActorId != null) ActorId = component.ActorId;
            if (component._isValid.HasValue) _isValid = component._isValid;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            data.IsValid = IsValid;
        }

        public override void SetDefaults()
        {
            ActorId ??= null;
            _isValid ??= true;
        }

        public static ActorDefinitionComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorDefinitionComponent
            {
                ActorId = data.IDString,
                _isValid = data.IsValid
            };

            return component;
        }
    }
}
