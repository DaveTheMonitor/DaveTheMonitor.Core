using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// The type of a mod.
    /// </summary>
    public enum ModType
    {
        /// <summary>
        /// <para>Used for mods with no Core functionality. Mods that don't contain any Core data will use this.</para>
        /// <para>If the Core Mod is used with non-core mods, those mods are still be accessible from the <see cref="ICoreModManager"/>. They use this mod type to differentiate them from Core mods.</para>
        /// </summary>
        TM,

        /// <summary>
        /// Used for mods with Core functionality. Mods that have Core Json data or a Core plugin will use this.
        /// </summary>
        Core
    }
}
