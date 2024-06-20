using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Assets.Loaders;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner;
using System;
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
        int ActiveMods { get; }

        /// <summary>
        /// Returns true if the mod with the specified ID is active.
        /// </summary>
        /// <param name="id">The ID of the mod to test.</param>
        /// <returns>True if the mod with the specified ID is active, otherwise false.</returns>
        bool IsModActive(string id);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of all active mods.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all active mods.</returns>
        IEnumerable<ICoreMod> GetAllActiveMods();

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of all active mods with a plugin.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all active mods with a plugin.</returns>
        IEnumerable<ICoreMod> GetAllActivePlugins();

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the mod to get.</param>
        /// <returns>The <see cref="ICoreMod"/> with the specified ID, or null if it is not found.</returns>
        ICoreMod GetMod(string id);

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> that defines the specified <see cref="ActorType"/>.
        /// </summary>
        /// <param name="actor">The actor assetType.</param>
        /// <returns>The <see cref="ICoreMod"/> that defines the specified <see cref="ActorType"/>.</returns>
        ICoreMod GetDefiningMod(ActorType actor);

        /// <summary>
        /// Gets the <see cref="ICoreMod"/> that defines the specified <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The item assetType.</param>
        /// <returns>The <see cref="ICoreMod"/> that defines the specified <see cref="Item"/>.</returns>
        ICoreMod GetDefiningMod(Item item);

        /// <summary>
        /// Loads a <see cref="CoreModAsset"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the asset is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        CoreModAsset LoadAsset(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="CoreModAsset"/> from any mod as <typeparamref name="T"/>, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the asset is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        T LoadAsset<T>(ICoreMod mod, string name) where T : CoreModAsset;

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the texture is from.</param>
        /// <returns>The asset with the specified name, or a 16x missing asset if the texture is not found.</returns>
        Texture2D LoadTexture(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the texture is from.</param>
        /// <param name="returnMissingTexture">If true, this method will return a missing texture of size 16 if the asset is not found.</param>
        /// <returns>The asset with the specified name, or null or a missing asset if the texture is not found.</returns>
        Texture2D LoadTexture(ICoreMod mod, string name, bool returnMissingTexture);

        /// <summary>
        /// Loads a <see cref="ICoreMap"/> component from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the component is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        ICoreMap LoadComponent(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="ActorModel"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the component is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        ActorModel LoadActorModel(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="JsonActorAnimation"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the component is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        JsonActorAnimation LoadActorAnimation(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="JsonAnimationController"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the component is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        JsonAnimationController LoadAnimationController(ICoreMod mod, string name);

        /// <summary>
        /// Loads a <see cref="SoundEffect"/> from any mod, getting the asset from <paramref name="mod"/> if the ID doesn't specify a target mod.
        /// </summary>
        /// <param name="mod">The default fallback mod.</param>
        /// <param name="name">The name of this asset. The name can specify a mod ID that the component is from.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        SoundEffect LoadSound(ICoreMod mod, string name);

        /// <summary>
        /// Calls <see cref="ICorePlugin.ModCall(object[])"/> for the mod with the specified ID, if it exists.
        /// </summary>
        /// <param name="modId">The ID of the mod to call.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The value returned by the mod, or null if no value was returned.</returns>
        object Call(string modId, params object[] args);

        /// <summary>
        /// Adds a loader for the specified asset type that will be used for all mods with the asset.
        /// </summary>
        /// <param name="assetType">The type the asset will be loaded as.</param>
        /// <param name="path">The path this asset must be in.</param>
        /// <param name="filter">The file filter.</param>
        /// <param name="loader">The loader used to load the asset.</param>
        void AddAssetLoader(Type assetType, string path, string filter, ICoreAssetLoader loader);
    }
}
