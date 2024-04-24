using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A plugin to be loaded by the Core Mod.
    /// </summary>
    /// <remarks>Types that implement this interface must also specify <see cref="PluginEntryAttribute"/> to be loaded.</remarks>
    public interface ICorePlugin
    {
        /// <summary>
        /// Called when this plugin is first initialized.
        /// </summary>
        /// <param name="mod">The mod that this plugin belongs to.</param>
        void Initialize(ICoreMod mod)
        {

        }

        /// <summary>
        /// Called when the game is initialized, after all plugins have been initialized.
        /// </summary>
        /// <param name="game">The main game instance.</param>
        void InitializeGame(ICoreGame game)
        {

        }

        /// <summary>
        /// Called when a world is initialized, after the game has been initialized.
        /// </summary>
        /// <param name="world">The world being initialized. See <see cref="ICoreWorld"/> for more information.</param>
        void InitializeWorld(ICoreWorld world)
        {

        }

        /// <summary>
        /// Called when this mod is unloaded. Dispose any resources here.
        /// </summary>
        void UnloadMod()
        {

        }

        /// <summary>
        /// Called when this mod is hot loaded. Dispose any resources here.
        /// </summary>
        void HotLoadMod()
        {

        }

        /// <summary>
        /// Called every frame when input should be handled.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if input was handled, otherwise false.</returns>
        bool HandleInput(ICorePlayer player)
        {
            return false;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        void Update()
        {

        }

        /// <summary>
        /// Called every frame for every world.
        /// </summary>
        /// <param name="world">The world being updated. See <see cref="ICoreWorld"/> for more information.</param>
        void Update(ICoreWorld world)
        {

        }

        /// <summary>
        /// Called every frame for every actor in the game.
        /// </summary>
        /// <param name="actor">The actor being updated.</param>
        void Update(ICoreActor actor)
        {

        }

        /// <summary>
        /// Called every rendered frame. Add custom drawing logic here.
        /// </summary>
        /// <param name="player">The local player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="vp">The viewport to draw to.</param>
        void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {

        }

        /// <summary>
        /// Called immediately before MapRenderer.Draw. Can be used for more advanced rendering, but most plugins won't need it.
        /// </summary>
        /// <param name="map">The map being drawn.</param>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="options">The draw options.</param>
        void PreDrawWorldMap(ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {

        }

        /// <summary>
        /// Called immediately after MapRenderer.Draw. Can be used for more advanced rendering, but most plugins won't need it.
        /// </summary>
        /// <param name="map">The map that was drawn.</param>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="options">The draw options.</param>
        void PostDrawWorldMap(ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options)
        {

        }

        /// <summary>
        /// Called by other mods for cross-mod communication.
        /// </summary>
        /// <param name="args">The arguments passed by the mod.</param>
        /// <returns>The result of the mod call, or null if no result should be returned.</returns>
        object ModCall(params object[] args)
        {
            return null;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of Lua function implementations.
        /// </summary>
        /// <param name="si">The script instance to register the functions to.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of Lua function implementations, or null if no functions are registered.</returns>
        IEnumerable<object> RegisterLuaFunctions(ITMScriptInstance si)
        {
            return null;
        }
    }
}
