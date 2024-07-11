using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.GUI;

namespace DaveTheMonitor.Core.Effects.Screens
{
    internal sealed class EffectWindow : Window
    {
        private static ColorProfile _windowColors;
        private static TextBox.ColorProfile _buffProgressBarColors;
        private static TextBox.ColorProfile _debuffProgressBarColors;
        private static TextBox.ColorProfile _neutralProgressBarColors;
        private ActorEffect _effect;
        private ProgressBar _durationBar;
        private int _prevDuration;
        private int _prevAge;

        static EffectWindow()
        {
            _windowColors = StudioForge.TotalMiner.Colors.ButtonColors.Clone();
            _windowColors.BackHoverColor = _windowColors.BackColor;
            _windowColors.BackClickColor = _windowColors.BackColor;

            _buffProgressBarColors = ProgressBar.DefaultColorProfile.Clone();
            _buffProgressBarColors.BackColor = _windowColors.BackColor * 0.85f;
            _buffProgressBarColors.BackColor.A = 255;
            _buffProgressBarColors.BackHoverColor = _buffProgressBarColors.BackColor;
            _buffProgressBarColors.BackClickColor = _buffProgressBarColors.BackColor;
            _buffProgressBarColors.ForeColor = new Color(124, 179, 251);

            _debuffProgressBarColors = _buffProgressBarColors.Clone();
            _debuffProgressBarColors.ForeColor = new Color(247, 109, 119);

            _neutralProgressBarColors = _buffProgressBarColors.Clone();
            _neutralProgressBarColors.ForeColor = new Color(217, 217, 217);
        }

        public void UpdateDuration()
        {
            int duration = (int)_effect.Duration;
            int age = (int)_effect.Age;
            if (duration != _prevDuration || age != _prevAge)
            {
                _durationBar.Text = GetDurationString();
            }
            _durationBar.progress = GetProgress();
            _prevDuration = duration;
            _prevAge = age;
        }

        private string GetDurationString()
        {
            float totalTime = _effect.Duration - _effect.Age;
            int minutes = (int)(totalTime / 60);
            int seconds = (int)(totalTime % 60);
            return minutes > 0 ? $"{minutes}m {seconds}s" : $"{seconds}s";
        }

        private float GetProgress()
        {
            return 1 - (_effect.Age / _effect.Duration);
        }

        private void InitWindows()
        {
            ActorEffectDefinition def = _effect.Definition;
            int margin = 8;
            int x = margin;
            int y = margin;

            int iconSize = Size.Y - margin - margin;
#if DEBUG
            CorePlugin.Log(_effect.Definition.BackgroundTexture?.ToString() ?? "Null");
#endif
            Window background = new Window(x, y, iconSize, iconSize);
            background.Colors = TransparentColorProfile;
            background.LoadTexture(_effect.Definition.BackgroundTexture, true, true, 1);
            background.Texture.SrRect = def.GetBackgroundSrc(_effect);
            AddChild(background);

            Window icon = new Window(0, 0, background.Size.X, background.Size.Y);
            icon.Colors = TransparentColorProfile;
            icon.LoadTexture(_effect.Definition.IconTexture, true, true, 1);
            icon.Texture.SrRect = def.GetIconSrc(_effect);
            background.AddChild(icon);

            x += iconSize + margin;
            SpriteFont font = CoreGlobals.GameFont16;

            string name = def.GetName(_effect);
            Vector2 measure = font.MeasureString(name);
            TextBox textBox = new TextBox(name, x, y, Size.X - x - margin, (int)measure.Y, 1, WinTextAlignX.Left, WinTextAlignY.Center);
            textBox.Font = font;
            textBox.Colors = StudioForge.TotalMiner.Colors.BlackText;
            AddChild(textBox);

            font = CoreGlobals.GameFont12;
            y += (int)measure.Y;

            string desc = def.GetDescription(_effect);
            measure = font.MeasureString(desc);
            textBox = new TextBox(desc, x, y, Size.X - x - margin, (int)measure.Y, 1, WinTextAlignX.Left, WinTextAlignY.Center);
            textBox.Font = font;
            textBox.Colors = StudioForge.TotalMiner.Colors.BlackText;
            AddChild(textBox);

            y = Size.Y - margin - 20;
            _durationBar = new ProgressBar(x, y, Size.X - x - margin, 20);
            _durationBar.Font = font;
            _durationBar.TextAlignX = WinTextAlignX.Center;
            _durationBar.TextAlignY = WinTextAlignY.Center;
            _durationBar.Text = GetDurationString();
            _durationBar.progress = GetProgress();
            _durationBar.Colors = _effect.Definition.Type switch
            {
                ActorEffectType.Buff => _buffProgressBarColors,
                ActorEffectType.Debuff => _debuffProgressBarColors,
                _ => _neutralProgressBarColors
            };
            _durationBar.BorderThickness = 2;
            AddChild(_durationBar);
        }

        public EffectWindow(ActorEffect effect, int x, int y, int width, int height) : base(x, y, width, height)
        {
            Colors = _windowColors;
            BorderThickness = 0;
            _effect = effect;
            InitWindows();
        }
    }
}
