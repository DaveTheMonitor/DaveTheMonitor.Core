using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Effects.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests if this actor has a specified <see cref="ActorEffect"/>.
    /// </summary>
    [JsonCondition("Core.HasEffect")]
    public sealed class HasEffectCondition : BooleanCondition
    {
        /// <summary>
        /// The effect to test for.
        /// </summary>
        public string Effect { get; private set; }

        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            return actor.Effects().HasEffect(Effect) == Value;
        }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            Effect = DeserializationHelper.GetStringProperty(element, "Effect") ?? "";
        }
    }
}
