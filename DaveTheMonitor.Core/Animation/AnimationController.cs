using DaveTheMonitor.Core.API;
using StudioForge.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A state machine for <see cref="ActorAnimation"/>s.
    /// </summary>
    public sealed class AnimationController
    {
        /// <summary>
        /// The current time through the animation.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// The total time the animation has been playing.
        /// </summary>
        public float TotalTime { get; private set; }

        /// <summary>
        /// The current animation state.
        /// </summary>
        public AnimationState CurrentState { get; private set; }

        /// <summary>
        /// True if the current animation has finished playing at least once.
        /// </summary>
        public bool Finished { get; private set; }
        private ActorPartSnapshot[] _snapshot;
        private ActorPartSnapshot _modelSnapshot;
        private AnimationState[] _states;
        private bool _shouldPlay;
        private ICoreActor _actor;

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void Update()
        {
            if (CurrentState.Animation.Length > 0 && _shouldPlay)
            {
                ActorAnimation animation = CurrentState.Animation;
                float prevTime = CurrentTime;
                CurrentTime += Services.ElapsedTime;
                TotalTime += Services.ElapsedTime;

                CurrentState.TriggerAllEvents(_actor, prevTime, CurrentTime);
                if (CurrentTime >= animation.Length)
                {
                    Finished = true;
                    switch (CurrentState.LoopType)
                    {
                        case AnimationLoopType.Loop:
                        {
                            CurrentTime -= animation.Length;
                            break;
                        }
                        default:
                        {
                            CurrentTime = animation.Length;
                            _shouldPlay = false;
                            break;
                        }
                    }
                }
            }

            string state = CurrentState.Transition(_actor);
            if (state != null)
            {
                PlayAnimation(state);
            }
        }

        /// <summary>
        /// Gets the snapshot taken when this animation controller last changed states.
        /// </summary>
        /// <param name="snapshot">The snapshot. This may be null.</param>
        /// <param name="modelSnapshot">The model snapshot.</param>
        /// <returns>True if this animation controller has a snapshot, otherwise false.</returns>
        public bool TryGetPreviousStateSnapshot(out ActorPartSnapshot[] snapshot, out ActorPartSnapshot modelSnapshot)
        {
            snapshot = _snapshot;
            modelSnapshot = _modelSnapshot;
            return _snapshot != null;
        }

        /// <summary>
        /// Immediately plays the animation with the specified ID, canceling the current animation.
        /// </summary>
        /// <param name="id">The ID of the animation to play.</param>
        /// <returns>True if the animation could be played, otherwise false.</returns>
        public bool PlayAnimation(string id)
        {
            foreach (AnimationState state in _states)
            {
                if (state.Id == id)
                {
                    ChangeState(state);
                    return true;
                }
            }
            return false;
        }

        private void ChangeState(AnimationState state)
        {
            if (CurrentState != null)
            {
                if (_snapshot != null && TotalTime < 0.25f)
                {
                    List<ActorPartSnapshot> snapshot = new List<ActorPartSnapshot>(_snapshot.Length);
                    CurrentState.Animation.Snapshot(_actor, CurrentTime, _snapshot, _modelSnapshot, 0.25f, snapshot, out _modelSnapshot);
                    _snapshot = snapshot.ToArray();
                }
                else
                {
                    List<ActorPartSnapshot> snapshot = new List<ActorPartSnapshot>();
                    CurrentState.Animation.Snapshot(_actor, CurrentTime, null, default, 0.25f, snapshot, out _modelSnapshot);
                    _snapshot = snapshot.ToArray();
                }
            }
            CurrentState = state;
            CurrentTime = 0;
            TotalTime = 0;
            _shouldPlay = true;
            Finished = false;
        }

        /// <summary>
        /// Adds an event listener for all states.
        /// </summary>
        /// <param name="eventName">The event to listen to.</param>
        /// <param name="listener">The listener.</param>
        /// <returns>this</returns>
        public AnimationController AddEventListener(string eventName, Action<ICoreActor> listener)
        {
            foreach (AnimationState state in _states)
            {
                state.AddEventListener(eventName, listener);
            }
            return this;
        }

        /// <summary>
        /// Adds an event listener for the specified state.
        /// </summary>
        /// <param name="eventName">The event to listen to.</param>
        /// <param name="stateName">The state to attach this event listener to.</param>
        /// <param name="listener">The listener.</param>
        /// <returns>this</returns>
        public AnimationController AddEventListener(string eventName, string stateName, Action<ICoreActor> listener)
        {
            GetState(stateName)?.AddEventListener(eventName, listener);
            return this;
        }

        private AnimationState GetState(string name)
        {
            foreach (AnimationState state in _states)
            {
                if (state.Id == name)
                {
                    return state;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new animation controller for the specified actor.
        /// </summary>
        /// <param name="actor">The actor to create the animation controller for.</param>
        /// <param name="states">The states this animation controller has.</param>
        public AnimationController(ICoreActor actor, AnimationState[] states)
        {
            _actor = actor;
            _states = new AnimationState[states.Length];
            Array.Copy(states, _states, states.Length);
        }
    }
}
