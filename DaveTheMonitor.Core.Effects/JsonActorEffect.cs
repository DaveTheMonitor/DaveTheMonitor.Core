using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Effects.Components;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace DaveTheMonitor.Core.Effects
{
    [DebuggerDisplay("{Id}")]
    [ActorEffectRegisterIgnore]
    public sealed class JsonActorEffect : ActorEffectDefinition, IJsonType<JsonActorEffect>
    {
        private class ActorEffectParticleData : ICoreData<ActorEffect>
        {
            public double TotalEmitted { get; set; }
            public bool ShouldSave => false;

            public void Initialize(ActorEffect item)
            {
                TotalEmitted = 0;
            }

            public void ReadState(BinaryReader reader, int tmVersion, int coreVersion)
            {
                
            }

            public void WriteState(BinaryWriter writer)
            {
                
            }
        }
        public override string Id => _id;
        public override Texture2D BackgroundTexture => _background;
        public override Texture2D IconTexture => _icon;
        public override ActorEffectType Type => _type;
        public ComponentCollection Components { get; private set; }
        public EffectDefinitionComponent Definition { get; private set; }
        public EffectDisplayComponent Display { get; private set; }
        public EffectHealthComponent Health { get; private set; }
        public EffectParticleComponent Particle { get; private set; }
        private string _id;
        private ActorEffectType _type;
        private Texture2D _background;
        private Texture2D _icon;
        private Rectangle _bgSrc;
        private Rectangle _iconSrc;
        private ICoreMod _mod;

        public static JsonActorEffect FromJson(string json)
        {
            JsonDocumentOptions docOptions = DeserializationHelper.DocumentOptionsTrailingCommasSkipComments;
            JsonSerializerOptions serializerOptions = DeserializationHelper.SerializerOptionsTrailingCommasSkipComments;
            ComponentCollection components = DeserializationHelper.ReadComponents(json, "Effect", docOptions, serializerOptions);

            if (!components.HasComponent<EffectDefinitionComponent>())
            {
                throw new InvalidCoreJsonException("Effect must have a Definition component.");
            }

            return new JsonActorEffect(components);
        }

        public void ReplaceWith(ICoreMod mod, IJsonType<JsonActorEffect> other)
        {
            other.Components.CopyTo(Components, true);
            LoadAssets(mod, other.Components);
            UpdateFields();
        }

        public override void OnRegister(ICoreMod mod)
        {
            LoadAssets(mod, Components);
            _mod = mod;
            UpdateFields();
        }

        private void LoadAssets(ICoreMod mod, ComponentCollection components)
        {
            var display = components.GetComponent<EffectDisplayComponent>();
            if (display.Background != null)
            {
                _background = Game.ModManager.LoadTexture(mod, display.Background);
            }
            if (display.Icon != null)
            {
                _icon = Game.ModManager.LoadTexture(mod, display.Icon);
            }
        }

        private void UpdateFields()
        {
            Definition = Components.GetComponent<EffectDefinitionComponent>();
            Display = Components.GetComponent<EffectDisplayComponent>();
            Health = Components.GetComponent<EffectHealthComponent>();
            Particle = Components.GetComponent<EffectParticleComponent>();
            _id = Definition.EffectId;
            _type = Definition.EffectType;
            _bgSrc = Display.BackgroundSrc ?? new Rectangle(0, 0, _background.Width, _background.Height);
            _iconSrc = Display.IconSrc ?? new Rectangle(0, 0, _icon.Width, _icon.Height);
        }

        public override string GetName(ActorEffect effect) => Display?.Name;

        public override string GetDescription(ActorEffect effect) => Display?.Description;

        public override Rectangle GetBackgroundSrc(ActorEffect effect) => _bgSrc;

        public override Rectangle GetIconSrc(ActorEffect effect) => _iconSrc;

        public override void Update(ActorEffect effect)
        {
            if (Health != null)
            {
                effect.Actor.ChangeHealth(Health.Health * Services.ElapsedTime, true);
            }
            if (Particle != null)
            {
                EmitParticles(effect);
            }
        }

        private void EmitParticles(ActorEffect effect)
        {
            float freq = GetParticleFrequency(effect);
            ActorEffectParticleData data = effect.GetData<ActorEffectParticleData>();
            double totalEmitted = data.TotalEmitted;
            double prev = totalEmitted;
            totalEmitted += freq * Services.ElapsedTime;

            int numToEmit = (int)Math.Floor(totalEmitted) - (int)Math.Floor(prev);
            if (numToEmit > 0)
            {
                for (int i = 0; i < numToEmit; i++)
                {
                    for (int j = 0; j < Particle.Count; j++)
                    {
                        EmitParticle(effect);
                    }
                }
            }
            data.TotalEmitted = totalEmitted;
        }

        private float GetParticleFrequency(ActorEffect effect)
        {
            if (effect.Actor.IsPlayer && ((ICorePlayer)effect.Actor).IsLocalPlayer)
            {
                return Particle.LocalFrequency ?? Particle.Frequency;
            }
            else
            {
                return Particle.Frequency;
            }
        }

        private void EmitParticle(ActorEffect effect)
        {
            Vector3 offset = GetParticleOffset(effect);
            Vector3 velocity = GetParticleVelocity(effect);
            if (effect.Actor.IsPlayer)
            {
                ICorePlayer player = (ICorePlayer)effect.Actor;
                if (player.IsLocalPlayer)
                {
                    effect.Actor.World.SpawnParticle(Particle.LocalParticleID, effect.Actor.Position + offset, velocity);
                    return;
                }
            }
            effect.Actor.World.SpawnParticle(Particle.ParticleID, effect.Actor.Position + offset, velocity);
        }

        private Vector3 GetParticleOffset(ActorEffect effect)
        {
            Vector3 offset = Particle.Offset ?? RandomVector3(effect.Game, Particle.OffsetMin.Value, Particle.OffsetMax.Value);
            if (Particle.OffsetType == EffectParticleOffsetType.Box)
            {
                Vector3 boxSize = effect.Actor.HitBoundingBox.Max - effect.Actor.HitBoundingBox.Min;
                Vector3 min = new Vector3(-boxSize.X / 2f, 0, -boxSize.Z / 2f);
                Vector3 max = new Vector3(-min.X, boxSize.Y, -min.Z);
                offset += RandomVector3(effect.Game, min, max);
            }
            return offset;
        }

        private Vector3 GetParticleVelocity(ActorEffect effect)
        {
            Vector3 velocity = Particle.Velocity ?? RandomVector3(effect.Game, Particle.VelocityMin.Value, Particle.VelocityMax.Value);
            if (Particle.VelocityType == EffectParticleVelocityType.Relative)
            {
                velocity += effect.Actor.Velocity / Services.ElapsedTime;
            }
            return velocity;
        }

        private Vector3 RandomVector3(ICoreGame game, Vector3 min, Vector3 max)
        {
            float x = min.X + ((float)game.TMGame.Random.NextDouble() * (max.X - min.X));
            float y = min.Y + ((float)game.TMGame.Random.NextDouble() * (max.Y - min.Y));
            float z = min.Z + ((float)game.TMGame.Random.NextDouble() * (max.Z - min.Z));
            return new Vector3(x, y, z);
        }

        public override void EffectAdded(ActorEffect effect)
        {
            if (Particle != null)
            {
                effect.SetDefaultData<ActorEffectParticleData>();
            }
        }

        public override void EffectRemoved(ActorEffect effect)
        {

        }

        private JsonActorEffect(ComponentCollection components)
        {
            Components = components;
            _id = Components.GetComponent<EffectDefinitionComponent>().EffectId;
        }
    }
}
