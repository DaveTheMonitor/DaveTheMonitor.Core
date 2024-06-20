using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A type of loop for an animation.
    /// </summary>
    public enum AnimationLoopType
    {
        /// <summary>
        /// The animation will not loop and will stay on the last frame until the state is changed.
        /// </summary>
        None,

        /// <summary>
        /// The animation will loop indefinitely until the state is changed.
        /// </summary>
        Loop
    }
}
