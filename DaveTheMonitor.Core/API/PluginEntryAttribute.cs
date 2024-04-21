using System;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// The type that should be used as the mod's plugin. This is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PluginEntryAttribute : Attribute
    {

    }
}
