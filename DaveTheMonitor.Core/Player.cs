using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Invokers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;

namespace DaveTheMonitor.Core
{
    internal sealed class Player : Actor, ICorePlayer
    {
        public ITMPlayer TMPlayer { get; private set; }
        public bool IsLocalPlayer => Gamer.IsLocal;
        public float FarClip => TMPlayer.GetFarClip();
        public float FogIntensity => TMPlayer.GetFogIntensity();
        public float FogVisibility => TMPlayer.GetFogVisibility();
        public Rectangle HudBounds => _getHudPosInvoker(TMPlayer);

        #region Scripts

        protected override string ScriptClanName => TMPlayer.ClanName;
        protected override int ScriptCursorX => TMPlayer.SwingFace != BlockFace.ProxyDefault ? TMPlayer.SwingTarget.X : 0;
        protected override int ScriptCursorY => TMPlayer.SwingFace != BlockFace.ProxyDefault ? TMPlayer.SwingTarget.Y : 0;
        protected override int ScriptCursorZ => TMPlayer.SwingFace != BlockFace.ProxyDefault ? TMPlayer.SwingTarget.Z : 0;

        protected override int ScriptGetAction(string item, string action)
        {
            Item? id = Game.ItemRegistry.GetDefinition(item)?.ItemType;
            if (!id.HasValue)
            {
                return 0;
            }

            return action switch
            {
                "used" => TMPlayer.GetAction(id.Value, ItemAction.Used),
                "crafted" => TMPlayer.GetAction(id.Value, ItemAction.Crafted),
                "collected" => TMPlayer.GetAction(id.Value, ItemAction.Collected),
                "mined" => TMPlayer.GetAction(id.Value, ItemAction.Mined),
                _ => 0
            };
        }

        #endregion

        #region ICorePlayer

        public override CoreActor CoreActor => Game.ActorRegistry.GetActor(ActorType.Player);
        public override GamerID Id => TMPlayer.GamerID;
        public PlayerIndex PlayerIndex => TMPlayer.PlayerIndex;
        public NetworkGamer Gamer => TMPlayer.Gamer;
        public ITMPlayer TMVirtualPlayer => TMPlayer.VirtualPlayer;
        public string ClanName => TMPlayer.ClanName;
        public bool IsGod => TMPlayer.IsGod;
        public bool IsInputEnabled => TMPlayer.IsInputEnabled;
        public Matrix WorldShake => TMPlayer.WorldShake;
        public Matrix WorldToolShake => TMPlayer.WorldToolShake;
        public int SwingFacePos => TMPlayer.SwingFacePos;
        public BlockFace SwingFace => TMPlayer.SwingFace;
        public GlobalPoint3D SwingTarget => TMPlayer.SwingTarget;
        public float SwingTargetDistance => TMPlayer.SwingTargetDistance;
        public GlobalPoint3D PlaceTarget => TMPlayer.PlaceTarget;
        public History History => TMPlayer.History;
        public ICoreActor ActorInReticle => TMPlayer.ActorInReticle != null ? World.ActorManager.GetActor(TMPlayer.ActorInReticle) : null;
        public IDictionary<string, TeleportMark> Teleports => TMPlayer.Teleports;
        public void AddTeleport(string name) => TMPlayer.AddTeleport(name);
        public bool RemoveTeleport(string name) => TMPlayer.RemoveTeleport(name);
        public bool TeleportTo(string name) => TMPlayer.TeleportTo(name);
        public void PasteCurrentClipboard(GlobalPoint3D p, BlockFace facing) => TMPlayer.PasteCurrentClipboard(p, facing);
        public int GetAction(CoreItem item, ItemAction action) => TMPlayer.GetAction(item.ItemType, action);
        public bool HasAction(CoreItem item, ItemAction action) => TMPlayer.HasAction(item.ItemType, action);

        #endregion

        #region Invokers

        private static GetHudPosInvoker _getHudPosInvoker;

        #endregion

        static Player()
        {
            Type graphicStatics = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.GraphicStatics");
            Type player = AccessTools.TypeByName("StudioForge.TotalMiner.Player");
            _getHudPosInvoker = AccessTools.Method(graphicStatics, "HUDPos", new Type[] { player }).CreateInvoker<GetHudPosInvoker>();
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            string world = reader.ReadString();
            if (reader.ReadBoolean())
            {
                Data.ReadState(Game.ModManager, reader, tmVersion, coreVersion);
            }
        }

        public void WriteState(BinaryWriter writer)
        {
            writer.Write(World.Id);
            if (Data.ShouldSaveState())
            {
                writer.Write(true);
                Data.WriteState(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public bool ShouldSaveState()
        {
            return World.Id != "Core.Overworld" || Data.ShouldSaveState();
        }

        public Player(ICoreGame game, ICoreWorld world, ITMPlayer player) : base(game, world, player)
        {
            TMPlayer = player;
        }
    }
}
