using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// The main Particles plugin.
    /// </summary>
    [PluginEntry]
    public sealed class ParticlesPlugin : ICorePlugin
    {
        /// <summary>
        /// The main plugin instance of the Particles module.
        /// </summary>
        public static ParticlesPlugin Instance { get; private set; }
        /// <summary>
        /// The Mod that defines the Particles module.
        /// </summary>
        public ICoreMod Mod { get; private set; }
        internal Effect _particleShader;
        internal ICoreGame _game;
#if DEBUG
        private Texture2D _slice;
        private int _sliceDepth;
#endif
        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        /// <summary>
        /// IPlugin.Initialize implementation.
        /// </summary>
        public void Initialize(ICoreMod mod)
        {
            Mod = mod;
            Instance = this;
            InitializeDelegates();
            _particleShader = mod.Content.MGContent.Load<Effect>("Shaders/ParticleShader");

            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), Color.White),
                new VertexPositionColor(new Vector3(0, 0.5f, 0), Color.White),
                new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), Color.White),
            };
            _vertexBuffer = new VertexBuffer(CoreGlobals.GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
            _basicEffect = new BasicEffect(CoreGlobals.GraphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.TextureEnabled = false;
            _basicEffect.LightingEnabled = false;
        }

        /// <summary>
        /// IPlugin.InitializeGame implementation.
        /// </summary>
        public void InitializeGame(ICoreGame game)
        {
            _game = game;
            ParticleRegistry particleRegistry = new ParticleRegistry(game);
            game.SetDefaultData<ParticleGameData>().SetRegistry(particleRegistry);
            particleRegistry.RegisterAllTypesAndJson<JsonParticle>(game.ModManager.GetAllActiveMods(), "Particles");
            Texture3D texture = particleRegistry.BuildTextureAtlas(out Dictionary<ParticleDefinition, ParticleTextureInfo> info);
            particleRegistry.SetTexture(texture);
#if DEBUG
            _slice = new Texture2D(CoreGlobals.GraphicsDevice, 512, 512, false, SurfaceFormat.Color);
#endif

            foreach (KeyValuePair<ParticleDefinition, ParticleTextureInfo> item in info)
            {
                item.Key._textureInfo = item.Value;
            }

            if (game.ModManager.IsModActive("DaveTheMonitor.Debug"))
            {
                dynamic plugin = game.ModManager.GetMod("DaveTheMonitor.Debug").TMMod.Plugin;
                dynamic screen = plugin.AddDebugScreen(Mod.TMMod, "Particles");
                ParticleDebugScreen testScreen = new ParticleDebugScreen();
                testScreen.AddScreen(screen);
            }
        }

        public void InitializeWorld(ICoreWorld world)
        {
            world.SetDefaultData<ParticleWorldData>();
        }

        /// <summary>
        /// IPlugin.Draw implementation.
        /// </summary>
        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            ParticleManager particleManager = player.World.GetData<ParticleWorldData>().ParticleManager;
            particleManager.Update(virtualPlayer, true, true);
            particleManager.Draw(player, virtualPlayer, vp);

#if DEBUG
            if (_slice != null)
            {
                CoreGlobals.SpriteBatch.Begin();
                CoreGlobals.SpriteBatch.Draw(_slice, new Rectangle(0, 0, 512, 512), Color.White);
                CoreGlobals.SpriteBatch.End();
            }
#endif
        }

        /// <summary>
        /// IPlugin.Update implementation.
        /// </summary>
        public void Update()
        {
            
        }

        /// <summary>
        /// IPlugin.HandleInput implementation.
        /// </summary>
        public bool HandleInput(ICorePlayer player)
        {
#if DEBUG
            if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.Y))
            {
                Texture3D texture = _game.ParticleRegistry().Texture;
                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(0, 0, 0, 512, 512, _sliceDepth, _sliceDepth + 1, data, 0, data.Length);
                _slice.SetData(data);
                _sliceDepth++;
                if (_sliceDepth == texture.Depth)
                {
                    _sliceDepth = 0;
                }
                return true;
            }
#endif
            return false;
        }

        /// <summary>
        /// IPlugin.Unload implementation.
        /// </summary>
        public void Unload()
        {
            _game.ParticleRegistry().Texture.Dispose();
            foreach (ICoreWorld world in _game.GetAllWorlds())
            {
                world.ParticleManager().Dispose();
            }
        }

        /// <summary>
        /// IPlugin.HotLoad implementation.
        /// </summary>
        public void HotLoad()
        {
            Unload();
        }

        /// <summary>
        /// IPlugin.ModCall implementation.
        /// </summary>
        /// <remarks>
        /// <para>ParticleInstance "SpawnParticle" Vector3 position, Vector3 velocity</para>
        /// <para>void "DestroyParticle" int id</para>
        /// <para>void "DestroyParticleInstance" ParticleInstance instance</para>
        /// <para>void "DestroyAllParticles"</para>
        /// </remarks>
        public object ModCall(params object[] args)
        {
            if (args == null || args.Length == 0 || args[0] is not string arg1)
            {
                return null;
            }

            switch (arg1)
            {
                case "SpawnParticle":
                {
                    if (!ParamHelper.VerifyArgs(args, out string _, out ICoreWorld world, out string id, out Vector3 position, out Vector3 velocity))
                    {
                        return null;
                    }

                    return world.ParticleManager().SpawnParticle(id, position, velocity);
                }
                case "DestroyParticle":
                {
                    if (!ParamHelper.VerifyArgs(args, out string _, out ICoreWorld world, out int id))
                    {
                        return null;
                    }

                    world.ParticleManager().DestroyParticle(id);
                    return null;
                }
                case "DestroyParticleInstance":
                {
                    if (!ParamHelper.VerifyArgs(args, out string _, out ICoreWorld world, out ParticleInstance particle))
                    {
                        return null;
                    }

                    world.ParticleManager().DestroyParticle(particle);
                    return null;
                }
                case "DestroyAllParticles":
                {
                    if (!ParamHelper.VerifyArgs(args, out string _, out ICoreWorld world))
                    {
                        return null;
                    }

                    world.ParticleManager().DestroyAllParticles();
                    return null;
                }
                case "GetSpawnParticleDelegate":
                {
                    return SpawnParticle;
                }
                case "GetSpawnParticleDefinitionDelegate":
                {
                    return SpawnParticleDefinition;
                }
                case "GetDestroyParticleDelegate":
                {
                    return DestroyParticle;
                }
                case "GetDestroyParticleInstanceDelegate":
                {
                    return DestroyParticleInstance;
                }
                case "GetDestroyAllParticlesDelegate":
                {
                    return DestroyAllParticles;
                }
                case "GetActiveParticlesDelegate":
                {
                    return ActiveParticles;
                }
                case "GetMaxParticlesDelegate":
                {
                    return MaxParticles;
                }
            }

            return null;
        }

        #region Delegates

        /// <summary>
        /// Delegate that calls <see cref="ParticleManager.SpawnParticle(string, Vector3, Vector3)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to call the method, but can't reference the assembly.</para>
        /// </remarks>
        public Func<ICoreWorld, string, Vector3, Vector3, object> SpawnParticle { get; private set; }
        /// <summary>
        /// Delegate that calls <see cref="ParticleManager.SpawnParticle(ParticleDefinition, Vector3, Vector3)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to call the method, but can't reference the assembly.</para>
        /// </remarks>
        public Func<ICoreWorld, object, Vector3, Vector3, object> SpawnParticleDefinition { get; private set; }
        /// <summary>
        /// Delegate that calls <see cref="ParticleManager.DestroyParticle(int)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to call the method, but can't reference the assembly.</para>
        /// </remarks>
        public Action<ICoreWorld, int> DestroyParticle { get; private set; }
        /// <summary>
        /// Delegate that calls <see cref="ParticleManager.DestroyParticle(ParticleInstance)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to call the method, but can't reference the assembly.</para>
        /// </remarks>
        public Action<ICoreWorld, object> DestroyParticleInstance { get; private set; }
        /// <summary>
        /// Delegate that calls <see cref="ParticleManager.DestroyAllParticles()"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to call the method, but can't reference the assembly.</para>
        /// </remarks>
        public Action<ICoreWorld> DestroyAllParticles { get; private set; }
        /// <summary>
        /// Delegate that returns <see cref="ParticleManager.ActiveParticles"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to get the property, but can't reference the assembly.</para>
        /// </remarks>
        public Func<ICoreWorld, int> ActiveParticles { get; private set; }
        /// <summary>
        /// Delegate that returns <see cref="ParticleManager.MaxParticles"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this when you need a performant way to get the property, but can't reference the assembly.</para>
        /// </remarks>
        public Func<ICoreWorld, int> MaxParticles { get; private set; }

        private void InitializeDelegates()
        {
            SpawnParticle = SpawnParticleMethod;
            SpawnParticleDefinition = SpawnParticleMethod;
            DestroyParticle = DestroyParticleMethod;
            DestroyParticleInstance = DestroyParticleMethod;
            DestroyAllParticles = DestroyAllParticlesMethod;
            ActiveParticles = ActiveParticlesMethod;
            MaxParticles = MaxParticlesMethod;
        }

        private object SpawnParticleMethod(ICoreWorld world, string id, Vector3 position, Vector3 velocity)
        {
            return world.ParticleManager().SpawnParticle(id, position, velocity);
        }

        private object SpawnParticleMethod(ICoreWorld world, object definition, Vector3 position, Vector3 velocity)
        {
            return world.ParticleManager().SpawnParticle((ParticleDefinition)definition, position, velocity);
        }

        private void DestroyParticleMethod(ICoreWorld world, int id)
        {
            world.ParticleManager().DestroyParticle(id);
        }

        private void DestroyParticleMethod(ICoreWorld world, object particle)
        {
            world.ParticleManager().DestroyParticle((ParticleInstance)particle);
        }

        private void DestroyAllParticlesMethod(ICoreWorld world)
        {
            world.ParticleManager().DestroyAllParticles();
        }

        private int ActiveParticlesMethod(ICoreWorld world)
        {
            return world.ParticleManager().ActiveParticles;
        }

        private int MaxParticlesMethod(ICoreWorld world)
        {
            return world.ParticleManager().MaxParticles;
        }

        #endregion
    }
}
