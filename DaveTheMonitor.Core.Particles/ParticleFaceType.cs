namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// The way a particle faces.
    /// </summary>
    public enum ParticleFaceType
    {
        /// <summary>
        /// Faces the camera.
        /// </summary>
        Billboard,
        /// <summary>
        /// Faces the camera around Y.
        /// </summary>
        ConstrainedBillboard
    }
}
