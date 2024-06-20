using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation.Json
{
    /// <summary>
    /// An animation transition for a <see cref="JsonAnimationState"/>.
    /// </summary>
    public sealed class JsonAnimationTransition
    {
        /// <summary>
        /// The target state of this transition.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// The condition for this transition to trigger.
        /// </summary>
        public JsonCondition Condition { get; private set; }

        internal static JsonAnimationTransition FromJson(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("AnimationController Transition must be an object.");
            }

            if (!element.TryGetProperty("State", out JsonElement stateElement))
            {
                throw new InvalidCoreJsonException("AnimationController Transition state must be specified.");
            }

            if (stateElement.ValueKind != JsonValueKind.String)
            {
                throw new InvalidCoreJsonException("AnimationController Transition state must be a string.");
            }

            string state = stateElement.GetString();
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new InvalidCoreJsonException("AnimationController Transition state must not be empty.");
            }

            if (!element.TryGetProperty("Condition", out JsonElement conditionElement))
            {
                throw new InvalidCoreJsonException("AnimationController Transition must have a condition.");
            }

            JsonCondition condition = JsonCondition.FromJson(conditionElement);

            return new JsonAnimationTransition(state, condition);
        }

        /// <summary>
        /// Tests this <see cref="JsonAnimationTransition"/>.
        /// </summary>
        /// <param name="actor">The actor playing the animation.</param>
        /// <returns>The target state, or null if the state should not transition.</returns>
        public string Test(ICoreActor actor)
        {
            return Condition.Evaluate(actor) ? State : null;
        }

        /// <summary>
        /// Creates a new <see cref="JsonAnimationTransition"/>.
        /// </summary>
        /// <param name="state">The state to transition to.</param>
        /// <param name="condition">The condition for the transition.</param>
        public JsonAnimationTransition(string state, JsonCondition condition)
        {
            State = state;
            Condition = condition;
        }
    }
}
