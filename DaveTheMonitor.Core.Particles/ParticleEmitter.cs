using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.Engine;
using System;

namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// Defines a particle that emits other particles. Particle emitters use <see cref="ParticleInstance.Data1"/> to keep track of how many particles to emit.
    /// </summary>
    public abstract class ParticleEmitter : ParticleDefinition
    {
        /// <summary>
        /// The ID of the particle to emit.
        /// </summary>
        protected abstract string EmitParticleId { get; }
        /// <summary>
        /// The type of this emitter.
        /// </summary>
        protected abstract ParticleEmitterType EmitterType { get; }
        /// <summary>
        /// The definition of the particle to emit.
        /// </summary>
        protected ParticleDefinition ParticleDefinition { get; private set; }
        public override bool Draw => false;

        public override void Initialize(ParticleInstance particle)
        {
            ParticleDefinition ??= ParticleRegister.GetDefinition(EmitParticleId);
        }

        public override void Update(ParticleManager particleManager, ParticleInstance particle, ICoreWorld world)
        {
            if (EmitterType == ParticleEmitterType.Instant)
            {
                for (int i = 0; i < GetEmitCount(particle); i++)
                {
                    Emit(particleManager, particle, GetEmitOffset(particle), GetEmitVelocity(particle));
                }
                Destroy(particleManager, particle);
                return;
            }

            float freq = GetFrequency(particle);
            float totalEmitted = particle.Data1;
            float prev = totalEmitted;
            totalEmitted += freq * Services.ElapsedTime;

            int numToEmit = (int)Math.Floor(totalEmitted) - (int)Math.Floor(prev);
            if (numToEmit > 0)
            {
                for (int i = 0; i < numToEmit; i++)
                {
                    for (int j = 0; j < GetEmitCount(particle); j++)
                    {
                        Emit(particleManager, particle, GetEmitOffset(particle), GetEmitVelocity(particle));
                    }
                }
            }
            particle.Data1 = totalEmitted;
        }

        /// <summary>
        /// Emits one particle.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <param name="offset">The offset of the particle.</param>
        protected virtual void Emit(ParticleManager particleManager, ParticleInstance emitter, Vector3 offset, Vector3 velocity)
        {
            ParticleInstance particle = particleManager.SpawnParticle(ParticleDefinition, emitter.Position + offset, velocity);
            PostEmit(emitter, particle);
        }

        /// <summary>
        /// Called after a particle is emitted. Use this if the emitter needs to set data for the particle.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <param name="particle">The particle emitter.</param>
        protected virtual void PostEmit(ParticleInstance emitter, ParticleInstance particle)
        {

        }

        /// <summary>
        /// Gets the number of particles this emitter should emit every second.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <returns>The number of particles to emit per second.</returns>
        protected abstract float GetFrequency(ParticleInstance emitter);

        /// <summary>
        /// Gets the number of particles to emit whenever a particle is emitted.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <returns>The number of particles to emit.</returns>
        protected abstract int GetEmitCount(ParticleInstance emitter);

        /// <summary>
        /// Gets the velocity of the particle to emit.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <returns>The velocity of the particle.</returns>
        protected virtual Vector3 GetEmitVelocity(ParticleInstance emitter) => Vector3.Zero;

        /// <summary>
        /// Gets the offset of the particle to emit.
        /// </summary>
        /// <param name="emitter">The emitter instance.</param>
        /// <returns>The offset of the particle.</returns>
        protected virtual Vector3 GetEmitOffset(ParticleInstance emitter) => Vector3.Zero;

        public override Vector2 GetSize(ParticleInstance emitter) => new Vector2(0, 0);
        public override Rectangle GetSrc(ParticleInstance emitter) => new Rectangle(0, 0, 0, 0);

        public ParticleEmitter()
        {
            Texture = null;
        }
    }
}
