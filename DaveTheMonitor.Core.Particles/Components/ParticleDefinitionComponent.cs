using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleDefinition", "Definition", "Particle")]
    public sealed class ParticleDefinitionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleDefinition";
        public string ParticleId { get; private set; }
        public float Duration => _duration.Value;
        private float? _duration;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ParticleId = DeserializationHelper.GetStringProperty(element, "ID");
            _duration = DeserializationHelper.GetSingleProperty(element, "Duration");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ParticleDefinitionComponent)replacement;
            if (component.ParticleId != null) ParticleId = component.ParticleId;
            if (component._duration.HasValue) _duration = component._duration;
        }

        public override void SetDefaults()
        {
            ParticleId ??= null;
            _duration ??= -1;
        }
    }
}
