using DaveTheMonitor.Core.API;

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
