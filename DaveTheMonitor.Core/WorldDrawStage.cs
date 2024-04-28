using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Represents a map draw stage.
    /// </summary>
    [Flags]
    public enum WorldDrawStage
    {
        /// <summary>
        /// The entire draw. Actions at this stage are called before and after everything else has been drawn.
        /// </summary>
        Draw = 1,

        /// <summary>
        /// The sky curtain and stars.
        /// </summary>
        Background = 2,

        /// <summary>
        /// The sun and moon.
        /// </summary>
        SunAndMoon = 4,

        /// <summary>
        /// The items in the player's hand.
        /// </summary>
        PlayerItems = 8,

        /// <summary>
        /// Chunks in the world.
        /// </summary>
        Chunks = 16,

        /// <summary>
        /// Sign text.
        /// </summary>
        Signs = 32,

        /// <summary>
        /// Arcade games.
        /// </summary>
        Arcade = 64,

        /// <summary>
        /// Splinters (block breaking animations).
        /// </summary>
        Splinters = 128,

        /// <summary>
        /// Particles.
        /// </summary>
        Particles = 256,

        /// <summary>
        /// Avatars (players and NPCs).
        /// </summary>
        Avatars = 512,

        /// <summary>
        /// Entities.
        /// </summary>
        Entities = 1024,
        
        /// <summary>
        /// The player's current clipboard.
        /// </summary>
        Clipboard = 2048,

        /// <summary>
        /// The swing target outline.
        /// </summary>
        SwingTarget = 4096,

        /// <summary>
        /// Nameplates.
        /// </summary>
        NamePlates = 8192,

        /// <summary>
        /// Water and other transparent blocks.
        /// </summary>
        TransparentBlocks = 16384,

        /// <summary>
        /// Zone outlines (if enabled).
        /// </summary>
        Zones = 32768,

        /// <summary>
        /// Area indicators (if enabled).
        /// </summary>
        AreaIndicators = 65536,

        /// <summary>
        /// Script tools (script ray, sphere, etc.).
        /// </summary>
        ScriptTools = 131072,

        /// <summary>
        /// Actor bounds and frustums (if enabled).
        /// </summary>
        Bounds = 262144,

        /// <summary>
        /// Draw clouds.
        /// </summary>
        Clouds = 464288
    }
}
