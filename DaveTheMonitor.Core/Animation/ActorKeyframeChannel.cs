using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A channel for a keyframe in an <see cref="ActorAnimation"/>.
    /// </summary>
    public enum ActorKeyframeChannel
    {
        /// <summary>
        /// The position channel.
        /// </summary>
        Position,
        /// <summary>
        /// The rotation chanel.
        /// </summary>
        Rotation
    }
}
