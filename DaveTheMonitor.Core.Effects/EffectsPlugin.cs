using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Effects.Screens;
using DaveTheMonitor.Core.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;
using System.Linq;

namespace DaveTheMonitor.Core.Effects
{
    [PluginEntry]
    public sealed class EffectsPlugin : ICorePlugin
    {
        private struct DurationEffectToDraw
        {
            public ActorEffect Effect;
            public Rectangle Dest;

            public DurationEffectToDraw(ActorEffect effect, Rectangle dest)
            {
                Effect = effect;
                Dest = dest;
            }
        }
        public static EffectsPlugin Instance { get; private set; }
        public ICoreMod Mod { get; private set; }
        private ICoreGame _game;
        private Effect _durationShader;
        private EffectParameter _durationBackgroundTexture;
        private EffectParameter _durationIconTexture;
        private List<DurationEffectToDraw> _durationEffectsToDraw;
        private ActorEffectVertex[] _vertices;

        public void Initialize(ICoreMod mod)
        {
            Mod = mod;
            Instance = this;
            _durationShader = mod.Content.MGContent.Load<Effect>("Shaders/EffectDurationShader");
            _durationBackgroundTexture = _durationShader.Parameters["BackgroundTexture"];
            _durationIconTexture = _durationShader.Parameters["IconTexture"];
            _vertices = new ActorEffectVertex[6];
            _durationEffectsToDraw = new List<DurationEffectToDraw>();
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
            if (!actor.IsActive)
            {
                return;
            }

            actor.Effects().Update();
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            if (_game.TryGetPlayer(virtualPlayer, out ICorePlayer virt))
            {
                _durationEffectsToDraw.Clear();
                EffectData data = virt.Effects();
                if (data.Effects == 0)
                {
                    return;
                }

                int i = 0;
                SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                foreach (ActorEffect effect in data)
                {
                    ActorEffectDefinition def = effect.Definition;
                    if (!def.ShouldDisplay)
                    {
                        continue;
                    }

                    Texture2D icon = def.IconTexture;
                    Texture2D bg = def.BackgroundTexture;
                    Rectangle iconSrc = def.GetIconSrc(effect);
                    Rectangle bgSrc = def.GetBackgroundSrc(effect);

                    int iconSize = 64;
                    Rectangle hudBounds = player.HudBounds;
                    float x = hudBounds.X + hudBounds.Width - iconSize;
                    float y = hudBounds.Y + (i * iconSize) + (i * 4);

                    Rectangle dest = new Rectangle((int)x, (int)y, iconSize, iconSize);

                    spriteBatch.Draw(bg, dest, bgSrc, Color.White);
                    spriteBatch.Draw(icon, dest, bgSrc, Color.White);
                    if (effect.Duration != -1)
                    {
                        _durationEffectsToDraw.Add(new DurationEffectToDraw(effect, dest));
                    }
                    i++;
                }
                spriteBatch.End();

                if (_durationEffectsToDraw.Count == 0)
                {
                    return;
                }
                
                // We use our own vertices here because we need to
                // use float positions/texcoords and SpriteBatch
                // only supports ints.
                GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
                graphicsDevice.BlendState = BlendState.AlphaBlend;
                graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                graphicsDevice.DepthStencilState = DepthStencilState.None;
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                _durationShader.Parameters["ScreenSize"].SetValue(graphicsDevice.Viewport.Bounds.Size.ToVector2());
                foreach (DurationEffectToDraw durationEffect in _durationEffectsToDraw)
                {
                    ActorEffect effect = durationEffect.Effect;
                    Texture2D bg = durationEffect.Effect.Definition.BackgroundTexture;
                    Rectangle bgSrc = durationEffect.Effect.Definition.GetBackgroundSrc(effect);
                    Texture2D icon = durationEffect.Effect.Definition.IconTexture;
                    Rectangle iconSrc = durationEffect.Effect.Definition.GetIconSrc(effect);
                    float progress = effect.Age / effect.Duration;

                    Vector2 bgTl = new Vector2((float)bgSrc.Top / bg.Height, (float)bgSrc.Left / bg.Width);
                    Vector2 bgBr = new Vector2((float)bgSrc.Bottom / bg.Height, (float)bgSrc.Right / bg.Width);
                    Vector2 iconTl = new Vector2((float)iconSrc.Top / icon.Height, (float)iconSrc.Left / icon.Width);
                    Vector2 iconBr = new Vector2((float)iconSrc.Bottom / icon.Height, (float)iconSrc.Right / icon.Width);
                    Vector4 dest = new Vector4(durationEffect.Dest.X, durationEffect.Dest.Y, durationEffect.Dest.Width, durationEffect.Dest.Height);

                    bgBr.Y = MathHelper.Lerp(bgTl.Y, bgBr.Y, progress);
                    iconBr.Y = MathHelper.Lerp(iconTl.Y, iconBr.Y, progress);
                    dest.W = MathHelper.Lerp(0, dest.W, progress);

                    _vertices[0] = new ActorEffectVertex(new Vector2(dest.X, dest.Y + dest.W), new Vector2(bgTl.X, bgBr.Y), new Vector2(iconTl.X, iconBr.Y));
                    _vertices[1] = new ActorEffectVertex(new Vector2(dest.X, dest.Y), bgTl, iconTl);
                    _vertices[2] = new ActorEffectVertex(new Vector2(dest.X + dest.Z, dest.Y + dest.W), bgBr, iconBr);
                    _vertices[3] = new ActorEffectVertex(new Vector2(dest.X + dest.Z, dest.Y + dest.W), bgBr, iconBr);
                    _vertices[4] = new ActorEffectVertex(new Vector2(dest.X, dest.Y), bgTl, iconTl);
                    _vertices[5] = new ActorEffectVertex(new Vector2(dest.X + dest.Z, dest.Y), new Vector2(bgBr.X, bgTl.Y), new Vector2(iconBr.X, iconTl.Y));

                    _durationBackgroundTexture.SetValue(bg);
                    _durationIconTexture.SetValue(icon);
                    _durationShader.CurrentTechnique.Passes[0].Apply();
                    graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 2);
                }
            }
        }
    }
}
