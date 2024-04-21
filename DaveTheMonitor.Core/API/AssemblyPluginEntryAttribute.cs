using System;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Specifies the <see cref="ICorePlugin"/> implementation for this assembly.
    /// </summary>
    /// <remarks>This attribute is recommended, but not required.</remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class AssemblyPluginEntryAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="ICorePlugin"/> implementation for this assembly.
        /// </summary>
        public Type PluginType { get; set; }

        public AssemblyPluginEntryAttribute(Type pluginType)
        {
            PluginType = pluginType;
        }
    }
}
