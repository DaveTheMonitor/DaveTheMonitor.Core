using DaveTheMonitor.Core.Assets;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Manages active mods.
    /// </summary>
    public interface ICoreModManager
    {
        /// <summary>
        /// The number of currently active mods.
        /// </summary>
        public int ActiveMods { get; }

        /// <summary>
        /// Returns true if the mod with the specified ID is active.
        /// </summary>
        /// <param name="id">The ID of the mod to test.</param>
        /// <returns>True if the mod with the specified ID is active, otherwise false.</returns>
        public bool IsModActive(string id);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of all active mods.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all active mods.</returns>
        public IEnumerable<ICoreMod> GetAllActiveMods();

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of all active mods with a plugin.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all active mods with a plugin.</returns>
        public IEnumerable<ICoreMod> GetAllActivePlugins();

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the mod to get.</param>
        /// <returns>The <see cref="ICoreMod"/> with the specified ID, or null if it is not found.</returns>
        public ICoreMod GetMod(string id);

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> that defines the specified <see cref="ActorType"/>.
        /// </summary>
        /// <param name="actor">The actor type.</param>
        /// <returns>The <see cref="ICoreMod"/> that defines the specified <see cref="ActorType"/>.</returns>
        public ICoreMod GetDefiningMod(ActorType actor);

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> that defines the specified <see cref="Item"/>.
        /// </summary>
        /// <param name="actor">The actor type.</param>
        /// <returns>The <see cref="ICoreMod"/> that defines the specified <see cref="Item"/>.</returns>
        public ICoreMod GetDefiningMod(Item item);

        /// <summary>
        /// Gets a <see cref="CoreModAsset"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="fullId">The full ID of this asset. The asset ID can specify a mod ID that the asset is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        public CoreModAsset GetAsset(ICoreMod mod, string fullId);

        /// <summary>
        /// Gets a <see cref="CoreModAsset"/> from any mod as <typeparamref name="T"/>, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="fullId">The full ID of this asset. The asset ID can specify a mod ID that the asset is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        public T GetAsset<T>(ICoreMod mod, string fullId) where T : CoreModAsset;

        /// <summary>
        /// Gets a <see cref="Texture2D"/> from any mod, getting the texture from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="fullId">The full ID of this texture. The texture ID can specify a mod ID that the texture is from.</param>
        /// <param name="size">The expected size of this texture. If the texture is not found, a missing texture with this size is returned. The texture added by this mod may not be this size.</param>
        /// <returns>The texture with the specified name, or null if the texture is not found.</returns>
        public Texture2D GetTexture(ICoreMod mod, string fullId, int size);

        /// <summary>
        /// Gets a <see cref="ICoreMap"/> component from any mod, getting the component from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="fullId">The full ID of this component. The component ID can specify a mod ID that the component is from.</param>
        /// <returns>The component with the specified name, or null if the component is not found.</returns>
        public ICoreMap GetComponent(ICoreMod mod, string fullId);

        /// <summary>
        /// Calls <see cref="ICorePlugin.ModCall(object[])"/> for the mod with the specified ID, if it exists.
        /// </summary>
        /// <param name="modId">The ID of the mod to call.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The value returned by the mod, or null if no value was returned.</returns>
        public object Call(string modId, params object[] args);
    }
}
