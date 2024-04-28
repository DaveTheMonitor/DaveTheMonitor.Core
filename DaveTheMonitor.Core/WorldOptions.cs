using DaveTheMonitor.Core.API;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{

    /// <summary>
    /// Options for controlling the generation and definition of a world.
    /// </summary>
    public sealed record WorldOptions
    {
        /// <summary>
        /// The size of the world. This is the main world's size by default.
        /// </summary>
        public Point3D? Size { get; init; } = null;

        /// <summary>
        /// The region size of the world. This is the main world's region size by default.
        /// </summary>
        public Point3D? RegionSize { get; init; } = null;

        /// <summary>
        /// The chunk size of the world. This is the main world's chunk size by default.
        /// </summary>
        public Point3D? ChunkSize { get; init; } = null;

        /// <summary>
        /// The map strategy used by the map. This is a new map strategy (null) by default.
        /// </summary>
        /// <remarks>
        /// This is expected to be a MapStrategyTM. If it is not, an exception will be thrown on world initialization.
        /// </remarks>
        public MapStrategy MapStrategy { get; init; } = null;

        /// <summary>
        /// The default draw options of the world. This is <see cref="WorldDrawOptions.All"/> by default.
        /// </summary>
        public WorldDrawOptions DrawOptions { get; init; } = WorldDrawOptions.All;

        /// <summary>
        /// The biome generator to use. This is the main world's type by default.
        /// </summary>
        public BiomeType? Biome { get; init; } = null;

        /// <summary>
        /// The biome params to use when generating this world. This is a copy of the main world's params by default.
        /// </summary>
        public BiomeParams BiomeParams { get; init; } = null;

        /// <summary>
        /// <para>The ground block to use for flat worlds. Unused if the world is not a flat world.</para>
        /// <para>
        /// By default, this is:
        /// <list type="bullet">
        /// <item><see cref="Block.Grass"/> if the <see cref="FlatWorldType"/> is <see cref="FlatWorldType.Natural"/>, <see cref="FlatWorldType.Default"/>,</item>
        /// <item><see cref="Block.None"/> if the <see cref="FlatWorldType"/> is <see cref="FlatWorldType.Sky"/> or <see cref="FlatWorldType.Space"/>,</item>
        /// <item>The main world's ground block if the main world is flat and <see cref="FlatWorldType"/> is null,</item>
        /// <item><see cref="Block.Grass"/> if the main world is not flat and <see cref="FlatWorldType"/> is null.</item>
        /// </list>
        /// </para>
        /// </summary>
        public Block? GroundBlock { get; init; } = null;

        /// <summary>
        /// <para>The type of flat world if this world's biome is <see cref="BiomeType.Flat"/>. This is the main world's flat type (or natural if the main world isn't flat) by default.</para>
        /// <para>
        /// By default, this is:
        /// <list type="bullet">
        /// <item><see cref="FlatWorldType.Default"/> if <see cref="GroundBlock"/> is not null,</item>
        /// <item>The main world's flat type if the main world is flat and <see cref="GroundBlock"/> is null,</item>
        /// <item><see cref="FlatWorldType.Natural"/> if the main world is not flat and <see cref="GroundBlock"/> is null.</item>
        /// </list>
        /// </para>
        /// </summary>
        public FlatWorldType? FlatWorldType { get; init; } = null;

        /// <summary>
        /// The sea level of the world. This is the main world's sea level by default.
        /// </summary>
        public ushort? SeaLevel { get; init; } = null;

        /// <summary>
        /// The seed used to generate this map. This is a combination of the main world seed and world ID by default. Set this to a constant value if you want the world to generate the same for every main world.
        /// </summary>
        public int? Seed { get; init; } = null;
    }
}
