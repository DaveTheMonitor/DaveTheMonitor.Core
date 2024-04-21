namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Implemented by types with <see cref="ICoreData{T}"/>.
    /// </summary>
    /// <typeparam name="TSelf">The type that implements this interface.</typeparam>
    public interface IHasCoreData<TSelf>
    {
        /// <summary>
        /// Gets the data from the specified mod as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <param name="mod">The mod that added the data.</param>
        /// <returns>The data from the specified mod as <typeparamref name="T"/>, or default(T) if no data is found.</returns>
        T GetData<T>(ICoreMod mod) where T : ICoreData<TSelf>;

        /// <summary>
        /// Sets the specified mod's data to the specified value. It's best practice to only call this for your own mod.
        /// </summary>
        /// <param name="mod">The mod to set the data for.</param>
        /// <param name="data">The data to set.</param>
        void SetData(ICoreMod mod, ICoreData<TSelf> data);

        /// <summary>
        /// Sets the specified mod's data to a new instance of <typeparamref name="T"/>. It's best practice to only call this for your own mod.
        /// </summary>
        /// <typeparam name="T">The type of data to set.</typeparam>
        /// <param name="mod">The mod to set the data for.</param>
        T SetDefaultData<T>(ICoreMod mod) where T : ICoreData<TSelf>, new();
    }
}
