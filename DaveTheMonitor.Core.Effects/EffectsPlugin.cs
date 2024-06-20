using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Effects.Screens;
using DaveTheMonitor.Core.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Effects
{
    [PluginEntry]
    public sealed class EffectsPlugin : ICorePlugin
    {
        public static EffectsPlugin Instance { get; private set; }
        public ICoreMod Mod { get; private set; }
        private ICoreGame _game;

        public void Initialize(ICoreMod mod)
        {
            Mod = mod;
            Instance = this;
        }

        public void InitializeGame(ICoreGame game)
        {
            _game = game;
            ActorEffectRegistry registry = new ActorEffectRegistry(game);
            game.SetDefaultData<EffectGameData>().SetRegistry(registry);
            registry.RegisterAllTypesAndJson<JsonActorEffect>(game.ModManager.GetAllActiveMods(), "Effects");
        }

        public bool HandleInput(ICorePlayer player)
        {
            if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.V))
            {
                EffectsScreen screen = new EffectsScreen(null, -1, _game, player);
                _game.TMGame.OpenPauseMenu(player.TMPlayer, new NewGuiMenu[] { screen }, false);
                return true;
            }
            return false;
        }

        public void Update(ICoreActor actor)
        {
            actor.Effects().Update();
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            if (_game.TryGetPlayer(virtualPlayer, out ICorePlayer virt))
            {
                EffectData data = virt.Effects();
                int i = 0;
                foreach (ActorEffect effect in data)
                {
                    ActorEffectDefinition def = effect.Definition;
                    if (!def.ShouldDisplay)
                    {
                        continue;
                    }

                    Texture2D icon = def.IconTexture;
                    Texture2D bg = def.BackgroundTexture;
                    Rectangle iconSrc = def.IconSrc;
                    Rectangle bgSrc = def.BackgroundSrc;

                    int iconSize = 64;
                    Rectangle hudBounds = player.HudBounds;
                    float x = hudBounds.X + hudBounds.Width - iconSize;
                    float y = hudBounds.Y + (i * iconSize) + (i * 4);

                    Rectangle dest = new Rectangle((int)x, (int)y, iconSize, iconSize);

                    SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                    spriteBatch.Draw(bg, dest, bgSrc, Color.White);
                    spriteBatch.Draw(icon, dest, bgSrc, Color.White);
                    if (effect.Duration != -1)
                    {
                        float progress = effect.Age / effect.Duration;
                        int height = iconSize - (int)(iconSize * (1 - progress));
                        Rectangle boxDest = new Rectangle((int)x, (int)y, iconSize, height);
                        spriteBatch.DrawFilledBox(boxDest, 0, Color.Black * 0.75f, Color.Black * 0.75f);
                    }
                    spriteBatch.End();
                    i++;
                }
            }
        }
    }
}
