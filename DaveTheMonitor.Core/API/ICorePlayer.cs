using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// A player in the world.
    /// </summary>
    public interface ICorePlayer : ICoreActor, IHasBinaryState
    {
        /// <summary>
        /// The game's <see cref="ITMPlayer"/> implementation for this player. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMPlayer TMPlayer { get; }

        /// <summary>
        /// This player's <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        PlayerIndex PlayerIndex { get; }

        /// <summary>
        /// This player's <see cref="NetworkGamer"/>.
        /// </summary>
        NetworkGamer Gamer { get; }

        /// <summary>
        /// This player's virtual player, eg. spectate or CCTV.
        /// </summary>
        ITMPlayer TMVirtualPlayer { get; }

        /// <summary>
        /// This player's clan name.
        /// </summary>
        string ClanName { get; }

        /// <summary>
        /// True if this player has god mode.
        /// </summary>
        bool IsGod { get; }

        /// <summary>
        /// True if this player's input is enabled.
        /// </summary>
        bool IsInputEnabled { get; }

        /// <summary>
        /// This player's world shake matrix.
        /// </summary>
        Matrix WorldShake { get; }

        /// <summary>
        /// This player's world tool shake matrix.
        /// </summary>
        Matrix WorldToolShake { get; }

        /// <summary>
        /// The quarter position of the reticle on the block face. A block face is divided
        /// into 4 quarters and a center quarter. Values 0 - 3 represent the 4 corner quarters.
        /// If Value is > 3 then the recticle is also in the center quarter. In this case,
        /// AND the value with 0x03 to extract the corner quarter.
        /// </summary>
        int SwingFacePos { get; }

        /// <summary>
        /// The player's <see cref="BlockFace"/> swing face, or <see cref="BlockFace.ProxyDefault"/> is the player is not looking at a block.
        /// </summary>
        BlockFace SwingFace { get; }

        /// <summary>
        /// The block position the player is looking at.
        /// </summary>
        GlobalPoint3D SwingTarget { get; }

        /// <summary>
        /// The distance between the player's eye position and the target block position.
        /// </summary>
        float SwingTargetDistance { get; }

        /// <summary>
        /// The position a block would be placed if the player were to place a block.
        /// </summary>
        GlobalPoint3D PlaceTarget { get; }

        /// <summary>
        /// The player's history.
        /// </summary>
        History History { get; }

        /// <summary>
        /// The actor in the player's reticle, or null if the player is not looking at another actor.
        /// </summary>
        ICoreActor ActorInReticle { get; }

        /// <summary>
        /// The player's teleport points.
        /// </summary>
        IDictionary<string, TeleportMark> Teleports { get; }

        /// <summary>
        /// True if the player is a local client, otherwise false.
        /// </summary>
        bool IsLocalPlayer { get; }

        /// <summary>
        /// The player's far clip.
        /// </summary>
        float FarClip { get; }

        /// <summary>
        /// The intensity of the fog for this player.
        /// </summary>
        /// <remarks>Note that this value is unaffected by shader modifiers. Shaders should use <see cref="IGameShader.FogColor"/> instead.</remarks>
        float FogIntensity { get; }

        /// <summary>
        /// The intensity of the fog for this player.
        /// </summary>
        /// <remarks>Note that this value is unaffected by shader modifiers. Shaders should use <see cref="IGameShader.FogEnd"/> instead.</remarks>
        float FogVisibility { get; }

        /// <summary>
        /// This player's HUD bounds.
        /// </summary>
        Rectangle HudBounds { get; }

        /// <summary>
        /// Adds a teleport point for this player at the player's current position and view direction.
        /// </summary>
        /// <param name="name">The name of the teleport point.</param>
        void AddTeleport(string name);

        /// <summary>
        /// Removes a teleport point for this player.
        /// </summary>
        /// <param name="name">The name of the teleport point.</param>
        /// <returns>True if the teleport point is removed, otherwise false.</returns>
        bool RemoveTeleport(string name);

        /// <summary>
        /// Teleports this player to the specified teleport point.
        /// </summary>
        /// <param name="name">The name of the teleport point.</param>
        /// <returns>True if the player was teleported, otherwise false.</returns>
        bool TeleportTo(string name);

        /// <summary>
        /// Pastes the player's current clipboard.
        /// </summary>
        /// <param name="p">The position to paste at.</param>
        /// <param name="facing">The direction of the clipboard.</param>
        void PasteCurrentClipboard(GlobalPoint3D p, BlockFace facing);

        /// <summary>
        /// Gets this player's action count for the specified item.
        /// </summary>
        /// <param name="item">The item to get the action for.</param>
        /// <param name="action">The action to get.</param>
        /// <returns>The action count for the specified item.</returns>
        int GetAction(CoreItem item, ItemAction action);

        /// <summary>
        /// Returns true if this player has the specified action for the specified item.
        /// </summary>
        /// <param name="item">The item to get the action for.</param>
        /// <param name="action">The action to get.</param>
        /// <returns>True if this player has the specified action, otherwise false.</returns>
        bool HasAction(CoreItem item, ItemAction action);

        /// <summary>
        /// Returns true if this player has any state to save.
        /// </summary>
        /// <returns>True if this player has any state to save, otherwise false.</returns>
        bool ShouldSaveState();
    }
}
