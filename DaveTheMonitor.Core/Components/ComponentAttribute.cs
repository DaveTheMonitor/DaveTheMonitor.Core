using System;

namespace DaveTheMonitor.Core.Components
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ComponentAttribute : Attribute
    {
        public string Id { get; set; }
        public string Alias { get; set; }
        /// <summary>
        /// Default usage types:
        /// Any,
        /// Item,
        /// Actor,
        /// Effect,
        /// Particle
        /// </summary>
        public string[] Usage { get; set; }

        public ComponentAttribute(string id, string name, params string[] usage)
        {
            Id = id;
            Alias = name;
            Usage = usage;
        }
    }
}
