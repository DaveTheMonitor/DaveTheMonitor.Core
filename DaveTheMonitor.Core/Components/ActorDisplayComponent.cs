using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    [Component("Core.ActorDisplay", "Display", "Actor")]
    public sealed class ActorDisplayComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorDisplay";
        public string Name { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            Name = DeserializationHelper.GetStringProperty(element, "Name");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorDisplayComponent)replacement;
            if (component.Name != null) Name = component.Name;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (Name != null) data.Name = Name;
        }

        public override void SetDefaults()
        {
            Name ??= "Unknown";
        }

        public static ActorDisplayComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorDisplayComponent
            {
                Name = data.Name
            };

            return component;
        }
    }
}
