using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System.IO;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Global data for the game.
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// The 16x16 texture used when a texture can't be found.
        /// </summary>
        public static Texture2D MissingTexture16 { get; private set; }

        /// <summary>
        /// The 32x32 texture used when a texture can't be found.
        /// </summary>
        public static Texture2D MissingTexture32 { get; private set; }

        /// <summary>
        /// The 64x64 texture used when a texture can't be found.
        /// </summary>
        public static Texture2D MissingTexture64 { get; private set; }

        /// <summary>
        /// Initializes the global data from the specified path.
        /// </summary>
        /// <param name="path"></param>
        public static void Initialize(string path)
        {
            MissingTexture16 = Texture2D.FromFile(CoreGlobals.GraphicsDevice, Path.Combine(path, "Textures", "MissingTexture16.png"));
            MissingTexture32 = Texture2D.FromFile(CoreGlobals.GraphicsDevice, Path.Combine(path, "Textures", "MissingTexture32.png"));
            MissingTexture64 = Texture2D.FromFile(CoreGlobals.GraphicsDevice, Path.Combine(path, "Textures", "MissingTexture64.png"));
        }
    }
}
