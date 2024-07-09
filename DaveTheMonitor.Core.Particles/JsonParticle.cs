using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using DaveTheMonitor.Core.Particles.Components;
using Microsoft.Xna.Framework;
using StudioForge.Engine;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles
{
    [DebuggerDisplay("{Id}")]
    [ParticleRegisterIgnore]
    internal sealed class JsonParticle : ParticleDefinition, IJsonType<JsonParticle>
    {
        public override string Id => _id;
        public ComponentCollection Components { get; private set; }
        public ParticleDefinitionComponent Definition { get; private set; }
        public ParticleDisplayComponent Display { get; private set; }
        public ParticleEmitterComponent Emitter { get; private set; }
        public ParticleWindComponent Wind { get; private set; }
        public ParticleGravityComponent Gravity { get; private set; }
        public ParticleCollisionComponent Collision { get; private set; }
        public override bool Draw => _shouldDraw;
        public override ParticleMaterial Material => _material;
        private string _id;
        private ParticleMaterial _material;
        private float _duration;
        private bool _shouldDraw;
        private bool _isEmitter;
        private ParticleFaceType _faceType;
        private ParticleEmitterType _emitterType;
        private float _emitFrequency;
        private ParticleDefinition _emitterParticleDefinition;

        public static JsonParticle FromJson(string json)
        {
            JsonDocumentOptions docOptions = DeserializationHelper.DocumentOptionsTrailingCommasSkipComments;
            JsonSerializerOptions serializerOptions = DeserializationHelper.SerializerOptionsTrailingCommasSkipComments;
            ComponentCollection components = DeserializationHelper.ReadComponents(json, "Particle", docOptions, serializerOptions);

            if (!components.HasComponent<ParticleDefinitionComponent>())
            {
                throw new InvalidCoreJsonException("Particle must have a Definition component.");
            }

            return new JsonParticle(components);
        }

        public void ReplaceWith(ICoreMod mod, IJsonType<JsonParticle> other)
        {
            other.Components.CopyTo(Components, true);
            LoadAssets(mod, other.Components);
            UpdateFields();
        }

        public override void OnRegister(ICoreMod mod)
        {
            LoadAssets(mod, Components);
            UpdateFields();
        }

        private void LoadAssets(ICoreMod mod, ComponentCollection components)
        {
            var display = components.GetComponent<ParticleDisplayComponent>();
            if (display != null)
            {
                Texture = Game.ModManager.LoadTexture(mod, display.Texture);
            }
        }

        private void UpdateFields()
        {
            Definition = Components.GetComponent<ParticleDefinitionComponent>();
            Display = Components.GetComponent<ParticleDisplayComponent>();
            Emitter = Components.GetComponent<ParticleEmitterComponent>();
            Wind = Components.GetComponent<ParticleWindComponent>();
            Gravity = Components.GetComponent<ParticleGravityComponent>();
            Collision = Components.GetComponent<ParticleCollisionComponent>();
            _id = Definition.ParticleId;
            _material = GetMaterial(Display);
            _duration = Definition.Duration;
            _shouldDraw = Display != null;
            _isEmitter = Emitter != null;
            _faceType = Display?.FaceType ?? ParticleFaceType.Billboard;
            _emitterType = Emitter?.EmitterType ?? ParticleEmitterType.Steady;
            _emitFrequency = Emitter?.Frequency ?? 1;
        }

        private static ParticleMaterial GetMaterial(ParticleDisplayComponent display)
        {
            if (display == null)
            {
                return null;
            }

            return display.Material switch
            {
                "AlphaTest" => ParticleMaterial.AlphaTest,
                "AlphaTest_NoFog" => ParticleMaterial.AlphaTestNoFog,
                "Opaque" => ParticleMaterial.Opaque,
                "Opaque_NoFog" => ParticleMaterial.OpaqueNoFog,
                "AlphaBlend" => ParticleMaterial.AlphaBlend,
                "AlphaBlend_NoFog" => ParticleMaterial.AlphaBlendNoFog,
                _ => throw new InvalidOperationException($"Particle material {display.Material} does not exist")
            };
        }

        public override void Initialize(ParticleInstance particle)
        {
            if (_isEmitter)
            {
                _emitterParticleDefinition ??= Emitter.ParticleId != null ? ParticleRegister.GetDefinition(Emitter.ParticleId) : null;
            }
        }

        public override void Update(ParticleManager particleManager, ParticleInstance particle, ICoreWorld world)
        {
            if (_isEmitter)
            {
                if (_emitterType == ParticleEmitterType.Instant)
                {
                    int count = GetEmitCount(particle);
                    for (int i = 0; i < count; i++)
                    {
                        Emit(particleManager, particle, GetEmitOffset(particle), GetEmitVelocity(particle));
                    }
                    Destroy(particleManager, particle);
                    return;
                }

                float freq = _emitFrequency;
                float totalEmitted = particle.Data1;
                float prev = totalEmitted;
                totalEmitted += freq * Services.ElapsedTime;

                int numToEmit = (int)Math.Floor(totalEmitted) - (int)Math.Floor(prev);
                if (numToEmit > 0)
                {
                    for (int i = 0; i < numToEmit; i++)
                    {
                        int count = GetEmitCount(particle);
                        for (int j = 0; j < count; j++)
                        {
                            Emit(particleManager, particle, GetEmitOffset(particle), GetEmitVelocity(particle));
                        }
                    }
                }
                particle.Data1 = totalEmitted;
            }

            if (Gravity != null)
            {
                particle.Velocity += new Vector3(0, -1, 0) * Services.ElapsedTime * Gravity.Multiplier;
            }

            bool resting = false;
            if (Collision != null)
            {
                if (TouchingNonPassableBlock(particle))
                {
                    if (Collision.Destroy)
                    {
                        Destroy(particleManager, particle);
                        return;
                    }
                    else
                    {
                        particle.Velocity = Vector3.Zero;
                        resting = true;
                    }
                }
            }
            
            if (Wind != null)
            {
                if (resting)
                {
                    Vector3 add = world.WindVelocity * world.WindFactor * Wind.Multiplier * Services.ElapsedTime;
                    add.Y = 0;
                    particle.Position += add;
                }
                else
                {
                    particle.Position += world.WindVelocity * world.WindFactor * Wind.Multiplier * Services.ElapsedTime;
                }
            }
        }

        private void Emit(ParticleManager particleManager, ParticleInstance emitter, Vector3 offset, Vector3 velocity)
        {
            ParticleInstance particle = particleManager.SpawnParticle(_emitterParticleDefinition, emitter.Position + offset, velocity);
        }

        private int GetEmitCount(ParticleInstance emitter)
        {
            // Json count max is inclusive, PcgRandom is exclusive, so we add 1.
            return Emitter.Count ?? Game.TMGame.Random.Next(Emitter.CountMin.Value, Emitter.CountMax.Value + 1);
        }

        public override float GetDuration(ParticleInstance particle) => _duration;

        public override void GetDirection(ParticleInstance particle, ref Vector3 cameraPos, Vector3 cameraViewDirection, out Matrix matrix)
        {
            Vector3 pos = particle.Position;
            Vector3 up = Vector3.Up;
            switch (_faceType)
            {
                case ParticleFaceType.Billboard:
                {
                    Matrix.CreateBillboard(ref pos, ref cameraPos, ref up, cameraViewDirection, out matrix);
                    break;
                }
                case ParticleFaceType.ConstrainedBillboard:
                {
                    Matrix.CreateConstrainedBillboard(ref pos, ref cameraPos, ref up, cameraViewDirection, null, out matrix);
                    break;
                }
                default:
                {
                    Matrix.CreateBillboard(ref pos, ref cameraPos, ref up, cameraViewDirection, out matrix);
                    break;
                }
            }
        }

        public override Rectangle GetSrc(ParticleInstance particle)
        {
            return Display.Src ?? Display.SrcKeyframes.GetValue(GetAgeNormalized(particle));
        }

        public override Vector2 GetSize(ParticleInstance particle)
        {
            return Display.Size ?? Display.SizeKeyframes.Lerp(GetAgeNormalized(particle));
        }

        public override Color GetColor(ParticleInstance particle)
        {
            return Display.ParticleColor ?? Display.ColorKeyframes.Lerp(GetAgeNormalized(particle));
        }

        private Vector3 GetEmitOffset(ParticleInstance particle)
        {
            return Emitter.Offset ?? RandomVector3(Emitter.OffsetMin.Value, Emitter.OffsetMax.Value);
        }

        private Vector3 GetEmitVelocity(ParticleInstance particle)
        {
            return Emitter.Velocity ?? RandomVector3(Emitter.VelocityMin.Value, Emitter.VelocityMax.Value);
        }

        private Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            float x = min.X + ((float)Game.TMGame.Random.NextDouble() * (max.X - min.X));
            float y = min.Y + ((float)Game.TMGame.Random.NextDouble() * (max.Y - min.Y));
            float z = min.Z + ((float)Game.TMGame.Random.NextDouble() * (max.Z - min.Z));
            return new Vector3(x, y, z);
        }

        private float GetAgeNormalized(ParticleInstance particle)
        {
            return _duration == -1 ? 0 : particle.Age / _duration;
        }

        private JsonParticle(ComponentCollection components)
        {
            Components = components;
            _id = Components.GetComponent<ParticleDefinitionComponent>().ParticleId;
        }
    }
}
