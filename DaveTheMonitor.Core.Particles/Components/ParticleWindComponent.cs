using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleWind", "Wind", "Particle")]
    public sealed class ParticleWindComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleWind";
        public float Multiplier => _multiplier.Value;
        private float? _multiplier;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _multiplier = DeserializationHelper.GetSingleProperty(element, "Multiplier");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ParticleWindComponent)replacement;
            if (component._multiplier.HasValue) _multiplier = component._multiplier;
        }

        public override void SetDefaults()
        {
            _multiplier ??= 1;
        }
    }
}
