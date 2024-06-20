using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorModel", "Model", "Actor")]
    public sealed class ActorModelComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorModel";
        public string ModelName { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ModelName = DeserializationHelper.GetStringProperty(element, "Model");

            if (string.IsNullOrEmpty(ModelName))
            {
                throw new InvalidCoreJsonException("ActorModelComponent Model must not be empty.");
            }
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorModelComponent)replacement;
            ModelName = component.ModelName;
        }

        public override void SetDefaults()
        {
            ModelName ??= null;
        }
    }
}
