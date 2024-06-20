using System;
using System.Diagnostics;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A keyframe containing a value and time.
    /// </summary>
    /// <typeparam name="T">The type of the value of this keyframe.</typeparam>
    [DebuggerDisplay("Time = {Time}, Value = {Value}")]
    public struct Keyframe<T> : IComparable<Keyframe<T>>
    {
        /// <summary>
        /// The value of this keyframe.
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// The time of this keyframe.
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// Compares this keyframe to another keyframe based on the time.
        /// </summary>
        /// <param name="other">The keyframe to compare to.</param>
        /// <returns>
        /// <para>-1 if this keyframe is before <paramref name="other"/></para>
        /// <para>1 if this keyframe is after <paramref name="other"/></para>
        /// <para>0 if this keyframe matches <paramref name="other"/> in time.</para></returns>
        public int CompareTo(Keyframe<T> other)
        {
            return Time < other.Time ? -1 : Time > other.Time ? 1 : 0;
        }

        /// <summary>
        /// Creates a new keyframe with the specified time and value.
        /// </summary>
        /// <param name="time">The time of the keyframe.</param>
        /// <param name="value">The value of the keyframe.</param>
        public Keyframe(float time, T value)
        {
            Value = value;
            Time = time;
        }
    }
}
