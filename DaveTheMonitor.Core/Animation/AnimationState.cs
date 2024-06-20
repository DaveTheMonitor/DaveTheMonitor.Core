using DaveTheMonitor.Core.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// An animation state for an <see cref="AnimationController"/>.
    /// </summary>
    [DebuggerDisplay("Id = {Id}")]
    public sealed class AnimationState
    {
        /// <summary>
        /// The animation for this state.
        /// </summary>
        public ActorAnimation Animation { get; private set; }

        /// <summary>
        /// The ID of this state.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The loop type of this state.
        /// </summary>
        public AnimationLoopType LoopType { get; private set; }
        private Func<ICoreActor, string>[] _transitions;
        private Dictionary<string, List<Action<ICoreActor>>> _eventListeners;

        /// <summary>
        /// Returns the ID of the state this state should transition to.
        /// </summary>
        /// <param name="actor">The actor in this state.</param>
        /// <returns>The ID of the state this state should transition to, or null if it should not transition.</returns>
        public string Transition(ICoreActor actor)
        {
            foreach (Func<ICoreActor, string> transition in _transitions)
            {
                string state = transition(actor);
                if (state != null)
                {
                    return state;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a transition to this state.
        /// </summary>
        /// <param name="transition">The transition function to add.</param>
        /// <returns>this</returns>
        public AnimationState AddTransition(Func<ICoreActor, string> transition)
        {
            int index = _transitions.Length;
            Array.Resize(ref _transitions, _transitions.Length + 1);
            _transitions[index] = transition;
            return this;
        }

        /// <summary>
        /// Adds an event listener for the specified event.
        /// </summary>
        /// <param name="eventName">The event to listen to.</param>
        /// <param name="listener">The listener action.</param>
        /// <returns>this</returns>
        public AnimationState AddEventListener(string eventName, Action<ICoreActor> listener)
        {
            _eventListeners ??= new Dictionary<string, List<Action<ICoreActor>>>();

            if (!_eventListeners.TryGetValue(eventName, out List<Action<ICoreActor>> list))
            {
                list = new List<Action<ICoreActor>>();
                _eventListeners.Add(eventName, list);
            }
            list.Add(listener);
            return this;
        }

        /// <summary>
        /// Triggers all event listeners for the events between <paramref name="minTime"/> and <paramref name="maxTime"/>.
        /// </summary>
        /// <param name="actor">The actor currently in this state.</param>
        /// <param name="minTime">The minimum time.</param>
        /// <param name="maxTime">The maximum time.</param>
        public void TriggerAllEvents(ICoreActor actor, float minTime, float maxTime)
        {
            if (_eventListeners == null)
            {
                return;
            }

            List<string> events = Animation.GetEvents(minTime, maxTime);
            if (events == null)
            {
                return;
            }

            foreach (string @event in events)
            {
                if (_eventListeners.TryGetValue(@event, out List<Action<ICoreActor>> listeners))
                {
                    foreach (Action<ICoreActor> listener in listeners)
                    {
                        listener(actor);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new animation state.
        /// </summary>
        /// <param name="id">The ID of this state.</param>
        /// <param name="animation">The animation this state should play.</param>
        /// <param name="loopType">The loop type of this state.</param>
        public AnimationState(string id, ActorAnimation animation, AnimationLoopType loopType)
        {
            Id = id;
            Animation = animation;
            LoopType = loopType;
            _transitions = Array.Empty<Func<ICoreActor, string>>();
            _eventListeners = null;
        }
    }
}
