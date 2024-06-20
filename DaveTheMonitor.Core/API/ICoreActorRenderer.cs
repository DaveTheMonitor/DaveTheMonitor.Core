using DaveTheMonitor.Core.Events;
using Microsoft.Xna.Framework;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Used to render actors with custom models.
    /// </summary>
    public interface ICoreActorRenderer
    {
        /// <summary>
        /// If true, the position and pivot of each part will be drawn. Useful for debugging or ensuring your model's positions and pivots are correct.
        /// </summary>
        bool DrawPositions { get; set; }

        /// <summary>
        /// Loads the content for this actor renderer.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Adds an actor to be rendered in the next frame.
        /// </summary>
        /// <param name="actor">The actor to render.</param>
        void AddActorToRender(ICoreActor actor);

        /// <summary>
        /// Draws all actors added with <see cref="AddActorToRender(ICoreActor)"/>
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        void Draw(ICorePlayer player, ITMPlayer virtualPlayer);
    }
}
