using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A keyframe for an <see cref="ActorAnimation"/>.
    /// </summary>
    public struct ActorPartKeyframe
    {
        /// <summary>
        /// The value of this keyframe. This may represent position or rotation depending on the channel.
        /// </summary>
        public Vector3 Value { get; set; }

        /// <summary>
        /// Returns the string representation of this keyframe.
        /// </summary>
        /// <returns>The string representation of this keyframe.</returns>
        public override string ToString() => Value.ToString();

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ActorPartKeyframe keyframe && keyframe.Value == Value;
        }

        /// <summary>
        /// Creates a new <see cref="ActorPartKeyframe"/> with the specified value.
        /// </summary>
        /// <param name="value">The value of the keyframe.</param>
        public ActorPartKeyframe(Vector3 value)
        {
            Value = value;
        }
    }
}
