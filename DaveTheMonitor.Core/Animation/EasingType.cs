using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// Id of easing for animations.
    /// </summary>
    public enum EasingType
    {
        /// <summary>
        /// Linearly interpolates between x and y.
        /// </summary>
        Linear,

        /// <summary>
        /// Instantly steps between x and y.
        /// </summary>
        Step
    }
}
