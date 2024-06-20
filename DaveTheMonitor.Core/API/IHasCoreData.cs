using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Implemented by types with <see cref="ICoreData{T}"/>.
    /// </summary>
    /// <typeparam name="TSelf">The type that implements this interface.</typeparam>
    public interface IHasCoreData<TSelf>
    {
        /// <summary>
        /// Gets the data of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns>The data of type <typeparamref name="T"/>, or default(T) if no data is found.</returns>
        T GetData<T>() where T : ICoreData<TSelf>;

        /// <summary>
        /// Sets <paramref name="result"/> to the data of type <typeparamref name="T"/>, or default if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns>True if the data exists, otherwise false.</returns>
        bool TryGetData<T>(out T result) where T : ICoreData<TSelf>;

        /// <summary>
        /// Clears and fills <paramref name="result"/> with the data for this item.
        /// </summary>
        /// <param name="result">The list to fill.</param>
        void GetAllData(List<ICoreData<TSelf>> result);

        /// <summary>
        /// Returns true if this item has data of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <returns>True if the data exists, otherwise false.</returns>
        bool HasData<T>();

        /// <summary>
        /// Sets the data of type of <paramref name="data"/> to the value. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        void SetData(ICoreData<TSelf> data);

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to the value. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        void SetData<T>(T data) where T : ICoreData<TSelf>;

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to a new instance of <typeparamref name="T"/>. It's best practice to only call this for your own data.
        /// </summary>
        /// <typeparam name="T">The type of data to set.</typeparam>
        /// <returns>The newly created data.</returns>
        T SetData<T>() where T : ICoreData<TSelf>, new();

        /// <summary>
        /// Sets the data of type of <paramref name="data"/> to the value if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        /// <returns>The existing data if data of type of <paramref name="data"/> if it already exists, otherwise <paramref name="data"/>.</returns>
        ICoreData<TSelf> SetDefaultData(ICoreData<TSelf> data);

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to the value if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <param name="data">The data to set.</param>
        /// <returns>The existing data if data of type <typeparamref name="T"/> already exists, otherwise <paramref name="data"/>.</returns>
        T SetDefaultData<T>(T data) where T : ICoreData<TSelf>;

        /// <summary>
        /// Sets the data of type <typeparamref name="T"/> to a new instance of <typeparamref name="T"/> if it doesn't already exist. It's best practice to only call this for your own data.
        /// </summary>
        /// <typeparam name="T">The type of data to set.</typeparam>
        /// <returns>The newly created data, or the existing data if it already exists.</returns>
        T SetDefaultData<T>() where T : ICoreData<TSelf>, new();
    }
}
