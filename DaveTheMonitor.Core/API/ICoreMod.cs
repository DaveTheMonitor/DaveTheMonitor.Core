using DaveTheMonitor.Core.Assets;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System.Reflection;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A mod loaded by the Core Mod.
    /// </summary>
    public interface ICoreMod
    {
        /// <summary>
        /// The full path of this mod.
        /// </summary>
        string FullPath { get; }
        
        /// <summary>
        /// The ID of this mod.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The type of this mod.
        /// </summary>
        ModType Type { get; }

        /// <summary>
        /// This mod's plugin, if it has one.
        /// </summary>
        ICorePlugin Plugin { get; }

        /// <summary>
        /// This mod's <see cref="ITMMod"/>.
        /// </summary>
        ITMMod TMMod { get; }

        /// <summary>
        /// This mod's version.
        /// </summary>
        ModVersion Version { get; }

        /// <summary>
        /// This mod's type offsets.
        /// </summary>
        EnumTypeOffsets TypeOffsets { get; }

        /// <summary>
        /// The assembly containing this mod's plugin, if it has one.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// This mod's MonoGame <see cref="ContentManager"/>.
        /// </summary>
        ContentManager MGContent { get; }

        /// <summary>
        /// Gets a <see cref="CoreModAsset"/> from this mod.
        /// </summary>
        /// <param name="name">The name of the asset to get.</param>
        /// <returns>The asset with the specified name, or null if the asset is not found.</returns>
        CoreModAsset GetAsset(string name);

        /// <summary>
        /// Gets a <see cref="CoreModAsset"/> from this mod as <typeparamref name="T"/>.
        /// </summary>
        /// <param name="name">The name of the asset to get.</param>
        /// <returns>The asset with the specified name as <typeparamref name="T"/>.</returns>
        T GetAsset<T>(string name) where T : CoreModAsset;

        /// <summary>
        /// Gets a <see cref="Texture2D"/> asset added by this mod.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="size">The expected size of this texture. If the texture is not found, a missing texture with this size is returned. The texture added by this mod may not be this size.</param>
        /// <returns>The texture asset from this mod.</returns>
        Texture2D GetTexture(string name, int size);

        /// <summary>
        /// Gets a <see cref="ICoreMap"/> component asset added by this mod.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <returns>The component asset from this mod.</returns>
        ICoreMap GetComponent(string name);

        /// <summary>
        /// Gets the full path to an asset added by this mod.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The full path to the asset, or null if the asset is not found.</returns>
        string GetFullPathToAsset(string name);

        /// <summary>
        /// Loads this mod with the specified <see cref="ModInfo"/> and <see cref="IMapComponentLoader"/>.
        /// </summary>
        /// <param name="info">The info of the mod.</param>
        /// <param name="componentLoader">The loader to use when loading components.</param>
        void Load(ModInfo info, IMapComponentLoader componentLoader);

        /// <summary>
        /// Unloads this mod.
        /// </summary>
        void Unload();
    }
}
