using System;

namespace DaveTheMonitor.Core.Assets
{
    public sealed class InvalidCoreAssetException : Exception
    {
        public InvalidCoreAssetException(string message) : base(message)
        {

        }

        public InvalidCoreAssetException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
