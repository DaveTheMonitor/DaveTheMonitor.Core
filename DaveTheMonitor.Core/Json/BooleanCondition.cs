using DaveTheMonitor.Core.Helpers;
using System.Text.Json;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that compares an actor property to a static <see cref="bool"/> value.
    /// </summary>
    public abstract class BooleanCondition : JsonCondition
    {
        /// <summary>
        /// The value to compare against.
        /// </summary>
        public bool Value { get; private set; }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            Value = DeserializationHelper.GetBoolProperty(element, "Value") ?? true;
        }
    }
}
