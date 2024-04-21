using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Events;
using DaveTheMonitor.Core.Patches;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core.Biomes
{
    [PluginEntry]
    public sealed class BiomesPlugin : ICorePlugin
    {
        public static BiomesPlugin Instance { get; private set; }
        public ICoreMod Mod { get; private set; }
        private ICoreGame _game;
        private PatchHelper _patchHelper;
        private Texture2D _biomeTex;
        private Texture2D _precTex;
        private Texture2D _tempTex;
        private Texture2D _tex;

        public void Initialize(ICoreMod mod)
        {
            Mod = mod;
            Instance = this;
            _patchHelper = new PatchHelper("DaveTheMonitor.Core.Biomes");
            _patchHelper.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void InitializeGame(ICoreGame game)
        {
            _game = game;
            game.GameShader.AddFogColorModifier(FogColorModifier);
            game.GameShader.AddFogStartModifier(FogStartModifier);
            game.GameShader.AddFogEndModifier(FogEndModifier);
            game.GameShader.AddLanturnColorModifier(LanturnColorModifier);
            game.GameShader.AddLanturnRangeModifier(LanturnRangeModifier);

            BiomeRegistry biomes = new BiomeRegistry(game);
            DecorationRegistry decorations = new DecorationRegistry(game);
            biomes.RegisterDefinition(new OceanBiome(), Mod);
            biomes.RegisterDefinition(new DefaultBiome(Color.LightGreen), Mod);
            //biomes.RegisterDefinition(new TestBiome("Forest", 0.5f, 0.8f, Color.LightGreen, Block.Grass), Mod);
            //biomes.RegisterDefinition(new TestBiome("Beach", 0.5f, 1f, Color.LightYellow, Block.Sand), Mod);
            //biomes.RegisterDefinition(new TestBiome("Terra", 0.5f, 0.15f, Color.Green, Block.ColorGreen), Mod);
            //biomes.RegisterDefinition(new MountainBiome(), Mod);
            //biomes.RegisterDefinition(new TestBiome("Aestus", 1f, 0.2f, Color.Red, Block.ColorRed), Mod);
            biomes.RegisterDefinition(new GlacierBiome(), Mod);
            decorations.RegisterDefinition(new TestDecoration(), Mod);
            decorations.RegisterDefinition(new BasicDecoration("Core.SnowBoulderSmall1", "SnowBoulderSmall1", true, new GlobalPoint3D(1, 1, 1), Map.CopyType.Merge), Mod);
            decorations.RegisterDefinition(new BasicDecoration("Core.SnowBoulder1", "SnowBoulder1", true, new GlobalPoint3D(2, 2, 2), Map.CopyType.Merge), Mod);
            decorations.RegisterDefinition(new BasicDecoration("Core.SnowBoulderLarge1", "SnowBoulderLarge1", true, new GlobalPoint3D(4, 3, 4), Map.CopyType.Merge), Mod);
            game.SetDefaultData<BiomeGameData>(Mod).SetRegisters(biomes, decorations);
        }

        private void LanturnColorModifier(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector3 color)
        {
            if (virtualPlayer.IsItemEquipped(Item.MultiTextureBlock2))
            {
                color = Color.DarkGreen.ToVector3();
            }
        }

        private void LanturnRangeModifier(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            if (virtualPlayer.IsItemEquipped(Item.MultiTextureBlock2))
            {
                value = 100;
            }
        }

        private void FogColorModifier(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value)
        {
            ICorePlayer p = player.Game.GetPlayer(virtualPlayer) ?? player;
            BiomeActorData biomeData = p.BiomeData();
            if (!biomeData.HasFog)
            {
                return;
            }

            Vector4 color = biomeData.FogColor;
            if (value != Vector4.Zero)
            {
                Vector3 orig = new Vector3(value.X, value.Y, value.Z);
                float blend = Math.Min((value.W / color.W) * 0.5f, 1);
                Vector3 merged = Vector3.Lerp(new Vector3(color.X, color.Y, color.Z), orig, blend);
                value = new Vector4(merged, Math.Max(value.W, color.W));
            }
            else
            {
                value = color;
            }
        }

        private void FogStartModifier(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            ICorePlayer p = player.Game.GetPlayer(virtualPlayer) ?? player;
            BiomeActorData biomeData = p.BiomeData();
            if (!biomeData.HasFog)
            {
                return;
            }

            value = Math.Min(value, -20);
        }

        private void FogEndModifier(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            ICorePlayer p = player.Game.GetPlayer(virtualPlayer) ?? player;
            BiomeActorData biomeData = p.BiomeData();
            if (!biomeData.HasFog)
            {
                return;
            }

            float distance = biomeData.FogDistance;
            value = Math.Min(value, distance);
        }

        private void ActorAdded(object sender, CoreActorEventArgs e)
        {
            e.Actor.SetDefaultData<BiomeActorData>(Mod);
        }

        public void InitializeWorld(ICoreWorld world)
        {
            world.ActorManager.ActorAdded += ActorAdded;
            BiomeManager biomeManager = world.SetDefaultData<BiomeWorldData>(Mod).BiomeManager;
            foreach (ICoreMod mod in _game.ModManager.GetAllActiveMods())
            {
                //LoadBiomes(mod);
            }

#if DEBUG
            CorePlugin.Log("Generating biome map");
#endif
            biomeManager.GenerateBiomeMap();
#if DEBUG
            CorePlugin.Log("Biome map generated");
#endif
            GetTextures(biomeManager);
        }

        private void GetTextures(BiomeManager biomeManager)
        {
            List<Color> tempData = new List<Color>();
            List<Color> precData = new List<Color>();
            List<Color> biomeData = new List<Color>();
            int mapSize = _game.TMGame.World.Header.MaxMapSize.X;
            BoxInt bounds = ((Map)_game.TMGame.World.Map).MapBound;
            int offsetX = _game.TMGame.World.Header.BiomeParams.OffsetX;
            int offsetZ = _game.TMGame.World.Header.BiomeParams.OffsetZ;
            for (int z = bounds.Min.Z; z < bounds.Max.Z; z++)
            {
                for (int x = bounds.Min.X; x < bounds.Max.Z; x++)
                {
                    float temp = biomeManager.GetTemperature(x - bounds.Min.X, z - bounds.Min.Z);
                    float prec = biomeManager.GetPrecipitation(x + offsetX, z + offsetZ);
                    Biome biome = biomeManager.GetBiome(x, z);
                    tempData.Add(new Color(temp, temp, temp));
                    precData.Add(new Color(prec, prec, prec));
                    biomeData.Add(biome.Color);
                }
            }

            _tempTex ??= new Texture2D(CoreGlobals.GraphicsDevice, mapSize, mapSize);
            _tempTex.SetData(tempData.ToArray());

            _precTex ??= new Texture2D(CoreGlobals.GraphicsDevice, mapSize, mapSize);
            _precTex.SetData(precData.ToArray());

            _biomeTex ??= new Texture2D(CoreGlobals.GraphicsDevice, mapSize, mapSize);
            _biomeTex.SetData(biomeData.ToArray());
        }

        internal void SaveTextures(string path)
        {
            using Stream tempStream = File.Create(Path.Combine(path, "temp.png"));
            _tempTex.SaveAsPng(tempStream, _tempTex.Width, _tempTex.Height);

            using Stream precStream = File.Create(Path.Combine(path, "prec.png"));
            _precTex.SaveAsPng(precStream, _precTex.Width, _precTex.Height);

            using Stream biomeStream = File.Create(Path.Combine(path, "biome.png"));
            _biomeTex.SaveAsPng(biomeStream, _biomeTex.Width, _biomeTex.Height);
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;

            if (_tex != null)
            {
                int min = Math.Min(vp.Width, vp.Height);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                spriteBatch.Draw(_tex, new Rectangle((vp.Width - min) / 2, (vp.Height - min) / 2, min, min), Color.White);
                spriteBatch.End();
            }
        }

        public bool HandleInput(ICorePlayer player)
        {
            if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.OemComma))
            {
                _tex = _tempTex;
                return true;
            }
            else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.OemPeriod))
            {
                _tex = _precTex;
                return true;
            }
            else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.OemQuestion))
            {
                _tex = _biomeTex;
                return true;
            }
            else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.RightShift))
            {
                _tex = null;
                return true;
            }
            else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.OemPlus))
            {
                BiomeParams param = player.World.Header.BiomeParams;
                int offsetX = param.OffsetX;
                int offsetZ = param.OffsetZ;
                int x = (int)MathF.Floor(player.Position.X);
                int z = (int)MathF.Floor(player.Position.Z);
                float prec = player.World.BiomeManager().GetPrecipitation(x + offsetX, z + offsetZ);
                Biome biome = player.World.BiomeManager().GetBiome(x, z, out Biome border, out float blend);

                Map map = (Map)player.World.Map.TMMap;
                GlobalPoint3D p = new GlobalPoint3D(x, (int)player.Position.Y, z);
                ushort height = map.GetHeight(p);
                ushort lightHeight = map.GetHeightForLighting(p);

                _game.AddNotification($"Prec: {prec}, Biome: {biome.Id}, Border: {border?.Id ?? "None"}, Blend: {blend}, GH: {player.World.BiomeManager().GetGroundHeight(x, z)}, H: {height}, LH: {lightHeight}");

                //byte aux = player.World.Map.GetAuxFullData(p + GlobalPoint3D.Down);
                player.World.Map.SetAuxData(p + GlobalPoint3D.Down, 1, UpdateBlockMethod.Player, player.Id, false);
                player.World.Map.Commit();
                //_game.AddNotification(aux.ToString());
                return true;
            }
            else if (InputManager.IsKeyPressedNew(player.PlayerIndex, Keys.Q))
            {
                player.World.TMEnvironmentManager.AddFog(new GlobalPoint3D(player.Position), 500, 5, 30, false);
            }
            return false;
        }

        public void Update(ICoreActor actor)
        {
            actor.GetData<BiomeActorData>(Mod).Update();
        }

        public void UnloadMod()
        {
            _patchHelper.Unpatch();
        }

        public void HotLoadMod()
        {
            UnloadMod();
        }
    }
}
