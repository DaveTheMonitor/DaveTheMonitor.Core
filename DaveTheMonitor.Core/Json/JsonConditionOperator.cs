namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// Represents an operator for a <see cref="JsonCondition"/>.
    /// </summary>
    public enum JsonConditionOperator
    {
        /// <summary>
        /// Equal, equivalent to 'left == right'.
        /// </summary>
        Equal,

        /// <summary>
        /// Not Equal, equivalent to 'left != right'.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Greater than, equivalent to 'left &gt; right'.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal, equivalent to 'left &gt;= right'.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Less than, equivalent to 'left &lt; right'.
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equak, equivalent to 'left &lt;= right'.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Modulo, equivalent to '(left % right) == 0'.
        /// </summary>
        Modulo
    }
}
