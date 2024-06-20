using DaveTheMonitor.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Components
{
    public abstract class Component
    {
        public abstract string ComponentId { get; }
        private static Dictionary<string, ComponentDefinition> _components;

        internal static void RegisterComponents(Assembly assembly)
        {
#if DEBUG
            CorePlugin.Log($"Registering components from {assembly.FullName}");
#endif
            _components ??= new Dictionary<string, ComponentDefinition>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<ComponentAttribute>() != null)
                {
                    ComponentDefinition definition = ComponentDefinition.FromType(type);
                    if (!_components.TryAdd(definition.Id, definition))
                    {
                        throw new ComponentException(type, $"A component with the ID {definition.Id} already exists.");
                    }
                }
            }
        }

        public static ComponentDefinition? GetComponent(string id)
        {
            if (_components.TryGetValue(id, out ComponentDefinition def))
            {
                return def;
            }
            return null;
        }

        public static ComponentDefinition? GetComponentFromAlias(string alias, string usage)
        {
            foreach (ComponentDefinition definition in  _components.Values)
            {
                if (definition.Alias == alias && ComponentDefinition.ValidUsage(definition, usage))
                {
                    return definition;
                }
            }
            return null;
        }

        public abstract void ReplaceWith(Component replacement);
        public abstract void SetDefaults();
    }
}
