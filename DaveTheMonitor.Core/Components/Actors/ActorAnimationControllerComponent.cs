using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorAnimationController", "AnimationController", "Actor")]
    public sealed class ActorAnimationControllerComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorAnimationController";
        public string ControllerName { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ControllerName = DeserializationHelper.GetStringProperty(element, "AnimationController");

            if (string.IsNullOrEmpty(ControllerName))
            {
                throw new InvalidCoreJsonException("ActorAnimationControllerComponent AnimationController must not be empty.");
            }
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorModelComponent)replacement;
            ControllerName = component.ModelName;
        }

        public override void SetDefaults()
        {
            ControllerName ??= null;
        }
    }
}
