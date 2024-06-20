using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System.IO;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Global data for the Core Mod. Not to be confused with <see cref="CoreGlobals"/>, which contains global data and variables for the game/engine.
    /// </summary>
    public static class CoreGlobalData
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
        /// The current save version of the Core Mod. This changes whenever the Core Mod gets an update that changes how state is saved.
        /// </summary>
        public static int CoreSaveVersion => 1;

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
