using DaveTheMonitor.Core.API;
using System;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests actor's current vertical speed against a value.
    /// </summary>
    [JsonCondition("Core.VerticalSpeed")]
    public sealed class VerticalSpeedCondition : SingleComparisonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return Compare(Math.Abs(actor.Velocity.Y), Value, Operator);
        }
    }
}
