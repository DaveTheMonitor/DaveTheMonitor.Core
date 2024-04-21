namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Represents a definition for an <see cref="IDefinitionRegistry{T}"/>
    /// </summary>
    public interface IDefinition : IHasId
    {
        /// <summary>
        /// The numeric ID of this definition.
        /// </summary>
        public int NumId { get; set; }

        /// <summary>
        /// Called when this definition is registered.
        /// </summary>
        /// <param name="mod">The mod that registered this definition.</param>
        void OnRegister(ICoreMod mod);
    }
}
