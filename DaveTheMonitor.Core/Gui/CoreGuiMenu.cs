using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Gui
{
    public abstract class CoreGuiMenu : NewGuiMenu
    {
        protected ICoreGame Game { get; private set; }
        protected ICorePlayer Player { get; private set; }

        public CoreGuiMenu(INewGuiMenuScreen screen, int tabId, ICoreGame game, ICorePlayer player) : base(screen, tabId, game.TMGame, player.TMPlayer)
        {
            Game = game;
            Player = player;
        }
    }
}
