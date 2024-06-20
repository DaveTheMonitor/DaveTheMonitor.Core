using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests if the actor's current animation has fully played at least once.
    /// </summary>
    [JsonCondition("Core.AnimationFinished")]
    public sealed class AnimationFinishedCondition : BooleanCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            if (actor.Animation == null)
            {
                return true;
            }

            return actor.Animation.Finished == Value;
        }
    }
}
