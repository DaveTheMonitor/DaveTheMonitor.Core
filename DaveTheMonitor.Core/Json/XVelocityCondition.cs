using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests actor's current X velocity against a value.
    /// </summary>
    [JsonCondition("Core.XVelocity")]
    public sealed class XVelocityCondition : SingleComparisonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return Compare(actor.Velocity.X, Value, Operator);
        }
    }
}
