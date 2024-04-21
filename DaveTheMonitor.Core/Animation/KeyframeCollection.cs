using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A collection of keyframes for animation.
    /// </summary>
    /// <typeparam name="T">The type of the value of the keyframes.</typeparam>
    public sealed class KeyframeCollection<T>
    {
        /// <summary>
        /// The total time of this keyframe animation.
        /// </summary>
        public float TotalTime => _keyframes[_keyframes.Length - 1].Time;
        private Keyframe<T>[] _keyframes;

        /// <summary>
        /// Reads a keyframe collection from a Json object.
        /// </summary>
        /// <param name="element">The json element is read.</param>
        /// <param name="parse">A function to parse <typeparamref name="T"/> from a <see cref="JsonElement"/>.</param>
        /// <returns>A keyframe collection defined by the Json.</returns>
        /// <exception cref="InvalidOperationException">The Json object is not valid.</exception>
        /// <remarks>
        /// The Json is expected to be an object, where the keys are the keyframe times and the values are the objects parsed by <paramref name="parse"/>.
        /// <code>
        /// {
        ///     "0": [ 0, 0, 0 ]
        ///     "0.5": [ 0, 2, 0 ]
        ///     "1": [ 0, 0, 0 ]
        /// }
        /// </code>
        /// </remarks>
        public static KeyframeCollection<T> FromJson(JsonElement element, Func<JsonElement, T> parse)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("Keyframe collection must be an object");
            }

            List<Keyframe<T>> keyframes = new List<Keyframe<T>>();
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!float.TryParse(property.Name, out float time) || time < 0)
                {
                    throw new InvalidOperationException("Keyframe time must be a positive number.");
                }

                Keyframe<T> keyframe = new Keyframe<T>(time, parse(property.Value));
                keyframes.Add(keyframe);
            }

            keyframes.Sort((left, right) => left.Time.CompareTo(right.Time));

            return new KeyframeCollection<T>(keyframes);
        }


        /// <summary>
        /// Gets the value of the lowest keyframe at the specified point in the animation.
        /// </summary>
        /// <param name="time">The time since the start of the animation.</param>
        /// <returns>The value of the keyframe at the specified time.</returns>
        public T GetValue(float time)
        {
            Keyframe<T>[] keyframes = _keyframes;
            for (int i = keyframes.Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time)
                {
                    return keyframes[i].Value;
                }
            }

            // If no keyframes <= time, we must be on the last keyframe
            return keyframes[keyframes.Length - 1].Value;
        }

        /// <summary>
        /// Gets the keyframes surrounding the specified point in the animation. <paramref name="keyframe1"/> and <paramref name="keyframe2"/> may be equal if the time exactly matches the keyframe's time.
        /// </summary>
        /// <param name="time">The time since the start of the animation.</param>
        /// <param name="keyframe1">The keyframe before the specified time.</param>
        /// <param name="keyframe2">The keyframe after the specified time.</param>
        public void GetKeyframes(float time, out Keyframe<T> keyframe1, out Keyframe<T> keyframe2)
        {
            Keyframe<T>[] keyframes = _keyframes;
            for (int i = keyframes.Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time)
                {
                    keyframe1 = keyframes[i];
                    keyframe2 = keyframes[Math.Min(i + 1, _keyframes.Length - 1)];
                    return;
                }
            }

            keyframe1 = keyframes[_keyframes.Length - 1];
            keyframe2 = keyframe1;
        }

        /// <summary>
        /// Sets the keyframes in this <see cref="KeyframeCollection{T}"/> to a copy of <paramref name="keyframes"/>.
        /// </summary>
        /// <param name="keyframes">The keyframes to copy.</param>
        public void SetKeyFrames(IEnumerable<Keyframe<T>> keyframes)
        {
            _keyframes = keyframes.ToArray();
            Array.Sort(_keyframes);
        }

        /// <summary>
        /// Creates a new <see cref="KeyframeCollection{T}"/> containing a copy of <paramref name="keyframes"/>.
        /// </summary>
        /// <param name="keyframes">The keyframes to copy.</param>
        public KeyframeCollection(IEnumerable<Keyframe<T>> keyframes)
        {
            SetKeyFrames(keyframes);
        }
    }
}
