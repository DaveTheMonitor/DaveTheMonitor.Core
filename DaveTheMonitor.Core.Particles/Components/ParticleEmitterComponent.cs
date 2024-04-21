using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleEmitter", "Emitter", "Particle")]
    public sealed class ParticleEmitterComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleEmitter";
        public string ParticleId { get; private set; }
        public float Frequency => _frequency.Value;
        public int? Count { get; private set; }
        public int? CountMin { get; private set; }
        public int? CountMax { get; private set; }
        public Vector3? Offset { get; private set; }
        public Vector3? OffsetMin { get; private set; }
        public Vector3? OffsetMax { get; private set; }
        public Vector3? Velocity { get; private set; }
        public Vector3? VelocityMin { get; private set; }
        public Vector3? VelocityMax { get; private set; }
        public ParticleEmitterType EmitterType => _emitterType.Value;
        private float? _frequency;
        private ParticleEmitterType? _emitterType;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            ParticleId = DeserializationHelper.GetStringProperty(element, "ID");
            _frequency = DeserializationHelper.GetSingleProperty(element, "Frequency");
            _emitterType = DeserializationHelper.GetEnumProperty<ParticleEmitterType>(element, "EmitterType");

            if (element.TryGetProperty("Count", out JsonElement count))
            {
                if (count.ValueKind != JsonValueKind.Number && count.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Count must be a number or object");
                }

                if (count.ValueKind == JsonValueKind.Object)
                {
                    Count = null;
                    CountMin = DeserializationHelper.GetInt32Property(count, "Min") ?? 1;
                    CountMax = DeserializationHelper.GetInt32Property(count, "Max") ?? CountMin;
                }
                else
                {
                    if (count.TryGetInt32(out int c))
                    {
                        Count = c;
                    }
                    else
                    {
                        Count = 1;
                    }
                    CountMin = null;
                    CountMax = null;
                }
            }

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
            var component = (ParticleEmitterComponent)replacement;
            if (component.ParticleId != null) ParticleId = component.ParticleId;
            if (component._frequency.HasValue) _frequency = component._frequency;
            if (component._emitterType.HasValue) _emitterType = component._emitterType;

            if (component.Count.HasValue)
            {
                Count = component.Count.Value;
                CountMin = null;
                CountMax = null;
            }
            else if (component.CountMin.HasValue)
            {
                Count = null;
                CountMin = component.CountMin.Value;
                CountMax = component.CountMax.Value;
            }

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
            ParticleId ??= null;
            _frequency ??= 1;
            _emitterType ??= ParticleEmitterType.Steady;
            if (!Count.HasValue && !CountMin.HasValue)
            {
                Count = 1;
            }
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
