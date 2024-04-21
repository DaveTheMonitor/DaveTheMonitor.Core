using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Effects.Components
{
    [Component("Core.EffectParticle", "Particle", "Effect")]
    public sealed class EffectParticleComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.EffectParticle";
        public string LocalParticleID { get; private set; }
        public string ParticleID { get; private set; }
        public Vector3? Offset { get; private set; }
        public Vector3? OffsetMin { get; private set; }
        public Vector3? OffsetMax { get; private set; }
        public Vector3? Velocity { get; private set; }
        public Vector3? VelocityMin { get; private set; }
        public Vector3? VelocityMax { get; private set; }
        public EffectParticleOffsetType OffsetType => _offsetType.Value;
        public EffectParticleVelocityType VelocityType => _velocityType.Value;
        public float? LocalFrequency { get; private set; }
        public float Frequency => _frequency.Value;
        public float Count => _count.Value;
        private float? _frequency;
        private float? _count;
        private EffectParticleOffsetType? _offsetType;
        private EffectParticleVelocityType? _velocityType;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            LocalParticleID = DeserializationHelper.GetStringProperty(element, "LocalID");
            ParticleID = DeserializationHelper.GetStringProperty(element, "ID");
            LocalFrequency = DeserializationHelper.GetSingleProperty(element, "LocalFrequency");
            _frequency = DeserializationHelper.GetSingleProperty(element, "Frequency");
            _count = DeserializationHelper.GetSingleProperty(element, "Count");
            _offsetType = DeserializationHelper.GetEnumProperty<EffectParticleOffsetType>(element, "OffsetType");
            _velocityType = DeserializationHelper.GetEnumProperty<EffectParticleVelocityType>(element, "VelocityType");

            if (element.TryGetProperty("Offset", out JsonElement offset))
            {
                if ((offset.ValueKind == JsonValueKind.Array && offset.GetArrayLength() != 3) && offset.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Offset must be an array or object");
                }

                if (offset.ValueKind == JsonValueKind.Object)
                {
                    Offset = null;
                    OffsetMin = DeserializationHelper.GetVector3Property(offset, "Min");
                    OffsetMax = DeserializationHelper.GetVector3Property(offset, "Max");
                }
                else
                {
                    Offset = DeserializationHelper.GetVector3(offset);
                    OffsetMin = null;
                    OffsetMax = null;
                }
            }

            if (element.TryGetProperty("Velocity", out JsonElement velocity))
            {
                if ((velocity.ValueKind == JsonValueKind.Array && velocity.GetArrayLength() != 3) && velocity.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Velocity must be an array or object");
                }

                if (velocity.ValueKind == JsonValueKind.Object)
                {
                    Velocity = null;
                    VelocityMin = DeserializationHelper.GetVector3Property(velocity, "Min");
                    VelocityMax = DeserializationHelper.GetVector3Property(velocity, "Max");
                }
                else
                {
                    Velocity = DeserializationHelper.GetVector3(velocity);
                    VelocityMin = null;
                    VelocityMax = null;
                }
            }
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (EffectParticleComponent)replacement;
            if (component.LocalParticleID != null) LocalParticleID = component.LocalParticleID;
            if (component.ParticleID != null) ParticleID = component.ParticleID;
            if (component.LocalFrequency.HasValue) LocalFrequency = component.LocalFrequency.Value;
            if (component._frequency.HasValue) _frequency = component._frequency.Value;
            if (component._count.HasValue) _count = component._count.Value;
            if (component._offsetType.HasValue) _offsetType = component._offsetType.Value;
            if (component._velocityType.HasValue) _velocityType = component._velocityType.Value;

            if (component.Offset.HasValue)
            {
                Offset = component.Offset.Value;
                OffsetMin = null;
                OffsetMax = null;
            }
            else if (component.OffsetMin.HasValue)
            {
                Offset = null;
                OffsetMin = component.OffsetMin.Value;
                OffsetMax = component.OffsetMax.Value;
            }

            if (component.Velocity.HasValue)
            {
                Velocity = component.Velocity.Value;
                VelocityMin = null;
                VelocityMax = null;
            }
            else if (component.VelocityMin.HasValue)
            {
                Velocity = null;
                VelocityMin = component.VelocityMin.Value;
                VelocityMax = component.VelocityMax.Value;
            }
        }

        public override void SetDefaults()
        {
            LocalParticleID ??= null;
            ParticleID ??= null;
            LocalFrequency ??= null;
            _frequency ??= 20;
            _count ??= 1;
            _offsetType ??= EffectParticleOffsetType.Box;
            _velocityType ??= EffectParticleVelocityType.Absolute;
            if (!Offset.HasValue && !OffsetMin.HasValue)
            {
                Offset = new Vector3(0, 0, 0);
            }
            if (!Velocity.HasValue && !VelocityMin.HasValue)
            {
                Velocity = new Vector3(0, 0, 0);
            }
        }
    }
}
