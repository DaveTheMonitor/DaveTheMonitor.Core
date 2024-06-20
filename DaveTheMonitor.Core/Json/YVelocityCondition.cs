using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests actor's current Y velocity against a value.
    /// </summary>
    [JsonCondition("Core.YVelocity")]
    public sealed class YVelocityCondition : SingleComparisonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return Compare(actor.Velocity.Y, Value, Operator);
        }
    }
}
