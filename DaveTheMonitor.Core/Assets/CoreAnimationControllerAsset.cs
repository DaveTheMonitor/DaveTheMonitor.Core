using DaveTheMonitor.Core.Animation.Json;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing an animation controller.
    /// </summary>
    public sealed class CoreAnimationControllerAsset : CoreModAsset
    {
        /// <summary>
        /// The animation controller of this asset.
        /// </summary>
        public JsonAnimationController AnimationController { get; private set; }

        /// <summary>
        /// Creates a new asset containing this specified map.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="animation">The animation controller of this asset.</param>
        public CoreAnimationControllerAsset(string fullPath, string name, JsonAnimationController animationController) : base(fullPath, name)
        {
            AnimationController = animationController;
        }
    }
}
