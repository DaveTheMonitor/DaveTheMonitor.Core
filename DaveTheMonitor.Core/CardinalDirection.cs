namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Represents a cardinal direction.
    /// </summary>
    public enum CardinalDirection
    {
        /// <summary>
        /// North, -z (<see cref="Microsoft.Xna.Framework.Vector3.Forward"/>)
        /// </summary>
        North = 0,
        /// <summary>
        /// East, -x (<see cref="Microsoft.Xna.Framework.Vector3.Left"/>)
        /// </summary>
        East = 1,
        /// <summary>
        /// South, +z (<see cref="Microsoft.Xna.Framework.Vector3.Backward"/>)
        /// </summary>
        South = 2,
        /// <summary>
        /// West, +x (<see cref="Microsoft.Xna.Framework.Vector3.Right"/>)
        /// </summary>
        West = 3
    }
}
