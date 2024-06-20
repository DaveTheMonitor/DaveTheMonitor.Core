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
    /// A <see cref="JsonCondition"/> that always returns true.
    /// </summary>
    [JsonCondition("Core.True")]
    public sealed class TrueCondition : JsonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor) => true;
    }
}
