namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// The type of a particle emitter.
    /// </summary>
    public enum ParticleEmitterType
    {
        /// <summary>
        /// All particles from <see cref="ParticleEmitter.GetEmitCount(ParticleInstance)"/> are emitted immediately when the emitter is spawned.
        /// </summary>
        Instant,
        /// <summary>
        /// Particles are emitted at a constant rate based on <see cref="ParticleEmitter.GetFrequency(ParticleInstance)"/>.
        /// </summary>
        Steady
    }
}
