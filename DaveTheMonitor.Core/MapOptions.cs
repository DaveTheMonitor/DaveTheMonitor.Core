using StudioForge.BlockWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Options for creating a new map.
    /// </summary>
    public sealed class MapOptions
    {
        /// <summary>
        /// The map tile size in meters.
        /// </summary>
        public float TileSize { get; set; }

        /// <summary>
        /// The map size.
        /// </summary>
        public Point3D MapSize { get; set; }

        /// <summary>
        /// The region size.
        /// </summary>
        public Point3D RegionSize { get; set; }

        /// <summary>
        /// The chunk size.
        /// </summary>
        public Point3D ChunkSize { get; set; }

        /// <summary>
        /// The map strategy.
        /// </summary>
        public MapStrategy MapStrategy { get; set; }

        /// <summary>
        /// If true, the mesh creator will make chunks fade in when they're generated.
        /// </summary>
        public bool AllowMeshCreatorToSplitOrFade { get; set; }

        /// <summary>
        /// The sea level of the map.
        /// </summary>
        public ushort SeaLevel { get; set; }
    }
}
