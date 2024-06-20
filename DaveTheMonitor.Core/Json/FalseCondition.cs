using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that always returns false. This is used in place of conditions that don't exist.
    /// </summary>
    [JsonCondition("Core.False")]
    public sealed class FalseCondition : JsonCondition
    {
        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor) => false;
    }
}
