using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A snapshot of an <see cref="ActorPart"/>'s transform during an animation.
    /// </summary>
    public struct ActorPartSnapshot
    {
        /// <summary>
        /// The part this snapshot is for.
        /// </summary>
        public ActorPart Part { get; set; }

        /// <summary>
        /// The position of this snapshot.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of this snapshot.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The full model matrix of this snapshot.
        /// </summary>
        public Matrix Transform { get; set; }

        /// <summary>
        /// Creates a new <see cref="ActorPartSnapshot"/>.
        /// </summary>
        /// <param name="part">The part of this snapshot.</param>
        /// <param name="position">The position of this snapshot.</param>
        /// <param name="rotation">The rotation of this snapshot.</param>
        /// <param name="transform">The model matrix for this snapshot.</param>
        public ActorPartSnapshot(ActorPart part, Vector3 position, Quaternion rotation, Matrix transform)
        {
            Part = part;
            Position = position;
            Rotation = rotation;
            Transform = transform;
        }
    }
}
