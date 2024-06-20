using DaveTheMonitor.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests if all of its children evaluate to true.
    /// </summary>
    [JsonCondition("Core.All")]
    public sealed class AllCondition : JsonCondition
    {
        /// <summary>
        /// The children conditions of this condition.
        /// </summary>
        public IEnumerable<JsonCondition> Conditions => _conditions;
        private JsonCondition[] _conditions;

        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            foreach (JsonCondition condition in _conditions)
            {
                if (!condition.Evaluate(actor))
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            if (!element.TryGetProperty("Conditions", out JsonElement conditionsElement))
            {
                throw new InvalidCoreJsonException("JsonCondition Any must contain at least one condition.");
            }

            if (conditionsElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidCoreJsonException("JsonCondition Any must be an array.");
            }

            List<JsonCondition> conditions = new List<JsonCondition>();
            foreach (JsonElement condition in conditionsElement.EnumerateArray())
            {
                conditions.Add(FromJson(condition));
            }
            _conditions = conditions.ToArray();
        }
    }
}
