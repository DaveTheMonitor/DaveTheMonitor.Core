using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Gui;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Effects.Screens
{
    public sealed class EffectsScreen : CoreGuiMenu
    {
        private static Window.ColorProfile _windowColors;
        public override string Name => "Effects";
        private Window mainContainer;

        static EffectsScreen()
        {
            _windowColors = Colors.ButtonColors.Clone();
            _windowColors.BackHoverColor = _windowColors.BackColor;
            _windowColors.BackClickColor = _windowColors.BackColor;
        }

        protected override void InitWindows()
        {
            base.InitWindows();
            InitMainContainer();
        }

        private void InitMainContainer()
        {
            mainContainer = new Window();
            mainContainer.Colors = Window.TransparentColorProfile;
            mainContainer.Name = "main_container";
            canvas.AddChild(mainContainer);

            InitEffectWindows();

            SetContainerToCanvas(mainContainer);
        }

        private void InitEffectWindows()
        {
            int margin = 8;
            int width = 600;
            int height = 80;
            int i = 0;
            int startY = 34;
            ICoreMod mod = EffectsPlugin.Instance.Mod;
            foreach (ActorEffect effect in Player.Effects())
            {
                Window win = CreateEffectWindow(effect, 0, startY + (i * height) + (i * margin), width, height);
                mainContainer.AddChild(win);
                i++;
            }
        }

        private Window CreateEffectWindow(ActorEffect effect, int x, int y, int width, int height)
        {
            ActorEffectDefinition def = effect.Definition;
            int margin = 8;
            int cX = margin;
            int cY = margin;

            Window container = new Window(0, y, width, height);
            container.Colors = _windowColors;
            container.BorderThickness = 0;

            int iconSize = height - margin - margin;
#if DEBUG
            CorePlugin.Log(effect.Definition.BackgroundTexture?.ToString() ?? "Null");
#endif
            Window background = new Window(cX, cY, iconSize, iconSize);
            background.Colors = Window.TransparentColorProfile;
            background.LoadTexture(effect.Definition.BackgroundTexture, true, true, 1);
            background.Texture.SrRect = def.BackgroundSrc;
            container.AddChild(background);

            Window icon = new Window(0, 0, background.Size.X, background.Size.Y);
            icon.Colors = Window.TransparentColorProfile;
            icon.LoadTexture(effect.Definition.IconTexture, true, true, 1);
            icon.Texture.SrRect = def.IconSrc;
            background.AddChild(icon);

            cX += iconSize + margin;
            SpriteFont font = CoreGlobals.GameFont16;

            Vector2 measure = font.MeasureString(def.Name);
            TextBox textBox = new TextBox(def.Name, cX, cY, width - cX - margin, (int)measure.Y, 1, WinTextAlignX.Left, WinTextAlignY.Center);
            textBox.Text = def.Name;
            textBox.Font = font;
            textBox.Colors = Colors.BlackText;
            container.AddChild(textBox);

            font = CoreGlobals.GameFont12;
            cY += (int)measure.Y;

            measure = font.MeasureString(def.Description);
            textBox = new TextBox(def.Description, cX, cY, width - cX - margin, (int)measure.Y, 1, WinTextAlignX.Left, WinTextAlignY.Center);
            textBox.Text = def.Description;
            textBox.Font = font;
            textBox.Colors = Colors.BlackText;
            container.AddChild(textBox);

            return container;
        }

        public EffectsScreen(INewGuiMenuScreen screen, int tabId, ICoreGame game, ICorePlayer player) : base(screen, tabId, game, player)
        {

        }
    }
}
