using System;

namespace DaveTheMonitor.Core.Patches
{
    public sealed class PatchException : Exception
    {
        public PatchException(Type patchType, string message) : base($"{patchType.FullName}: {message}")
        {
            
        }
    }
}
