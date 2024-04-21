using System;

namespace DaveTheMonitor.Core.Components
{
    public sealed class ComponentException : Exception
    {
        public ComponentException(Type type, string message) : base($"{type.FullName}: {message}")
        {
            
        }
    }
}
