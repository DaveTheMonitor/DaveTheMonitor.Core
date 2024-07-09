using DaveTheMonitor.Core.Json;
using DaveTheMonitor.Core.Plugin;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components
{
    public sealed class ComponentCollection
    {
        public IEnumerable<Component> Components => _components;
        private Dictionary<string, Component> _componentsDictionary;
        private Dictionary<Type, Component> _componentsTypeDictionary;
        private List<Component> _components;

        public static ComponentCollection ReadFromJson(ModVersion version, JsonElement element, string componentUsage, JsonSerializerOptions options = null)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("ComponentCollection must be an object.");
            }

            ComponentCollection components = new ComponentCollection();

            foreach (JsonProperty property in element.EnumerateObject())
            {
                string alias = property.Name;
                ComponentDefinition? definition = Component.GetComponentFromAlias(alias, componentUsage);
                if (definition.HasValue)
                {
                    object component;
                    if (definition.Value.IComponentDeserializable)
                    {
                        IComponentDeserializable deserializable = (IComponentDeserializable)Activator.CreateInstance(definition.Value.Type);
                        Type deserializeType = deserializable.GetDeserializeType(version);
                        if (deserializeType == null || deserializeType == definition.Value.Type)
                        {
                            deserializable = (IComponentDeserializable)property.Value.Deserialize(deserializeType, options);
                        }
                        else if (deserializeType == typeof(JsonElement))
                        {
                            deserializable.ReadFrom(version, property.Value);
                        }
                        else
                        {
                            deserializable.ReadFrom(version, property.Value.Deserialize(deserializeType, options));
                        }
                        component = deserializable;
                    }
                    else
                    {
                        component = JsonSerializer.Deserialize(property.Value, definition.Value.Type, options);
                    }

                    components.AddComponent((Component)component);
                }
#if DEBUG
                else
                {
                    CorePlugin.Warn($"Invalid component: {alias}");
                }
#endif
            }

            return components;
        }

        public void AddComponent(Component component)
        {
            _componentsDictionary.Add(component.ComponentId, component);
            _componentsTypeDictionary.Add(component.GetType(), component);
            _components.Add(component);
        }

        public bool RemoveComponent(Component component)
        {
            if (_components.Remove(component))
            {
                _componentsDictionary.Remove(component.ComponentId);
                return true;
            }
            return false;
        }

        public bool RemoveComponent(string id)
        {
            if (_componentsDictionary.Remove(id, out Component component))
            {
                _components.Remove(component);
                return true;
            }
            return false;
        }

        public bool HasComponent(string id)
        {
            return _componentsDictionary.ContainsKey(id);
        }

        public bool HasComponent<T>() where T : Component
        {
            return _componentsTypeDictionary.ContainsKey(typeof(T));
        }

        public bool HasAllComponents(params string[] ids)
        {
            foreach (string id in ids)
            {
                if (!HasComponent(id))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAllComponents<T1, T2>()
            where T1 : Component
            where T2 : Component
        {
            return HasComponent<T1>() && HasComponent<T2>();
        }

        public bool HasAllComponents<T1, T2, T3>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
        {
            return HasComponent<T1>() && HasComponent<T2>() && HasComponent<T3>();
        }

        public bool HasAllComponents<T1, T2, T3, T4>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
        {
            return
                HasComponent<T1>() &&
                HasComponent<T2>() &&
                HasComponent<T3>() &&
                HasComponent<T4>();
        }

        public bool HasAllComponents<T1, T2, T3, T4, T5>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
        {
            return
                HasComponent<T1>() &&
                HasComponent<T2>() &&
                HasComponent<T3>() &&
                HasComponent<T4>() &&
                HasComponent<T5>();
        }

        public bool HasAllComponents<T1, T2, T3, T4, T5, T6>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
            where T6 : Component
        {
            return
                HasComponent<T1>() &&
                HasComponent<T2>() &&
                HasComponent<T3>() &&
                HasComponent<T4>() &&
                HasComponent<T5>() &&
                HasComponent<T6>();
        }

        public bool HasAllComponents<T1, T2, T3, T4, T5, T6, T7>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
            where T6 : Component
            where T7 : Component
        {
            return
                HasComponent<T1>() &&
                HasComponent<T2>() &&
                HasComponent<T3>() &&
                HasComponent<T4>() &&
                HasComponent<T5>() &&
                HasComponent<T6>() &&
                HasComponent<T7>();
        }

        public bool HasAllComponents<T1, T2, T3, T4, T5, T6, T7, T8>()
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
            where T6 : Component
            where T7 : Component
            where T8 : Component
        {
            return
                HasComponent<T1>() &&
                HasComponent<T2>() &&
                HasComponent<T3>() &&
                HasComponent<T4>() &&
                HasComponent<T5>() &&
                HasComponent<T6>() &&
                HasComponent<T7>() &&
                HasComponent<T8>();
        }

        public Component GetComponent(string id)
        {
            _componentsDictionary.TryGetValue(id, out Component component);
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            _componentsTypeDictionary.TryGetValue(typeof(T), out Component component);
            return (T)component;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            if (_componentsTypeDictionary.TryGetValue(typeof(T), out Component result))
            {
                component = (T)result;
                return true;
            }
            component = null;
            return false;
        }

        public void CopyTo(ComponentCollection other, bool replace)
        {
            foreach (Component component in _components)
            {
                Component otherComponent = other.GetComponent(component.ComponentId);
                if (otherComponent != null)
                {
                    if (!replace)
                    {
                        continue;
                    }

                    otherComponent.ReplaceWith(component);
                    continue;
                }

                other.AddComponent(component);
            }
        }

        public void SetComponentDefaults()
        {
            foreach (Component component in _components)
            {
                component.SetDefaults();
            }
        }

        public ComponentCollection()
        {
            _componentsDictionary = new Dictionary<string, Component>();
            _componentsTypeDictionary = new Dictionary<Type, Component>();
            _components = new List<Component>();
        }

        public ComponentCollection(IEnumerable<Component> components)
        {
            _componentsDictionary = new Dictionary<string, Component>();
            _componentsTypeDictionary = new Dictionary<Type, Component>();
            foreach (Component component in components)
            {
                _componentsDictionary.Add(component.ComponentId, component);
                _componentsTypeDictionary.Add(component.GetType(), component);
            }
            _components = new List<Component>(components);
        }
    }
}
