using DaveTheMonitor.Core.Components;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A definition that can be created from a Json object.
    /// </summary>
    /// <typeparam name="TSelf">The type that implements this interface.</typeparam>
    public interface IJsonType<TSelf>
    {
        /// <summary>
        /// This item's defined components.
        /// </summary>
        ComponentCollection Components { get; }

        /// <summary>
        /// Parses a Json string as <typeparamref name="TSelf"/>.
        /// </summary>
        /// <param name="json">The Json string to parse.</param>
        /// <returns>A <typeparamref name="TSelf"/> from the Json.</returns>
        abstract static TSelf FromJson(string json);

        /// <summary>
        /// Replaces this item with another.
        /// </summary>
        /// <param name="mod">The mod that defines the replacement.</param>
        /// <param name="other">The item to replace this item with.</param>
        void ReplaceWith(ICoreMod mod, IJsonType<TSelf> other);
    }
}
