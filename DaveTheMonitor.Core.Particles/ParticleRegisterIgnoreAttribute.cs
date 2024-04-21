using System;

namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// Specifies that a class should not be automatically registered as a particle even if it inherits <see cref="ParticleDefinition"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ParticleRegisterIgnoreAttribute : Attribute
    {
        
    }
}
