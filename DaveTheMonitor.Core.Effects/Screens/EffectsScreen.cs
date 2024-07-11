using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Gui;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Effects.Screens
{
    public sealed class EffectsScreen : CoreGuiMenu
    {
        public override string Name => "Effects";
        private Dictionary<ActorEffect, EffectWindow> _windows;
        private Window _container;

        protected override void InitWindows()
        {
            base.InitWindows();
            _windows = new Dictionary<ActorEffect, EffectWindow>();
            InitMainContainer();
        }

        private void InitMainContainer()
        {
            _container = new Window();
            _container.Colors = Window.TransparentColorProfile;
            _container.Name = "main_container";
            canvas.AddChild(_container);

            InitEffectWindows();

            SetContainerToCanvas(_container);
        }

        private void InitEffectWindows()
        {
            ICoreMod mod = EffectsPlugin.Instance.Mod;
            foreach (ActorEffect effect in Player.Effects())
            {
                AddEffectWindow(effect);
            }

            MoveWindows();
        }
        private void AddEffectWindow(ActorEffect effect)
        {
            EffectWindow win = new EffectWindow(effect, 0, 0, 600, 80);
            _container.AddChild(win);
            _windows.Add(effect, win);
        }

        private void MoveWindows()
        {
            int margin = 8;
            int y = 34;
            foreach (EffectWindow win in _windows.Values)
            {
                win.Position.Y = y;
                y += win.Size.Y + margin;
            }
        }

        protected override void UpdateCore()
        {
            try
            {
                foreach (EffectWindow win in _windows.Values)
                {
                    win.UpdateDuration();
                }
            }
            catch { }
        }

        protected override void OnOpen()
        {
            Player.Effects().EffectAdded += EffectAdded;
            Player.Effects().EffectRemoved += EffectRemoved;
        }

        protected override void OnClose()
        {
            Player.Effects().EffectAdded -= EffectAdded;
            Player.Effects().EffectRemoved -= EffectRemoved;
        }

        private void EffectAdded(object sender, ActorEffectEventArgs e)
        {
            AddEffectWindow(e.Effect);
            MoveWindows();
        }

        private void EffectRemoved(object sender, ActorEffectEventArgs e)
        {
            if (_windows.Remove(e.Effect, out EffectWindow win))
            {
                _container.RemoveChild(win);
            }
            MoveWindows();
        }

        public EffectsScreen(INewGuiMenuScreen screen, int tabId, ICoreGame game, ICorePlayer player) : base(screen, tabId, game, player)
        {

        }
    }
}
