using StudioForge.TotalMiner;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A <see cref="IDefinitionRegistry{T}"/> containing <see cref="CoreActor"/> definitions.
    /// </summary>
    public interface ICoreActorRegistry : IDefinitionRegistry<CoreActor>
    {
        /// <summary>
        /// Indexer for actor types, equivalent to <see cref="GetActor(ActorType)"/>.
        /// </summary>
        /// <param name="actor">The actor type.</param>
        /// <returns>The <see cref="CoreActor"/> definition for the specified <see cref="ActorType"/>.</returns>
        CoreActor this[ActorType actor] { get; }

        /// <summary>
        /// Initializes all actor definitions from <see cref="ActorTypeDataXML"/>.
        /// </summary>
        /// <param name="data">The data to initialize from.</param>
        void InitializeAllActors(IEnumerable<ActorTypeDataXML> data);

        /// <summary>
        /// Updates global actor data to use the actor definitions from this registry.
        /// </summary>
        void UpdateGlobalActorData();

        /// <summary>
        /// Gets the <see cref="CoreActor"/> definition for an <see cref="ActorType"/>.
        /// </summary>
        /// <param name="actorType">The actor type.</param>
        /// <returns>The definition for the specified <see cref="ActorType"/>.</returns>
        public CoreActor GetActor(ActorType actorType);
    }
}
