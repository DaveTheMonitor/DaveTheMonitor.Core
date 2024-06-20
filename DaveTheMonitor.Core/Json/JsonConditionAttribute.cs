using System;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// Specifies that a type should be automatically added as a <see cref="JsonCondition"/> when <see cref="JsonCondition.RegisterConditionTypes(System.Reflection.Assembly)"/> is called.
    /// </summary>
    public class JsonConditionAttribute : Attribute
    {
        /// <summary>
        /// The ID of the condition.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Creates a new <see cref="JsonConditionAttribute"/> with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the attribute.</param>
        public JsonConditionAttribute(string id)
        {
            Id = id;
        }
    }
}
