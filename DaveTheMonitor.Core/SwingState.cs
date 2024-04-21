namespace DaveTheMonitor.Core
{
    /// <summary>
    /// The state of a swing.
    /// </summary>
    public enum SwingState
    {
        /// <summary>
        /// The hand is not swinging.
        /// </summary>
        None,

        /// <summary>
        /// The swing is extending.
        /// </summary>
        Extending,

        /// <summary>
        /// The swing is fully extended. This is when damage is dealt and item events are called.
        /// </summary>
        Extended,

        /// <summary>
        /// The swing is retracting.
        /// </summary>
        Retracting,

        /// <summary>
        /// The swing is in cooldown.
        /// </summary>
        Delay,

        /// <summary>
        /// The swing is fully complete.
        /// </summary>
        Complete
    }
}
