using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing an actor model.
    /// </summary>
    public sealed class CoreActorModelAsset : CoreModAsset
    {
        /// <summary>
        /// The model of this asset.
        /// </summary>
        public ActorModel Model { get; private set; }

        /// <summary>
        /// Creates a new asset containing this specified map.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="model">The model of this asset.</param>
        public CoreActorModelAsset(string fullPath, string name, ActorModel model) : base(fullPath, name)
        {
            Model = model;
        }
    }
}
