using DaveTheMonitor.Core.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A collection of keyframes for animation.
    /// </summary>
    /// <typeparam name="T">The type of the value of the keyframes.</typeparam>
    [DebuggerDisplay("Length = {Length}, Keyframes = {_keyframes.Length}")]
    public sealed class KeyframeCollection<T>
    {
        /// <summary>
        /// The total time of this keyframe animation.
        /// </summary>
        public float Length => _keyframes.Length >= 1 ? _keyframes[_keyframes.Length - 1].Time : 0;
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
                throw new InvalidCoreJsonException("Keyframe collection must be an object");
            }

            List<Keyframe<T>> keyframes = new List<Keyframe<T>>();
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!float.TryParse(property.Name, out float time) || time < 0)
                {
                    throw new InvalidCoreJsonException("Keyframe time must be a positive number.");
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
            if (keyframes.Length == 0)
            {
                return default(T);
            }
            else if (keyframes.Length == 1)
            {
                return keyframes[0].Value;
            }

            for (int i = keyframes.Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time)
                {
                    return keyframes[i].Value;
                }
            }

            // If no keyframes <= time, we must be in negative time.
            // We treat this as the first keyframe.
            return keyframes[0].Value;
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
            if (keyframes.Length == 0)
            {
                keyframe1 = keyframe2 = new Keyframe<T>(0, default(T));
                return;
            }
            else if (keyframes.Length == 1)
            {
                keyframe1 = keyframe2 = keyframes[0];
                return;
            }
            
            if (time >= Length)
            {
                keyframe1 = keyframe2 = keyframes[_keyframes.Length - 1];
                return;
            }

            for (int i = keyframes.Length - 1; i >= 0; i--)
            {
                if (keyframes[i].Time <= time)
                {
                    keyframe1 = keyframes[i];
                    keyframe2 = keyframes[Math.Min(i + 1, _keyframes.Length - 1)];
                    return;
                }
            }

            // If no keyframes <= time, we must be in negative time.
            // We treat this as the first keyframe.
            keyframe1 = keyframe2 = keyframes[0];
        }

        /// <summary>
        /// Gets all of the keyframes between <paramref name="minTime"/> and <paramref name="maxTime"/> and stores them in <paramref name="result"/>.
        /// </summary>
        /// <param name="minTime">The minimum time.</param>
        /// <param name="maxTime">The maximum time.</param>
        /// <param name="result">The list to store the result in.</param>
        public void GetAllKeyframes(float minTime, float maxTime, List<T> result)
        {
            result.Clear();
            if (_keyframes.Length == 0)
            {
                return;
            }

            if (_keyframes.Length == 1)
            {
                if (_keyframes[0].Time >= minTime && _keyframes[0].Time <= maxTime)
                {
                    result.Add(_keyframes[0].Value);
                }
                return;
            }

            foreach (Keyframe<T> keyframe in _keyframes)
            {
                if (keyframe.Time >= minTime && keyframe.Time <= maxTime)
                {
                    result.Add(keyframe.Value);
                }
            }
        }

        /// <summary>
        /// Gets all of the keyframes between <paramref name="minTime"/> and <paramref name="maxTime"/>.
        /// </summary>
        /// <param name="minTime">The minimum time.</param>
        /// <param name="maxTime">The maximum time.</param>
        /// <returns>A list of all keyframes between <paramref name="minTime"/> and <paramref name="maxTime"/>. This will be null if there are no keyframes within the specified timeframe.</returns>
        public List<T> GetAllKeyframes(float minTime, float maxTime)
        {
            if (_keyframes.Length == 0)
            {
                return null;
            }

            if (_keyframes.Length == 1)
            {
                if (_keyframes[0].Time >= minTime && _keyframes[0].Time <= maxTime)
                {
                    return new List<T>()
                    {
                        _keyframes[0].Value
                    };
                }
            }

            List<T> list = null;
            foreach (Keyframe<T> keyframe in _keyframes)
            {
                if (keyframe.Time >= minTime && keyframe.Time <= maxTime)
                {
                    list ??= new List<T>();
                    list.Add(keyframe.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// Sets the keyframes in this <see cref="KeyframeCollection{T}"/> to a copy of <paramref name="keyframes"/>.
        /// </summary>
        /// <param name="keyframes">The keyframes to copy.</param>
        public void SetKeyframes(IEnumerable<Keyframe<T>> keyframes)
        {
            _keyframes = keyframes.ToArray();
            Array.Sort(_keyframes);
        }

        /// <summary>
        /// Adds the specified keyframe to this <see cref="KeyframeCollection{T}"/>.
        /// </summary>
        /// <param name="time">The time of this keyframe.</param>
        /// <param name="keyframe">The keyframe to add.</param>
        /// <returns>This <see cref="KeyframeCollection{T}"/></returns>
        public KeyframeCollection<T> Add(float time, T keyframe)
        {
            int index = _keyframes.Length;
            Array.Resize(ref _keyframes, _keyframes.Length + 1);
            _keyframes[index] = new Keyframe<T>(time, keyframe);
            Array.Sort(_keyframes);
            return this;
        }

        /// <summary>
        /// Clones this <see cref="KeyframeCollection{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="KeyframeCollection{T}"/> this the contents of this <see cref="KeyframeCollection{T}"/>.</returns>
        public KeyframeCollection<T> Clone()
        {
            KeyframeCollection<T> copy = new KeyframeCollection<T>(_keyframes);
            return copy;
        }

        /// <summary>
        /// Creates a new, empty <see cref="KeyframeCollection{T}"/>.
        /// </summary>
        public KeyframeCollection()
        {
            _keyframes = Array.Empty<Keyframe<T>>();
        }

        /// <summary>
        /// Creates a new <see cref="KeyframeCollection{T}"/> containing a copy of <paramref name="keyframes"/>.
        /// </summary>
        /// <param name="keyframes">The keyframes to copy.</param>
        public KeyframeCollection(IEnumerable<Keyframe<T>> keyframes)
        {
            SetKeyframes(keyframes);
        }
    }
}
