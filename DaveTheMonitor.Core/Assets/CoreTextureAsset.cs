using Microsoft.Xna.Framework.Graphics;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing a texture.
    /// </summary>
    public sealed class CoreTextureAsset : CoreModAsset
    {
        /// <summary>
        /// The texture of this asset.
        /// </summary>
        public Texture2D Texture { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Texture?.Dispose();
                Texture = null;
            }
        }

        /// <summary>
        /// Creates a new asset containing the specified texture.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="texture">The texture of this asset.</param>
        public CoreTextureAsset(string fullPath, string name, Texture2D texture) : base(fullPath, name)
        {
            Texture = texture;
        }
    }
}
