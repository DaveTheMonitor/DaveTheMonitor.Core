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
