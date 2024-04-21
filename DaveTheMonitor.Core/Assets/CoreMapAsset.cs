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
        /// Creates a new asset containing this specified map.
        /// </summary>
        /// <param name="fullPath">The full path of the component.</param>
        /// <param name="map">The map of this asset. Should be a component.</param>
        public CoreMapAsset(string fullPath, ICoreMap map) : base(fullPath)
        {
            Map = map;
        }
    }
}
