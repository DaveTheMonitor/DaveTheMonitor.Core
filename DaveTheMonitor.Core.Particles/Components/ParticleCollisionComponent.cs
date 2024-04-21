using DaveTheMonitor.Core.Components;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleCollision", "Collision", "Particle")]
    public sealed class ParticleCollisionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleCollision";

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
        }

        public override void ReplaceWith(Component replacement)
        {
            
        }

        public override void SetDefaults()
        {
            
        }
    }
}
