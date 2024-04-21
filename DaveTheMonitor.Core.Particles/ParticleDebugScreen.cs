using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner.API;
using System;

namespace DaveTheMonitor.Core.Particles
{
    internal sealed class ParticleDebugScreen
    {
        private (int Count, int Rendered, string Text) _active;
        private (int Count, int Chunks, string Text) _max;
        private (int Count, string Text) _definitions;
        private (long Amount, string Text) _vertexRam;
        private (Vector3 Velocity, float Factor, string Text) _wind;

        public string UpdateActive(ITMGame game, ITMPlayer player)
        {
            ICorePlayer corePlayer = ParticlesPlugin.Instance._game.GetPlayer(player);
            ParticleManager particleManager = corePlayer.World.ParticleManager();
            int count = particleManager.ActiveParticles;
            int rendered = particleManager.RenderedParticles;
            if (_active.Count != count || _active.Rendered != rendered)
            {
                _active.Count = count;
                _active.Rendered = rendered;
                _active.Text = $"Active Particles: {count}, R: {rendered}";
            }
            return _active.Text;
        }

        public string UpdateMax(ITMGame game, ITMPlayer player)
        {
            ICorePlayer corePlayer = ParticlesPlugin.Instance._game.GetPlayer(player);
            ParticleManager particleManager = corePlayer.World.ParticleManager();
            int count = particleManager.MaxParticles;
            int chunks = particleManager.ChunkCount;
            if (_max.Count != count || _max.Chunks != chunks )
            {
                _max.Count = count;
                _max.Chunks = chunks;
                _max.Text = $"Max Particles: {count}, C: {chunks}";
            }
            return _max.Text;
        }

        public string UpdateDefinitions(ITMGame game, ITMPlayer player)
        {
            ICorePlayer corePlayer = ParticlesPlugin.Instance._game.GetPlayer(player);
            int count = corePlayer.Game.ParticleRegistry().Definitions;
            if (_definitions.Count != count)
            {
                _definitions.Count = count;
                _definitions.Text = $"Particle Definitions: {count}";
            }
            return _definitions.Text;
        }

        public string UpdateVertexRam(ITMGame game, ITMPlayer player)
        {
            ICorePlayer corePlayer = ParticlesPlugin.Instance._game.GetPlayer(player);
            ParticleManager particleManager = corePlayer.World.ParticleManager();
            int per = particleManager.GetVertexBytes();
            long amount = particleManager.GetBufferSize();
            if (_vertexRam.Amount != amount)
            {
                _vertexRam.Amount = amount;
                _vertexRam.Text = $"Particles Vertex Ram: {(amount / 1000d):0.0} KB ({per} B/p)";
            }
            return _vertexRam.Text;
        }

        public string UpdateWind(ITMGame game, ITMPlayer player)
        {
            Vector3 velocity = game.GetWindVelocity();
            float factor = game.GetWindFactor();
            if (_wind.Velocity != velocity || _wind.Factor != factor)
            {
                _wind.Velocity = velocity;
                _wind.Factor = factor;
                _wind.Text = $"Wind: {velocity.X:0.000}, {velocity.Y:0.000}, {velocity.Z:0.000}: {factor:0.000}";
            }
            return _wind.Text;
        }

        public void AddScreen(dynamic screen)
        {
            screen.AddItem(new Func<ITMGame, ITMPlayer, string>(UpdateActive));
            screen.AddItem(new Func<ITMGame, ITMPlayer, string>(UpdateMax));
            screen.AddItem(new Func<ITMGame, ITMPlayer, string>(UpdateDefinitions));
            screen.AddItem(new Func<ITMGame, ITMPlayer, string>(UpdateVertexRam));
            screen.AddItem(new Func<ITMGame, ITMPlayer, string>(UpdateWind));
        }

        public ParticleDebugScreen()
        {
            _active.Text = "Active Particles: 0, R: 0";
            _max.Text = "Max Particles: 0";
            _definitions.Text = "Particle Definitions: 0";
            _vertexRam.Text = "Particles Vertex Ram: 0.0 KB (0 B/p)";
            _wind.Text = "Wind: 0.000, 0.000, 0.000: 0.000";
        }
    }
}
