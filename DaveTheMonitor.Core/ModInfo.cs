using StudioForge.TotalMiner;
using System;
using System.Text.Json.Serialization;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Basic info for a Core mod.
    /// </summary>
    public struct ModInfo
    {
        /// <summary>
        /// The ID of the mod.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ID")]
        public string Id { get; private set; }

        /// <summary>
        /// The version string of the mod. eg. "1.2.3"
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("Version")]
        public string Version { get; private set; }

        /// <summary>
        /// The dependencies of the mod.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("Dependencies")]
        public string[] Dependencies { get; private set; }

        /// <summary>
        /// The name of the DLL file containing the mod's plugin.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("Plugin")]
        public string Plugin { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ModInfo"/> instance.
        /// </summary>
        /// <param name="id">The ID of the mod.</param>
        /// <param name="version">The version of the mod.</param>
        public ModInfo(string id, ModVersion version)
            : this(id, version, Array.Empty<string>(), null)
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="ModInfo"/> instance.
        /// </summary>
        /// <param name="id">The ID of the mod.</param>
        /// <param name="version">The version of the mod.</param>
        /// <param name="dependencies">The dependencies of the mod.</param>
        /// <param name="plugin">The name of the DLL file containing the mod's plugin.</param>
        public ModInfo(string id, ModVersion version, string[] dependencies, string plugin)
        {
            Id = id;
            Version = version.ToString();
            Dependencies = dependencies;
            Plugin = plugin;
        }
    }
}
