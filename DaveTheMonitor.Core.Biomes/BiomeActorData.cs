using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System.IO;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BiomeActorData : ICoreData<ICoreActor>
    {
        public ICoreActor Actor { get; private set; }
        public bool ShouldSave => false;
        public Biome CurrentBiome { get; private set; }
        public Biome PrevBiome { get; private set; }
        public float TimeInBiome { get; private set; }
        public bool HasFog => FogColor != Vector4.Zero;
        public Vector4 FogColor { get; private set; }
        public float FogDistance { get; private set; }
        private Vec4Interpolator _fogColorInterpolator;
        private FloatInterpolator _fogDistanceInterpolator;
        private IHasMovement _particle;

        public void Initialize(ICoreActor actor)
        {
            Actor = actor;
            if (actor.IsPlayer)
            {
                _fogColorInterpolator = new Vec4Interpolator();
                _fogDistanceInterpolator = new FloatInterpolator();
            }
        }

        private void SetBiome(Biome biome)
        {
            if (biome == CurrentBiome)
            {
                return;
            }
#if DEBUG
            if (Actor.IsPlayer)
            {
                CorePlugin.Log($"{Actor.Name} entered biome {biome.Id}");
            }
#endif
            if (Actor.IsPlayer)
            {
                ICorePlayer player = (ICorePlayer)Actor;
                if (player.IsLocalPlayer)
                {
                    if (_particle != null)
                    {
                        player.World.DestroyParticle(_particle);
                        _particle = null;
                    }

                    if (biome.ParticleEmitter != null)
                    {
                        player.World.SpawnParticle(biome.ParticleEmitter, player.Position, Vector3.Zero, out object p);
                        if (p != null)
                        {
                            _particle = (IHasMovement)p;
                        }
                    }
                }
            }
            PrevBiome = CurrentBiome;
            CurrentBiome = biome;
            if (_fogColorInterpolator != null)
            {
                Vector4 prevColor = _fogColorInterpolator.CurrentValue;
                float prevDistance = _fogDistanceInterpolator.CurrentValue;
                _fogColorInterpolator.Start(prevColor, biome.GetFogColor(Actor.World).ToVector4(), 3);
                _fogDistanceInterpolator.Start(prevDistance, biome.GetFogDistance(Actor.World), 3);
            }
            TimeInBiome = 0;
            CurrentBiome.OnBiomeEnter(Actor);
            PrevBiome?.OnBiomeExit(Actor);
        }

        public void Update()
        {
            if (CurrentBiome == null)
            {
                Biome b = Actor.World.BiomeManager().GetBiome(Actor.Position);
                SetBiome(b);
                return;
            }

            TimeInBiome += Services.ElapsedTime;
            if (_fogColorInterpolator != null)
            {
                _fogColorInterpolator.Update();
                _fogDistanceInterpolator.Update();
                FogColor = _fogColorInterpolator.CurrentValue;
                FogDistance = _fogDistanceInterpolator.CurrentValue;
            }

            if (_particle != null)
            {
                _particle.Position = Actor.Position;
            }
            Biome biome = Actor.World.BiomeManager().GetBiome(Actor.Position);
            SetBiome(biome);
        }

        public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
        {
            
        }

        public void WriteState(BinaryWriter writer)
        {
            
        }
    }
}
