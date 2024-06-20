using Microsoft.Xna.Framework.Audio;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing a sound effect.
    /// </summary>
    public sealed class CoreSoundAsset : CoreModAsset
    {
        /// <summary>
        /// The sound effect of this asset.
        /// </summary>
        public SoundEffect Sound { get; private set; }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Sound.Dispose();
                Sound = null;
            }
        }

        /// <summary>
        /// Creates a new asset containing the specified sound.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="sound">The sound of this asset.</param>
        public CoreSoundAsset(string fullPath, string name, SoundEffect sound) : base(fullPath, name)
        {
            Sound = sound;
        }
    }
}
