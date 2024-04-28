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
        /// Draw the sky curtain and stars.
        /// </summary>
        Background = 1,

        /// <summary>
        /// Draw the sun and moon.
        /// </summary>
        SunAndMoon = 2,

        /// <summary>
        /// Draw the items in the player's hand.
        /// </summary>
        PlayerItems = 4,

        /// <summary>
        /// Draw chunks in the world.
        /// </summary>
        Chunks = 8,

        /// <summary>
        /// Draw sign text.
        /// </summary>
        Signs = 16,

        /// <summary>
        /// Draw arcade games.
        /// </summary>
        Arcade = 32,

        /// <summary>
        /// Draw splinters (block breaking animations).
        /// </summary>
        Splinters = 64,

        /// <summary>
        /// Draw particles.
        /// </summary>
        Particles = 128,

        /// <summary>
        /// Draw avatars (players and NPCs).
        /// </summary>
        Avatars = 256,

        /// <summary>
        /// Draw entities.
        /// </summary>
        Entities = 512,
        
        /// <summary>
        /// Draw the player's current clipboard.
        /// </summary>
        Clipboard = 1024,

        /// <summary>
        /// Draw the swing target outline.
        /// </summary>
        SwingTarget = 2048,

        /// <summary>
        /// Draw nameplates.
        /// </summary>
        NamePlates = 4096,

        /// <summary>
        /// Draw water and other transparent blocks.
        /// </summary>
        TransparentBlocks = 8192,

        /// <summary>
        /// Draw zone outlines (if enabled).
        /// </summary>
        Zones = 16384,

        /// <summary>
        /// Draw area indicators (if enabled).
        /// </summary>
        AreaIndicators = 32768,

        /// <summary>
        /// Draw script tools (script ray, sphere, etc.).
        /// </summary>
        ScriptTools = 65536,

        /// <summary>
        /// Draw actor bounds and frustums (if enabled).
        /// </summary>
        Bounds = 131072,

        /// <summary>
        /// Draw clouds.
        /// </summary>
        Clouds = 262144,

        /// <summary>
        /// Draw all elements.
        /// </summary>
        All = Background | SunAndMoon | PlayerItems | Chunks | Signs | Arcade | Splinters | Particles | Avatars | Entities | Clipboard | SwingTarget | NamePlates | TransparentBlocks | Zones | AreaIndicators | ScriptTools | Bounds | Clouds,

        /// <summary>
        /// Draw all elements except the background and sun and moon.
        /// </summary>
        AllNoBackground = PlayerItems | Chunks | Signs | Arcade | Splinters | Particles | Avatars | Entities | Clipboard | SwingTarget | NamePlates | TransparentBlocks | Zones | AreaIndicators | ScriptTools | Bounds | Clouds
    }
}
