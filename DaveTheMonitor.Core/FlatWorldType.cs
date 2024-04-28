using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Represents a type of flat world.
    /// </summary>
    public enum FlatWorldType
    {
        /// <summary>
        /// Default float world, all blocks are filled with the ground block ID.
        /// </summary>
        Default,

        /// <summary>
        /// Natural flat world, using rock layers.
        /// </summary>
        Natural,

        /// <summary>
        /// Space flat world, with no blocks and a permanent night background.
        /// </summary>
        Space,

        /// <summary>
        /// Sky flat world, with no blocks.
        /// </summary>
        Sky
    }
}
