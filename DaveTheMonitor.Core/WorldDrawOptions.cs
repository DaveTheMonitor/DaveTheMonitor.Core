using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Flags for world drawing.
    /// </summary>
    [Flags]
    public enum WorldDrawOptions
    {
        /// <summary>
        /// No options, nothing will draw.
        /// </summary>
        None = 0,

        /// <summary>
        /// Draw the sky curtain, stars, and sun and moon.
        /// </summary>
        Background = 1,

        /// <summary>
        /// Draw the items in the player's hand.
        /// </summary>
        PlayerItems = 2,

        /// <summary>
        /// Draw chunks in the world.
        /// </summary>
        Chunks = 4,

        /// <summary>
        /// Draw sign text.
        /// </summary>
        Signs = 8,

        /// <summary>
        /// Draw arcade games.
        /// </summary>
        Arcade = 16,

        /// <summary>
        /// Draw splinters (block breaking animations)
        /// </summary>
        Splinters = 32,

        /// <summary>
        /// Draw particles.
        /// </summary>
        Particles = 64,

        /// <summary>
        /// Draw avatars (players and NPCs).
        /// </summary>
        Avatars = 128,

        /// <summary>
        /// Draw entities.
        /// </summary>
        Entities = 256,
        
        /// <summary>
        /// Draw the player's current clipboard.
        /// </summary>
        Clipboard = 512,

        /// <summary>
        /// Draw the swing target outline.
        /// </summary>
        SwingTarget = 1024,

        /// <summary>
        /// Draw nameplates.
        /// </summary>
        NamePlates = 2048,

        /// <summary>
        /// Draw water and other transparent blocks.
        /// </summary>
        TransparentBlocks = 4096,

        /// <summary>
        /// Draw zone outlines (if enabled)
        /// </summary>
        Zones = 8192,

        /// <summary>
        /// Draw area indicators (if enabled).
        /// </summary>
        AreaIndicators = 16384,

        /// <summary>
        /// Draw script tools.
        /// </summary>
        ScriptTools = 32768,

        /// <summary>
        /// Draw actor bounds and frustums (if enabled).
        /// </summary>
        Bounds = 65536,

        /// <summary>
        /// Draw clouds.
        /// </summary>
        Clouds = 131072,

        /// <summary>
        /// Draw all elements.
        /// </summary>
        All = Background | PlayerItems | Chunks | Signs | Arcade | Splinters | Particles | Avatars | Entities | Clipboard | SwingTarget | NamePlates | TransparentBlocks | Zones | AreaIndicators | ScriptTools | Bounds | Clouds,

        /// <summary>
        /// Draw all elements except the background.
        /// </summary>
        AllNoBackground = PlayerItems | Chunks | Signs | Arcade | Splinters | Particles | Avatars | Entities | Clipboard | SwingTarget | NamePlates | TransparentBlocks | Zones | AreaIndicators | ScriptTools | Bounds | Clouds
    }
}
