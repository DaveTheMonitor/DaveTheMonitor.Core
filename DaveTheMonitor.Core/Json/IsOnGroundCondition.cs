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
    /// A <see cref="JsonCondition"/> that tests if the actor is currently on the ground with a very small amount of coyote time (primarily to avoid a TM bug, see <see cref="ICoreActor.Grounded"/>).
    /// </summary>
    [JsonCondition("Core.IsOnGround")]
    public sealed class IsOnGroundCondition : BooleanCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return actor.IsOnGround(0.0167f) == Value;
        }
    }
}
