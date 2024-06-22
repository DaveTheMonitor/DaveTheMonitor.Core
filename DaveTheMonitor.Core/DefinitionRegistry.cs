using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// An abstract definition registry for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of definition this registry stores.</typeparam>
    public abstract class DefinitionRegistry<T> : IDefinitionRegistry<T> where T : IDefinition
    {
        /// <inheritdoc/>
        public T this[int numId] => GetDefinition(numId);
        /// <inheritdoc/>
        public T this[string id] => GetDefinition(id);
        /// <inheritdoc/>
        public int Definitions => _definitionsArray.Length;

        /// <summary>
        /// The type that marks an implementation/subclass of <typeparamref name="T"/> to not be automatically registered.
        /// </summary>
        protected Type IgnoreAttribute { get; private set; }

        /// <summary>
        /// The main game instance.
        /// </summary>
        protected ICoreGame Game { get; private set; }
        private Dictionary<string, T> _definitions;
        private T[] _definitionsArray;

        /// <inheritdoc/>
        public void RegisterTypes(Assembly assembly, ICoreMod mod)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(T)) && !type.IsAbstract)
                {
                    if (IgnoreAttribute != null && type.GetCustomAttribute(IgnoreAttribute) != null)
                    {
                        continue;
                    }
#if DEBUG
                    CorePlugin.Log($"Auto-registering definition {type.FullName}");
#endif
                    T definition = (T)Activator.CreateInstance(type);
                    RegisterDefinition(definition, mod);
                }
            }
        }

        /// <inheritdoc/>
        public void RegisterDefinition(T definition, ICoreMod mod)
        {
            if (_definitions.ContainsKey(definition.Id))
            {
#if DEBUG
                CorePlugin.Warn($"Definition {definition.Id} already registered.");
#endif
                _definitions[definition.Id] = definition;
                return;
            }

            int index = _definitionsArray.Length;
            Array.Resize(ref _definitionsArray, index + 1);
            definition.NumId = index;
            OnRegister(definition);
            _definitionsArray[index] = definition;
            _definitions.Add(definition.Id, definition);
            definition.OnRegister(mod);
        }

        /// <inheritdoc/>
        public void RegisterJsonDefinition<TJson>(TJson definition, ICoreMod mod) where TJson : T, IJsonType<TJson>
        {
            if (_definitions.TryGetValue(definition.Id, out T orig))
            {
#if DEBUG
                CorePlugin.Warn($"Definition {definition.Id} already registered.");
#endif
                if (orig is not TJson origJson)
                {
                    _definitions[definition.Id] = definition;
                    return;
                }

                origJson.ReplaceWith(mod, definition);
                return;
            }

            int index = _definitionsArray.Length;
            Array.Resize(ref _definitionsArray, index + 1);
            definition.NumId = index;
            OnRegister(definition);
            _definitionsArray[index] = definition;
            _definitions.Add(definition.Id, definition);
            definition.OnRegister(mod);
        }

        /// <inheritdoc/>
        public void RegisterJson<TJson>(ICoreMod mod, string path) where TJson : T, IJsonType<TJson>
        {
            string fullPath = Path.Combine(mod.FullPath, path);
            if (!Directory.Exists(fullPath))
            {
                return;
            }

            TJson[] arr = DeserializationHelper.ReadAllFromPath<TJson>(fullPath, SearchOption.AllDirectories);
            if (arr != null)
            {
                foreach (TJson item in arr)
                {
#if DEBUG
                    CorePlugin.Log($"Registering defintion from Json {Path.GetFileName(item.Id)}");
#endif
                    RegisterJsonDefinition<TJson>(item, mod);
                }
            }
        }

        /// <inheritdoc/>
        public void RegisterTypesAndJson<TJson>(ICoreMod mod, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            if (mod.Assembly != null)
            {
                RegisterTypes(mod.Assembly, mod);
            }
            RegisterJson<TJson>(mod, jsonPath);
        }

        /// <inheritdoc/>
        public void RegisterAllTypesAndJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            foreach (ICoreMod mod in mods)
            {
                RegisterTypesAndJson<TJson>(mod, jsonPath);
            }
        }

        /// <inheritdoc/>
        public void RegisterAllJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            foreach (ICoreMod mod in mods)
            {
                RegisterJson<TJson>(mod, jsonPath);
            }
        }

        /// <summary>
        /// Called when a definition is registered.
        /// </summary>
        /// <param name="definition">The definition that was registered.</param>
        protected abstract void OnRegister(T definition);

        /// <inheritdoc/>
        public T GetDefinition(string id)
        {
            TryGetDefinition(id, out T definition);
            return definition;
        }

        /// <inheritdoc/>
        public T GetDefinition(int id)
        {
            return _definitionsArray[id];
        }

        /// <inheritdoc/>
        public bool HasDefinition(string id)
        {
            return GetDefinition(id) != null;
        }

        /// <inheritdoc/>
        public bool TryGetDefinition(string id, out T definition)
        {
            return _definitions.TryGetValue(id, out definition);
        }

        /// <inheritdoc/>
        public bool TryGetDefinition(int id, out T definition)
        {
            if (id < 0 || id >= _definitionsArray.Length)
            {
                definition = default(T);
                return false;
            }
            definition = _definitionsArray[id];
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_definitionsArray).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitionsArray.GetEnumerator();
        }

        /// <summary>
        /// Creates a new <see cref="DefinitionRegistry{T}"/>.
        /// </summary>
        /// <param name="game">The main game instance.</param>
        /// <param name="ignoreAttribute">The attribute that marks a class to not be automatically registered.</param>
        public DefinitionRegistry(ICoreGame game, Type ignoreAttribute)
        {
            Game = game;
            IgnoreAttribute = ignoreAttribute;
            _definitions = new Dictionary<string, T>();
            _definitionsArray = Array.Empty<T>();
        }
    }
}
