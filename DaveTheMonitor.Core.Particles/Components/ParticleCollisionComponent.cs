using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleCollision", "Collision", "Particle")]
    public sealed class ParticleCollisionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleCollision";
        public bool Destroy => _destroy.Value;
        private bool? _destroy;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _destroy = DeserializationHelper.GetBoolProperty(element, "Destroy");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ParticleCollisionComponent)replacement;
            if (component._destroy.HasValue) _destroy = component._destroy;
        }

        public override void SetDefaults()
        {
            _destroy ??= true;
        }
    }
}
