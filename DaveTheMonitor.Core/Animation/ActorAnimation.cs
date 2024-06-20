using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// An animation for an actor.
    /// </summary>
    [DebuggerDisplay("Length = {Length}, Parts = {_partKeyframes.GetLength(1)}")]
    public sealed class ActorAnimation
    {
        /// <summary>
        /// The total length of this animation.
        /// </summary>
        public float Length { get; private set; }
        internal const int _typeCount = 2;
        private KeyframeCollection<ActorPartKeyframe>[,] _partKeyframes;
        private KeyframeCollection<ActorPartKeyframe>[] _modelKeyframes;
        private KeyframeCollection<string> _events;

        /// <summary>
        /// Creates a new animation from a Json string.
        /// </summary>
        /// <param name="model">The model to create the animation for.</param>
        /// <param name="json">The json string to parse.</param>
        /// <returns>A new animation from the Json string.</returns>
        public static ActorAnimation FromJson(ActorModel model, string json)
        {
            JsonActorAnimation jsonAnimation = JsonActorAnimation.FromJson(json);
            return jsonAnimation.ToActorAnimation(model);
        }

        /// <summary>
        /// Snapshots this animation's current state into <paramref name="result"/>.
        /// </summary>
        /// <param name="actor">The actor that this animation belongs to.</param>
        /// <param name="time">The current time through the animation.</param>
        /// <param name="result">The result of the snapshot.</param>
        /// <param name="modelSnapshot">The result of the snapshot for the model.</param>
        public void Snapshot(ICoreActor actor, float time, List<ActorPartSnapshot> result, out ActorPartSnapshot modelSnapshot)
        {
            Snapshot(actor, time, null, default, 0.5f, result, out modelSnapshot);
        }

        /// <summary>
        /// Snapshots this animation's current state into <paramref name="result"/>.
        /// </summary>
        /// <param name="actor">The actor that this animation belongs to.</param>
        /// <param name="time">The current time through the animation.</param>
        /// <param name="result">The result of the snapshot.</param>
        /// <param name="modelSnapshot">The result of the snapshot for the model.</param>
        public void Snapshot(ICoreActor actor, float time, ActorPartSnapshot[] result, out ActorPartSnapshot modelSnapshot)
        {
            Snapshot(actor, time, null, default, 0.5f, result, out modelSnapshot);
        }

        /// <summary>
        /// Snapshots this animation's current state into <paramref name="result"/>.
        /// </summary>
        /// <param name="actor">The actor that this animation belongs to.</param>
        /// <param name="time">The current time through the animation.</param>
        /// <param name="currentSnapshot">The current snapshot, if any. The animation will transition from this snapshot to the new snapshot over <paramref name="transitionTime"/>.</param>
        /// <param name="currentModelSnapshot">The current snapshot of the model, if any. Use default if there is no current snapshot.</param>
        /// <param name="transitionTime">The time the transition from the current state to the new state lasts, in seconds.</param>
        /// <param name="result">The result of the snapshot.</param>
        /// <param name="modelSnapshot">The result of the snapshot for the model.</param>
        public void Snapshot(ICoreActor actor, float time, ActorPartSnapshot[] currentSnapshot, ActorPartSnapshot currentModelSnapshot, float transitionTime, List<ActorPartSnapshot> result, out ActorPartSnapshot modelSnapshot)
        {
            result.Clear();
            result.EnsureCapacity(actor.Model.TotalParts);
            
            // This is required for CollectionsMarshal.AsSpan to return the
            // correct length
            // There's a faster way to do this in .NET 8, but we're in .NET 7
            ActorPartSnapshot defaultSnapshot = default;
            for (int i = 0; i < actor.Model.TotalParts; i++)
            {
                result.Add(defaultSnapshot);
            }
            Span<ActorPartSnapshot> resultSpan = CollectionsMarshal.AsSpan(result);
            Snapshot(actor, time, currentSnapshot.AsSpan(), currentModelSnapshot, transitionTime, resultSpan, out modelSnapshot);
        }

        /// <summary>
        /// Snapshots this animation's current state into <paramref name="result"/>.
        /// </summary>
        /// <param name="actor">The actor that this animation belongs to.</param>
        /// <param name="time">The current time through the animation.</param>
        /// <param name="currentSnapshot">The current snapshot, if any.</param>
        /// <param name="currentModelSnapshot">The current snapshot of the model, if any. Use default if there is no current snapshot.</param>
        /// <param name="transitionTime">The time the transition from the current state to the new state lasts, in seconds.</param>
        /// <param name="result">The result of the snapshot.</param>
        /// <param name="modelSnapshot">The result of the snapshot for the model.</param>
        public void Snapshot(ICoreActor actor, float time, ActorPartSnapshot[] currentSnapshot, ActorPartSnapshot currentModelSnapshot, float transitionTime, ActorPartSnapshot[] result, out ActorPartSnapshot modelSnapshot)
        {
            Snapshot(actor, time, currentSnapshot.AsSpan(), currentModelSnapshot, transitionTime, result.AsSpan(), out modelSnapshot);
        }

        /// <summary>
        /// Snapshots this animation's current state into <paramref name="result"/>.
        /// </summary>
        /// <param name="actor">The actor that this animation belongs to.</param>
        /// <param name="time">The current time through the animation.</param>
        /// <param name="currentSnapshot">The current snapshot, if any.</param>
        /// <param name="currentModelSnapshot">The current snapshot of the model, if any. Use default if there is no current snapshot.</param>
        /// <param name="transitionTime">The time the transition from the current state to the new state lasts, in seconds.</param>
        /// <param name="result">The result of the snapshot.</param>
        /// <param name="modelSnapshot">The result of the snapshot for the model.</param>
        public void Snapshot(ICoreActor actor, float time, ReadOnlySpan<ActorPartSnapshot> currentSnapshot, ActorPartSnapshot currentModelSnapshot, float transitionTime, Span<ActorPartSnapshot> result, out ActorPartSnapshot modelSnapshot)
        {
            ActorModel model = actor.Model;
            AnimationController controller = actor.Animation;
            ActorAnimation animation = controller.CurrentState.Animation;

            float scale = model.ModelHeight / model.BlockHeight;
            Vector3 modelRotXYZ = animation.GetRotation(-1, time, EasingType.Linear);
            Vector3 modelPos = animation.GetPosition(-1, time, EasingType.Linear);
            if (currentSnapshot != null && controller.TotalTime < transitionTime)
            {
                float percent = time / transitionTime;
                modelPos = Vector3.Lerp(currentModelSnapshot.Position, modelPos, percent);
            }
            Matrix modelMatrix = Matrix.Identity;

            Quaternion modelRot;
            if (modelRotXYZ != Vector3.Zero)
            {
                modelRot = CreateRotation(modelRotXYZ);
                if (currentSnapshot != null && controller.TotalTime < transitionTime)
                {
                    float percent = time / transitionTime;
                    modelRot = Quaternion.Slerp(currentModelSnapshot.Rotation, modelRot, percent);
                }
                modelMatrix *= Matrix.CreateFromQuaternion(modelRot);
            }
            else
            {
                if (currentSnapshot != null && controller.TotalTime < transitionTime)
                {
                    float percent = time / transitionTime;
                    modelRot = Quaternion.Slerp(currentModelSnapshot.Rotation, Quaternion.Identity, percent);
                }
                else
                {
                    modelRot = Quaternion.Identity;
                }
            }

            modelMatrix *= Matrix.CreateScale(scale);
            modelMatrix *= Matrix.CreateTranslation(modelPos * scale);

            modelSnapshot = new ActorPartSnapshot(null, modelPos, modelRot, modelMatrix);

            result.Clear();
            for (int i = 0; i < model.TotalParts; i++)
            {
                Vector3 finalPos;
                Quaternion finalRot;

                ActorPart part = model.GetPart(i);
                Vector3 animPos = animation.GetPosition(i, time, EasingType.Linear);
                Vector3 animRot = animation.GetRotation(i, time, EasingType.Linear);
                Matrix m = Matrix.Identity;

                if (animRot != Vector3.Zero)
                {
                    Quaternion rot = CreateRotation(animRot);
                    if (currentSnapshot != null && controller.TotalTime < transitionTime)
                    {
                        float percent = time / transitionTime;
                        rot = Quaternion.Slerp(currentSnapshot[i].Rotation, rot, percent);
                    }
                    finalRot = rot;
                }
                else
                {
                    if (currentSnapshot != null && controller.TotalTime < transitionTime)
                    {
                        float percent = time / transitionTime;
                        finalRot = Quaternion.Slerp(currentSnapshot[i].Rotation, Quaternion.Identity, percent);
                    }
                    else
                    {
                        finalRot = Quaternion.Identity;
                    }
                }

                if (finalRot != Quaternion.Identity)
                {
                    m = Matrix.CreateTranslation(-part.Pivot - Vector3.One);
                    m *= Matrix.CreateFromQuaternion(finalRot);
                    m *= Matrix.CreateTranslation(part.Pivot + Vector3.One);
                }

                if (part.Parent != -1)
                {
                    ActorPartSnapshot parent = result[part.Parent];
                    finalPos = part.Position + animPos;
                    if (currentSnapshot != null && controller.TotalTime < transitionTime)
                    {
                        float percent = time / transitionTime;
                        finalPos = Vector3.Lerp(currentSnapshot[i].Position, finalPos, percent);
                    }
                    m *= Matrix.CreateTranslation(finalPos);
                    m *= parent.Transform;
                }
                else
                {
                    finalPos = part.Position + (animPos * scale) - Vector3.One;
                    if (currentSnapshot != null && controller.TotalTime < transitionTime)
                    {
                        float percent = time / transitionTime;
                        finalPos = Vector3.Lerp(currentSnapshot[i].Position, finalPos, percent);
                    }
                    m *= Matrix.CreateTranslation(finalPos);
                    m *= modelMatrix;
                }

                result[i] = new ActorPartSnapshot(part, finalPos, finalRot, m);
            }
        }

        private Quaternion CreateRotation(Vector3 rotation)
        {
            // This will likely get changed in the future, we do this
            // order of rotation for easy animation conversion from
            // Blockbench to Core
            // Ideally we'd want to use something like
            // Quaternion.CreateFromYawPitchRoll so we only have to
            // create one quaternion, with no rotations, but that
            // method's output doesn't match Blockbench animation
            // format
            // Look into converting Blockbench animation format to
            // Core animation format at conversion time
            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation.Z);
            rot *= Quaternion.CreateFromAxisAngle(Vector3.Down, rotation.X);
            rot *= Quaternion.CreateFromAxisAngle(Vector3.Left, rotation.Y);
            return rot;
        }

        /// <summary>
        /// Gets the interpolated value at the specified point in time.
        /// </summary>
        /// <param name="partIndex">The index of the part to get.</param>
        /// <param name="channel">The channel of keyframe to get.</param>
        /// <param name="time">The time into the animation.</param>
        /// <param name="easing">The type of easing to use.</param>
        /// <returns>The interpolated value at the specified time.</returns>
        public ActorPartKeyframe GetValue(int partIndex, ActorKeyframeChannel channel, float time, EasingType easing)
        {
            if (easing == EasingType.Step)
            {
                KeyframeCollection<ActorPartKeyframe> keyframes = partIndex == -1 ? _modelKeyframes[(int)channel] : _partKeyframes[(int)channel, partIndex];
                return keyframes?.GetValue(time) ?? new ActorPartKeyframe(Vector3.Zero);
            }

            GetKeyframes(partIndex, channel, time, out Keyframe<ActorPartKeyframe> left, out Keyframe<ActorPartKeyframe> right);
            if (left.Time == right.Time)
            {
                return left.Value;
            }

            float lerp = (time - left.Time) / (right.Time - left.Time);
            Vector3 value = Vector3.Lerp(left.Value.Value, right.Value.Value, lerp);

            return new ActorPartKeyframe(value);
        }

        /// <summary>
        /// Gets the interpolated position at the specified point in time.
        /// </summary>
        /// <param name="partIndex">The index of the part to get.</param>
        /// <param name="time">The time into the animation.</param>
        /// <param name="easing">The type of easing to use.</param>
        /// <returns>The interpolated position at the specified time.</returns>
        public Vector3 GetPosition(int partIndex, float time, EasingType easing)
        {
            return GetValue(partIndex, ActorKeyframeChannel.Position, time, easing).Value;
        }

        /// <summary>
        /// Gets the interpolated rotation at the specified point in time.
        /// </summary>
        /// <param name="partIndex">The index of the part to get.</param>
        /// <param name="time">The time into the animation.</param>
        /// <param name="easing">The type of easing to use.</param>
        /// <returns>The interpolated rotation at the specified time.</returns>
        public Vector3 GetRotation(int partIndex, float time, EasingType easing)
        {
            return GetValue(partIndex, ActorKeyframeChannel.Rotation, time, easing).Value;
        }

        /// <summary>
        /// Gets the keyframes surrounding the specified point in time.
        /// </summary>
        /// <param name="partIndex">The index of the part to get.</param>
        /// <param name="channel">The channel of keyframe to get.</param>
        /// <param name="time">The time into the animation.</param>
        /// <param name="left">The keyframe to the left of the specified point in time.</param>
        /// <param name="right">The keyframe to the right of the specified point in time.</param>
        public void GetKeyframes(int partIndex, ActorKeyframeChannel channel, float time, out Keyframe<ActorPartKeyframe> left, out Keyframe<ActorPartKeyframe> right)
        {
            KeyframeCollection<ActorPartKeyframe> keyframes = partIndex == -1 ? _modelKeyframes[(int)channel] : _partKeyframes[(int)channel, partIndex];
            if (keyframes == null)
            {
                left = right = new Keyframe<ActorPartKeyframe>(0, new ActorPartKeyframe(Vector3.Zero));
                return;
            }
            keyframes.GetKeyframes(time, out left, out right);
        }

        /// <summary>
        /// Gets all of the events between <paramref name="minTime"/> and <paramref name="maxTime"/>.
        /// </summary>
        /// <param name="minTime">The minimum point in time.</param>
        /// <param name="maxTime">The maximum point in time.</param>
        /// <returns>A list of all events between <paramref name="minTime"/> and <paramref name="maxTime"/>. This will be if no events are at the specified time.</returns>
        public List<string> GetEvents(float minTime, float maxTime)
        {
            return _events?.GetAllKeyframes(minTime, maxTime);
        }

        /// <summary>
        /// Gets all of the events between <paramref name="minTime"/> and <paramref name="maxTime"/> and stores them in <paramref name="result"/>.
        /// </summary>
        /// <param name="minTime">The minimum point in time.</param>
        /// <param name="maxTime">The maximum point in time.</param>
        /// <param name="result">The list to store the result in.</param>
        public void GetEvents(float minTime, float maxTime, List<string> result)
        {
            _events?.GetAllKeyframes(minTime, maxTime, result);
        }

        /// <summary>
        /// Sets the keyframes of this animation for the specified channel.
        /// </summary>
        /// <param name="partIndex">The index of the part to set.</param>
        /// <param name="channel">The channel to set.</param>
        /// <param name="keyframes">The keyframes.</param>
        /// <returns>this</returns>
        public ActorAnimation SetKeyframes(int partIndex, ActorKeyframeChannel channel, KeyframeCollection<ActorPartKeyframe> keyframes)
        {
            if (partIndex == -1)
            {
                _modelKeyframes[(int)channel] = keyframes?.Clone();
            }
            else
            {
                _partKeyframes[(int)channel, partIndex] = keyframes?.Clone();
            }

            if (keyframes?.Length > Length)
            {
                Length = keyframes.Length;
            }
            return this;
        }

        /// <summary>
        /// Sets the events of the animation.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns>this</returns>
        public ActorAnimation SetEvents(KeyframeCollection<string> events)
        {
            _events = events?.Clone();
            if (events?.Length > Length)
            {
                Length = events.Length;
            }
            return this;
        }

        /// <summary>
        /// Creates a new, empty animation for the specified model.
        /// </summary>
        /// <param name="model">The model to create the animation for.</param>
        public ActorAnimation(ActorModel model) : this(model.TotalParts)
        {
            
        }

        /// <summary>
        /// Creates a new, empty animation with the specified number of parts.
        /// </summary>
        /// <param name="parts">The number of parts in the animation.</param>
        public ActorAnimation(int parts)
        {
            _modelKeyframes = new KeyframeCollection<ActorPartKeyframe>[_typeCount];
            _partKeyframes = new KeyframeCollection<ActorPartKeyframe>[_typeCount, parts];
            Length = 0;
        }
    }
}
