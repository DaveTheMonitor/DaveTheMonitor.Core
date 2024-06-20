using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing a map (component)
    /// </summary>
    public sealed class CoreMapAsset : CoreModAsset
    {
        /// <summary>
        /// The component of this asset.
        /// </summary>
        public ICoreMap Map { get; private set; }

        /// <summary>
        /// Creates a new asset containing the specified map.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="map">The map of this asset. Should be a component.</param>
        public CoreMapAsset(string fullPath, string name, ICoreMap map) : base(fullPath, name)
        {
            Map = map;
        }
    }
}
