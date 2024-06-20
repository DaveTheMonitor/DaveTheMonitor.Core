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
        /// The priority of this data. Lower priority data is added first. The default priority is 100. This number should not change for any data instance in a session.
        /// </summary>
        int Priority => 100;

        /// <summary>
        /// Initializes this data with <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item that owns this data.</param>
        void Initialize(T item);
    }
}
