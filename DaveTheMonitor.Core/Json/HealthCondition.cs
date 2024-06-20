using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests actor's current health against a value.
    /// </summary>
    [JsonCondition("Core.Health")]
    public sealed class HealthCondition : SingleComparisonCondition
    {
        /// <summary>
        /// If true, the health will be compared as a percentage.
        /// </summary>
        public bool Percent { get; private set; }

        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            if (Percent)
            {
                return Compare(actor.Health / actor.MaxHealth, Value, Operator);
            }
            else
            {
                return Compare(actor.Health, Value, Operator);
            }
        }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            Percent = DeserializationHelper.GetBoolProperty(element, "Percent") ?? false;
        }
    }
}
