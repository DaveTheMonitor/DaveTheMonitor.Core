using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Biomes.Components
{
    [Component(Id, "Definition", "Decoration")]
    public sealed class DecorationDefinitionComponent : Component, IComponentDeserializable
    {
        private const string Id = "Core.DecorationDefinition";
        public override string ComponentId => Id;
        public string DecorationId { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            DecorationId = DeserializationHelper.GetStringProperty(element, "ID");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (DecorationDefinitionComponent)replacement;
            if (component.DecorationId != null) DecorationId = component.DecorationId;
        }

        public override void SetDefaults()
        {
            DecorationId ??= null;
        }
    }
}
