using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests actor's current horizontal speed against a value.
    /// </summary>
    [JsonCondition("Core.HorizontalSpeed")]
    public sealed class HorizontalSpeedCondition : SingleComparisonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            Vector3 velocity = actor.Velocity;
            velocity.Z = 0;
            float speed = velocity.Length();
            return Compare(speed, Value, Operator);
        }
    }
}
