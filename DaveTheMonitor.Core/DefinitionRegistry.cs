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
    public abstract class DefinitionRegistry<T> : IDefinitionRegistry<T> where T : IDefinition
    {
        public int Definitions => _definitionsArray.Length;
        protected Type IgnoreAttribute { get; private set; }
        protected ICoreGame Game { get; private set; }
        private Dictionary<string, T> _definitions;
        private T[] _definitionsArray;

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

        public void RegisterTypesAndJson<TJson>(ICoreMod mod, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            if (mod.Assembly != null)
            {
                RegisterTypes(mod.Assembly, mod);
            }
            RegisterJson<TJson>(mod, jsonPath);
        }

        public void RegisterAllTypesAndJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            foreach (ICoreMod mod in mods)
            {
                RegisterTypesAndJson<TJson>(mod, jsonPath);
            }
        }

        public void RegisterAllJson<TJson>(IEnumerable<ICoreMod> mods, string jsonPath) where TJson : T, IJsonType<TJson>
        {
            foreach (ICoreMod mod in mods)
            {
                RegisterJson<TJson>(mod, jsonPath);
            }
        }

        protected abstract void OnRegister(T definition);

        public T GetDefinition(string id)
        {
            TryGetDefinition(id, out T definition);
            return definition;
        }

        public T GetDefinition(int id)
        {
            return _definitionsArray[id];
        }

        public bool HasDefinition(string id)
        {
            return GetDefinition(id) != null;
        }

        /// <summary>
        /// Tries to get the particle definition with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the definition to get.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>True if the definition exists, otherwise false.</returns>
        public bool TryGetDefinition(string id, out T definition)
        {
            return _definitions.TryGetValue(id, out definition);
        }

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

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all registered particle definitions.
        /// </summary>
        /// <returns>All registered particle definitions.</returns>
        public IEnumerable<T> GetAllDefinitions()
        {
            return _definitionsArray;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_definitionsArray).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitionsArray.GetEnumerator();
        }

        public DefinitionRegistry(ICoreGame game, Type ignoreAttribute)
        {
            Game = game;
            IgnoreAttribute = ignoreAttribute;
            _definitions = new Dictionary<string, T>();
            _definitionsArray = Array.Empty<T>();
        }
    }
}
