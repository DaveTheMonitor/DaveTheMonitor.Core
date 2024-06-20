﻿using DaveTheMonitor.Core.API;
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
    /// A <see cref="JsonCondition"/> that tests actor's current Z velocity against a value.
    /// </summary>
    [JsonCondition("Core.ZVelocity")]
    public sealed class ZVelocityCondition : SingleComparisonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return Compare(actor.Velocity.Z, Value, Operator);
        }
    }
}
