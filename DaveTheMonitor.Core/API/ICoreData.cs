namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Custom data for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that owns this data.</typeparam>
    public interface ICoreData<T> : IHasBinaryState
    {
        /// <summary>
        /// True if this data should save binary state, otherwise false.
        /// </summary>
        bool ShouldSave { get; }

        /// <summary>
        /// Initializes this data with <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item that owns this data.</param>
        void Initialize(T item);
    }
}
