using System;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// Exception thrown when a Json string or file is invalid.
    /// </summary>
    public sealed class InvalidCoreJsonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCoreJsonException"/> class with a specified error message.
        /// </summary>
        /// <inheritdoc/>
        public InvalidCoreJsonException(string message) : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCoreJsonException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <inheritdoc/>
        public InvalidCoreJsonException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
