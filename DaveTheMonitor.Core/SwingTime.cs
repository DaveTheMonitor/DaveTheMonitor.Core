namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Represents the time of a swing animation.
    /// </summary>
    public struct SwingTime
    {
        /// <summary>
        /// The progress, in seconds, through the swing animation.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// The total time, in seconds, the swing lasts.
        /// </summary>
        public float TotalTime { get; private set; }

        /// <summary>
        /// The time, in seconds, at which point the swing is fully extended.
        /// </summary>
        public float ExtendTime { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SwingTime"/> instance.
        /// </summary>
        /// <param name="currentTime">The progress, in seconds, through the swing animation.</param>
        /// <param name="totalTime">The total time, in seconds, the swing lasts.</param>
        /// <param name="extendTime">The time, in seconds, at which point the swing is fully extended.</param>
        public SwingTime(float currentTime, float totalTime, float extendTime)
        {
            CurrentTime = currentTime;
            TotalTime = totalTime;
            ExtendTime = extendTime;
        }
    }
}
