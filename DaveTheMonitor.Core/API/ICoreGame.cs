using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Compiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A main game instance containing data and information independent of the world.
    /// </summary>
    public interface ICoreGame : IHasCoreData<ICoreGame>, IHasBinaryState
    {
        /// <summary>
        /// The game's <see cref="ITMGame"/> implementation. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMGame TMGame { get; }

        /// <summary>
        /// This game's <see cref="ICoreModManager"/>.
        /// </summary>
        ICoreModManager ModManager { get; }

        /// <summary>
        /// This game's <see cref="IGameShader"/>. Used to modify shader parameters.
        /// </summary>
        IGameShader GameShader { get; }

        /// <summary>
        /// This game's main <see cref="IMapComponentLoader"/>.
        /// </summary>
        IMapComponentLoader MapComponentLoader { get; }

        /// <summary>
        /// This game's main <see cref="ICoreItemRegistry"/>.
        /// </summary>
        ICoreItemRegistry ItemRegistry { get; }

        /// <summary>
        /// This game's main <see cref="ICoreActorRegistry"/>.
        /// </summary>
        ICoreActorRegistry ActorRegistry { get; }

        /// <summary>
        /// This game's main <see cref="IScriptRuntime"/>.
        /// </summary>
        IScriptRuntime ScriptRuntime { get; set; }

        /// <summary>
        /// This game's main <see cref="IScriptCompiler"/>.
        /// </summary>
        IScriptCompiler ScriptCompiler { get; set; }

        /// <summary>
        /// This game's main Random instance.
        /// </summary>
        PcgRandom Random { get; }

        /// <summary>
        /// The main <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/>.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// This game's main <see cref="StudioForge.Engine.GUI.WindowManager"/>.
        /// </summary>
        WindowManager WindowManager { get; }

        /// <summary>
        /// The currently active <see cref="ITMTexturePack"/>.
        /// </summary>
        ITMTexturePack TexturePack { get; }

        /// <summary>
        /// The main <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>.
        /// </summary>
        SpriteBatchSafe SpriteBatch { get; }

        /// <summary>
        /// True if this client is the host of the session, otherwise false.
        /// </summary>
        bool IsHost { get; }

        /// <summary>
        /// The full path of this world.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Gets the local player with the specified <see cref="PlayerIndex"/>.
        /// </summary>
        /// <param name="playerIndex">The <see cref="PlayerIndex"/> of the player.</param>
        /// <returns>The player with the specified <see cref="PlayerIndex"/>.</returns>
        ICorePlayer GetLocalPlayer(PlayerIndex playerIndex);

        /// <summary>
        /// Adds a notification with the specified message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void AddNotification(string message);

        /// <summary>
        /// Adds a notification with the specified message and recipient.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="recType">The recipient.</param>
        void AddNotification(string message, NotifyRecipient recType);

        /// <summary>
        /// Adds a notification with the specified message, color, and recipient.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="recType">The recipient.</param>
        void AddNotification(string message, Color color, NotifyRecipient recType);

        /// <summary>
        /// Opens the pause menu with one or more custom tabs.
        /// </summary>
        /// <param name="player">The player to open the menu for.</param>
        /// <param name="menus">The menus to add.</param>
        /// <param name="mainTabs">True if the main pause menu tabs should be visible, otherwise false.</param>
        /// <returns>The screen containing the specified menus.</returns>
        INewGuiMenuScreen OpenMenu(ICorePlayer player, NewGuiMenu[] menus = null, bool mainTabs = true);

        /// <summary>
        /// Adds a screen to the game.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        /// <param name="player">The player to add the screen for.</param>
        void AddScreen(IGameScreen screen, ICorePlayer player);

        /// <summary>
        /// Returns true if the specified player is in the game.
        /// </summary>
        /// <param name="player">The player to test.</param>
        /// <returns>True if the specified player is in the game, otherwise false.</returns>
        bool IsInGame(ICorePlayer player);

        /// <summary>
        /// Returns true if the specified player is in the game.
        /// </summary>
        /// <param name="player">The player to test.</param>
        /// <returns>True if the specified player is in the game, otherwise false.</returns>
        bool IsInGame(ITMPlayer player);

        /// <summary>
        /// Calls <see cref="ICorePlugin.ModCall(object[])"/> for the mod with the specified ID, if it exists.
        /// </summary>
        /// <param name="modId">The ID of the mod to call.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The value returned by the mod, or null if no value was returned.</returns>
        object ModCall(string modId, params object[] args);

        /// <summary>
        /// Gets am <see cref="IEnumerable{T}"/> of all worlds.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all worlds.</returns>
        IEnumerable<ICoreWorld> GetAllWorlds();

        /// <summary>
        /// Gets the world with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the world to get.</param>
        /// <returns>The world with the specified, or null if it doesn't exist.</returns>
        ICoreWorld GetWorld(string id);

        /// <summary>
        /// Gets the world using the specified map.
        /// </summary>
        /// <param name="map">The map used by the world.</param>
        /// <returns>The world using the specified map, ot null if no world is found.</returns>
        ICoreWorld GetWorld(ICoreMap map);

        /// <summary>
        /// Gets the world using the specified map.
        /// </summary>
        /// <param name="map">The map used by the world.</param>
        /// <returns>The world using the specified map, ot null if no world is found.</returns>
        ICoreWorld GetWorld(Map map);

        /// <summary>
        /// Gets the <see cref="ICorePlayer"/> for the specified <see cref="ITMPlayer"/>.
        /// </summary>
        /// <param name="player">The player to get.</param>
        /// <returns>The <see cref="ICorePlayer"/> for the specified <see cref="ITMPlayer"/>.</returns>
        ICorePlayer GetPlayer(ITMPlayer player);

        /// <summary>
        /// Gets the <see cref="ICoreActor"/> for the specified <see cref="ITMActor"/>.
        /// </summary>
        /// <param name="actor">The actor to get.</param>
        /// <returns>The <see cref="ICoreActor"/> for the specified <see cref="ITMActor"/>.</returns>
        ICoreActor GetActor(ITMActor actor);

        /// <summary>
        /// Returns true if the game has any state to save.
        /// </summary>
        /// <returns>True if the game has any state to save, otherwise false.</returns>
        bool ShouldSaveState();

        /// <summary>
        /// Loads the save state for the specified player.
        /// </summary>
        /// <param name="player">The player to load state for.</param>
        void LoadPlayerState(ICorePlayer player);
    }
}
