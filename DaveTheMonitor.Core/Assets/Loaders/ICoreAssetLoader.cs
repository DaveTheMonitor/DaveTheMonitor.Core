using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Assets.Loaders
{
    /// <summary>
    /// A loader used to load core assets. Implement this to add your own loader.
    /// </summary>
    public interface ICoreAssetLoader
    {
        /// <summary>
        /// Loads the asset from the specified path.
        /// </summary>
        /// <param name="fullPath">The full path of the asset to load.</param>
        /// <param name="name">The name of the asset.</param>
        /// <param name="mod">The mod this asset belongs to.</param>
        /// <returns>The loaded asset.</returns>
        CoreModAsset Load(string fullPath, string name, ICoreMod mod);
    }
}
