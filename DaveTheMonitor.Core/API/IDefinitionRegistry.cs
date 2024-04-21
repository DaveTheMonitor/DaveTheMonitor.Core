using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A definition registry containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of definitions for this <see cref="IDefinitionRegistry{T}"/>.</typeparam>
    public interface IDefinitionRegistry<T> : IEnumerable<T> where T : IDefinition
    {
        /// <summary>
        /// The number of registered definitions.
        /// </summary>
        int Definitions { get; }

        /// <summary>
        /// Registers all types that inherit from <typeparamref name="T"/> in the specified assembly as definitions.
        /// </summary>
        /// <param name="assembly">The assembly to register.</param>
        /// <param name="mod">The mod that the assembly belongs to.</param>
        void RegisterTypes(Assembly assembly, ICoreMod mod);

        /// <summary>
        /// Registers a single definition.
        /// </summary>
        /// <param name="definition">The definition to register.</param>
        /// <param name="mod">The mod that defines this definition.</param>
        void RegisterDefinition(T definition, ICoreMod mod);

        /// <summary>
        /// Registers a single Json definition.
        /// </summary>
        /// <typeparam name="TJson">The type of definition to register.</typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="mod">The mod that defines this definition.</param>
        void RegisterJsonDefinition<TJson>(TJson definition, ICoreMod mod) where TJson : T, IJsonType<TJson>;

        /// <summary>
        /// Registers all Json definitions in the specified directory.
        /// </summary>
        /// <typeparam name="TJson">The type of definition to register.</typeparam>
        /// <param name="mod">The mod that defines the definitions in the directory.</param>
        /// <param name="path">The directory to register the definitions from.</param>
        void RegisterJson<TJson>(ICoreMod mod, string path) where TJson : T, IJsonType<TJson>;

        /// <summary>
        /// Registers all types that inherit <typeparamref name="T"/>, and Json definitions in the specified directory.
        /// </summary>
        /// <typeparam name="TJson">The type of Json definition to register.</typeparam>
        /// <param name="mod">The mod that defines the definitions.</param>
        /// <param name="jsonPath">The directory to register the Json definitions from.</param>
        void RegisterTypesAndJson<TJson>(ICoreMod mod, string jsonPath) where TJson : T, IJsonType<TJson>;

        /// <summary>
        /// Registers all types that inherit <typeparamref name="T"/>, and Json definitions in the specified directory, for all specified mods.
        /// </summary>
        /// <typeparam name="TJson">The type of Json definition to register.</typeparam>
        /// <param name="mods">The mods to register definitions for.</param>
        /// <param name="jsonPath">The directory to register the Json definitions from.</param>
        void RegisterAllTypesAndJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>;

        /// <summary>
        /// Registers all Json definitions in the specified directory for all specified mods.
        /// </summary>
        /// <typeparam name="TJson">The type of Json definition to register.</typeparam>
        /// <param name="mods">The mods to register definitions for.</param>
        /// <param name="jsonPath">The directory to register the Json definitions from.</param>
        void RegisterAllJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>;

        /// <summary>
        /// Gets the definition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the definition.</param>
        /// <returns>The definition with the specified ID, or default(T) if no definition is found.</returns>
        T GetDefinition(string id);

        /// <summary>
        /// Gets the definition with the specified numeric ID.
        /// </summary>
        /// <param name="id">The numeric ID of the definition.</param>
        /// <returns>The definition with the specified numeric ID, or default(T) if no definition is found.</returns>
        T GetDefinition(int id);

        /// <summary>
        /// Returns true if a definition with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the definition.</param>
        /// <returns>True if the definition exists, otherwise false.</returns>
        bool HasDefinition(string id);

        /// <summary>
        /// Returns true if a definition with the specified ID exists, and sets <paramref name="definition"/> to the registered definition.
        /// </summary>
        /// <param name="id">The ID of the definition.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>True if the definition exists, otherwise false.</returns>
        bool TryGetDefinition(string id, out T definition);

        /// <summary>
        /// Returns true if a definition with the specified numeric ID exists, and sets <paramref name="definition"/> to the registered definition.
        /// </summary>
        /// <param name="id">The numeric ID of the definition.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>True if the definition exists, otherwise false.</returns>
        bool TryGetDefinition(int id, out T definition);
    }
}
