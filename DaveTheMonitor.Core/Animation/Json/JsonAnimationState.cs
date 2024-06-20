using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DaveTheMonitor.Core.Animation.Json
{
    /// <summary>
    /// An animation state for a <see cref="JsonAnimationController"/>.
    /// </summary>
    public sealed class JsonAnimationState
    {
        /// <summary>
        /// The ID of this state.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The animation this state should play.
        /// </summary>
        public string Animation { get; private set; }

        /// <summary>
        /// The loop type of this state.
        /// </summary>
        public AnimationLoopType LoopType { get; private set; }

        /// <summary>
        /// All transitions in this state.
        /// </summary>
        public IEnumerable<JsonAnimationTransition> Transitions => _transitions;
        private JsonAnimationTransition[] _transitions;
        private Dictionary<ActorModel, ActorAnimation> _animationCache;

        internal static JsonAnimationState FromJson(string name, JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("AnimationController state must be an object.");
            }

            if (!element.TryGetProperty("Animation", out JsonElement animationElement))
            {
                throw new InvalidCoreJsonException("AnimationController state Animation must be specified.");
            }

            if (animationElement.ValueKind != JsonValueKind.String)
            {
                throw new InvalidCoreJsonException("AnimationController state Animation must be a string.");
            }

            string animation = animationElement.GetString();
            if (string.IsNullOrEmpty(animation))
            {
                throw new InvalidCoreJsonException("AnimationController state Animation must not be empty.");
            }

            AnimationLoopType loopType = AnimationLoopType.None;
            if (element.TryGetProperty("LoopType", out JsonElement loopTypeElement))
            {
                if (loopTypeElement.ValueKind != JsonValueKind.String || !Enum.TryParse(loopTypeElement.GetString(), true, out loopType))
                {
                    throw new InvalidCoreJsonException("AnimationController state LoopType must be a valid loop type.");
                }
            }

            JsonAnimationState state = new JsonAnimationState(name, animation, loopType);

            if (element.TryGetProperty("Transitions", out JsonElement transitionsElement))
            {
                if (transitionsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidCoreJsonException("AnimationController state Transitions must be an array");
                }

                foreach (JsonElement transitionElement in transitionsElement.EnumerateArray())
                {
                    JsonAnimationTransition transition = JsonAnimationTransition.FromJson(transitionElement);
                    state.AddTransition(transition);
                }
            }

            return state;
        }

        internal AnimationState ToAnimationState(ActorModel model, ICoreMod mod)
        {
            JsonActorAnimation jsonAnimation = mod.ModManager.LoadActorAnimation(mod, Animation);
            if (jsonAnimation == null)
            {
                throw new InvalidCoreJsonException($"AnimationController state animation {Animation} does not exist.");
            }

            if (!_animationCache.TryGetValue(model, out ActorAnimation animation))
            {
                animation = jsonAnimation.ToActorAnimation(model);
                _animationCache.Add(model, animation);
            }

            AnimationState state = new AnimationState(Id, animation, LoopType);
            foreach (JsonAnimationTransition transition in _transitions)
            {
                state.AddTransition(transition.Test);
            }

            return state;
        }

        /// <summary>
        /// Adds a transition to this state.
        /// </summary>
        /// <param name="transition">The transition to add.</param>
        /// <returns>this</returns>
        public JsonAnimationState AddTransition(JsonAnimationTransition transition)
        {
            int index = _transitions.Length;
            Array.Resize(ref _transitions, _transitions.Length + 1);
            _transitions[index] = transition;
            return this;
        }

        /// <summary>
        /// Creates a new, empty <see cref="JsonAnimationState"/>.
        /// </summary>
        /// <param name="id">The ID of this state.</param>
        /// <param name="animation">The animation this state should play.</param>
        /// <param name="loopType">The loop type of this state.</param>
        public JsonAnimationState(string id, string animation, AnimationLoopType loopType)
        {
            Id = id;
            Animation = animation;
            LoopType = loopType;
            _transitions = Array.Empty<JsonAnimationTransition>();
            _animationCache = new Dictionary<ActorModel, ActorAnimation>();
        }
    }
}
