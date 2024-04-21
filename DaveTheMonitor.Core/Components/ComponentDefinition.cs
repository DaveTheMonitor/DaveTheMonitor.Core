using System;
using System.Reflection;

namespace DaveTheMonitor.Core.Components
{
    public struct ComponentDefinition
    {
        public string Id { get; private set; }
        public string Alias { get; private set; }
        public Type Type { get; private set; }
        public string[] Usage { get; private set; }
        public bool IComponentDeserializable { get; private set; }

        public static ComponentDefinition FromType(Type type)
        {
            ComponentAttribute attribute = type.GetCustomAttribute<ComponentAttribute>();
            if (!type.IsAssignableTo(typeof(Component)))
            {
                throw new ComponentException(type, "Component type must inherit Component.");
            }
            else if (attribute == null)
            {
                throw new ComponentException(type, "Component type must specify the Component attribute.");
            }
            else if (string.IsNullOrWhiteSpace(attribute.Id))
            {
                throw new ComponentException(type, "Component Id must not be null or empty.");
            }
            else if (string.IsNullOrWhiteSpace(attribute.Alias))
            {
                throw new ComponentException(type, "Component Name must not be null or empty.");
            }

            return new ComponentDefinition(type, attribute);
        }

        public static bool ValidUsage(ComponentDefinition definition, string usage)
        {
            foreach (string defUsage in definition.Usage)
            {
                if (defUsage == usage || defUsage == "Any")
                {
                    return true;
                }
            }
            return false;
        }

        private ComponentDefinition(Type type, ComponentAttribute attribute)
        {
            Type = type;
            Id = attribute.Id;
            Alias = attribute.Alias;
            Usage = attribute.Usage?.Length > 0 ? attribute.Usage : new string[1] { "Any" };
            IComponentDeserializable = type.IsAssignableTo(typeof(IComponentDeserializable));
        }
    }
}
