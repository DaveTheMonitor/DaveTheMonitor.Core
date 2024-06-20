using DaveTheMonitor.Core.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Used to automatically initialize default data for an item.
    /// </summary>
    /// <typeparam name="T">The type of the item that has the data.</typeparam>
    public sealed class CoreDataInitializer<T> where T : IHasCoreData<T>
    {
        /// <summary>
        /// The types this <see cref="CoreDataInitializer{T}"/> creates and initializes.
        /// </summary>
        public IEnumerable<Type> Types => _types;
        private static MethodInfo _createMethod = typeof(CoreDataInitializer<T>).GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Static);
        private List<Func<ICoreData<T>>> _constructors;
        private List<Type> _types;

        private static ICoreData<T> Create<TImpl>() where TImpl : ICoreData<T>, new()
        {
            return new TImpl();
        }

        /// <summary>
        /// Adds a new type to be initialized. The type must implement <see cref="ICoreData{T}"/> and have a public parameterless constructor.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <exception cref="ArgumentException">The type is not valid for automatic initialization.</exception>
        public void AddType(Type type)
        {
            if (!type.IsAssignableTo(typeof(ICoreData<T>)))
            {
                throw new ArgumentException($"{type} must implement {typeof(ICoreData<T>).Name}.", nameof(type));
            }
            if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>()) == null)
            {
                throw new ArgumentException($"{type} must have a public parameterless constructor.", nameof(type));
            }

            MethodInfo method = _createMethod.MakeGenericMethod(type);
            Func<ICoreData<T>> func = method.CreateDelegate<Func<ICoreData<T>>>();
            _constructors.Add(func);
            _types.Add(type);
        }

        /// <summary>
        /// Creates and initializes default data for <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to initialize data for.</param>
        public void CreateAndInitializeAll(T item)
        {
            foreach (Func<ICoreData<T>> ctor in _constructors)
            {
                ICoreData<T> data = ctor();
                item.SetDefaultData(data);
            }
        }

        /// <summary>
        /// Creates a new <see cref="CoreDataInitializer{T}"/> with no types added.
        /// </summary>
        public CoreDataInitializer()
        {
            _constructors = new List<Func<ICoreData<T>>>();
            _types = new List<Type>();
        }
    }
}
