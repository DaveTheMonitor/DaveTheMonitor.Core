using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that compares an actor property to a static <see cref="float"/> value.
    /// </summary>
    public abstract class SingleComparisonCondition : JsonCondition
    {
        /// <summary>
        /// The operator to use for comparison.
        /// </summary>
        public JsonConditionOperator Operator { get; private set; }

        /// <summary>
        /// The value to compare against.
        /// </summary>
        public float Value { get; private set; }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            Operator = GetOperator(element, "Operator", JsonConditionOperator.Equal);
            Value = DeserializationHelper.GetSingleProperty(element, "Value") ?? 0;
        }
    }
}
