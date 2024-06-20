﻿using DaveTheMonitor.Core.Animation.Json;
using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// A core mod asset containing an actor animation.
    /// </summary>
    public sealed class CoreActorAnimationAsset : CoreModAsset
    {
        /// <summary>
        /// The model of this asset.
        /// </summary>
        public JsonActorAnimation Animation { get; private set; }

        /// <summary>
        /// Creates a new asset containing this specified map.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset.</param>
        /// <param name="animation">The animation of this asset.</param>
        public CoreActorAnimationAsset(string fullPath, string name, JsonActorAnimation animation) : base(fullPath, name)
        {
            Animation = animation;
        }
    }
}
