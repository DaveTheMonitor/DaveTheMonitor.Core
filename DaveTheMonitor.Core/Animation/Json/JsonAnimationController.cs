using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DaveTheMonitor.Core.Animation.Json
{
    /// <summary>
    /// An controller parsed from a Json string. This should not be used for animation, convert it to an <see cref="AnimationController"/> first.
    /// </summary>
    public sealed class JsonAnimationController
    {
        /// <summary>
        /// The default state of this <see cref="JsonAnimationController"/>.
        /// </summary>
        public string DefaultState { get; private set; }

        /// <summary>
        /// All states of this <see cref="JsonAnimationController"/>.
        /// </summary>
        public IEnumerable<JsonAnimationState> States => _states;
        private JsonAnimationState[] _states;
        private ICoreMod _mod;
        private Dictionary<ActorModel, AnimationState[]> _statesCache;

        /// <summary>
        /// Creates a new <see cref="JsonAnimationController"/> from a Json string.
        /// </summary>
        /// <param name="json">The Json string to parse.</param>
        /// <param name="mod">The default fallback mod for asset loading.</param>
        /// <returns>A new <see cref="JsonAnimationController"/> from the Json string.</returns>
        public static JsonAnimationController FromJson(string json, ICoreMod mod)
        {
            JsonDocument doc = JsonDocument.Parse(json, DeserializationHelper.DocumentOptionsTrailingCommasSkipComments);

            if (!doc.RootElement.TryGetProperty("DefaultState", out JsonElement defaultStateElement))
            {
                throw new InvalidCoreJsonException("AnimationController DefaultState must be specified.");
            }

            if (defaultStateElement.ValueKind != JsonValueKind.String)
            {
                throw new InvalidCoreJsonException("AnimationController DefaultState must be a string.");
            }

            string defaultState = defaultStateElement.GetString();
            if (string.IsNullOrWhiteSpace(defaultState))
            {
                throw new InvalidCoreJsonException("AnimationController DefaultState must not be empty.");
            }

            if (!doc.RootElement.TryGetProperty("States", out JsonElement partsElement))
            {
                throw new InvalidCoreJsonException("AnimationController must define at least one state.");
            }

            if (partsElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("AnimationController States must be an object.");
            }

            JsonAnimationController controller = new JsonAnimationController(defaultState, mod);

            foreach (JsonProperty property in partsElement.EnumerateObject())
            {
                controller.AddState(JsonAnimationState.FromJson(property.Name, property.Value));
            }

            if (!controller._states.Any(s => s.Id == defaultState))
            {
                throw new InvalidCoreJsonException($"AnimationController DefaultState {defaultState} not found.");
            }

            return controller;
        }

        /// <summary>
        /// Adds a state to this <see cref="JsonAnimationController"/>.
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <returns>this</returns>
        public JsonAnimationController AddState(JsonAnimationState state)
        {
            int index = _states.Length;
            Array.Resize(ref _states, _states.Length + 1);
            _states[index] = state;
            return this;
        }

        /// <summary>
        /// Creates a new <see cref="AnimationController"/> from this <see cref="JsonAnimationController"/>.
        /// </summary>
        /// <param name="actor">The actor this controller belongs to.</param>
        /// <returns>A new <see cref="AnimationController"/> from this <see cref="JsonAnimationController"/>.</returns>
        public AnimationController ToAnimationController(ICoreActor actor)
        {
            if (!_statesCache.TryGetValue(actor.Model, out AnimationState[] states))
            {
                states = new AnimationState[_states.Length];
                for (int i = 0; i < _states.Length; i++)
                {
                    states[i] = _states[i].ToAnimationState(actor.Model, _mod);
                }
                _statesCache.Add(actor.Model, states);
            }

            return new AnimationController(actor, states);
        }

        /// <summary>
        /// Creates a new, empty <see cref="JsonAnimationController"/>.
        /// </summary>
        /// <param name="defaultState">The default state of this animation controller.</param>
        /// <param name="mod">The default fallback mod for asset loading.</param>
        public JsonAnimationController(string defaultState, ICoreMod mod)
        {
            DefaultState = defaultState;
            _states = Array.Empty<JsonAnimationState>();
            _mod = mod;
            _statesCache = new Dictionary<ActorModel, AnimationState[]>();
        }
    }
}
