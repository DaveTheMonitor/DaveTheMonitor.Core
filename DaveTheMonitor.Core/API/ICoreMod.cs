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
        /// This mod's content manager. Used to load content, eg. textures.
        /// </summary>
        ModContentManager Content { get; }

        /// <summary>
        /// The <see cref="ICoreModManager"/> that loaded this mod.
        /// </summary>
        ICoreModManager ModManager { get; }

        /// <summary>
        /// Loads this mod with the specified <see cref="ModInfo"/> and <see cref="IMapComponentLoader"/>.
        /// </summary>
        /// <param name="info">The info of the mod.</param>
        void Load(ModInfo info);

        /// <summary>
        /// Unloads this mod.
        /// </summary>
        void Unload();
    }
}
