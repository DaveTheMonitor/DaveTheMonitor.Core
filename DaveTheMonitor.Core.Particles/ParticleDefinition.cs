using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using System;

namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// Represents a particle definition that is used to update particles.
    /// <para>Individual particles are instances of <see cref="ParticleInstance"/>, which are passed to the definition when updating or drawing. Do not store particle instance data in the definition, instead use ParticleInstance.Data.</para>
    /// </summary>
    public abstract class ParticleDefinition : IDefinition
    {
        /// <summary>
        /// The ID of this particle definition.
        /// </summary>
        public abstract string Id { get; }
        /// <summary>
        /// The numeric ID of this particle definition.
        /// </summary>
        public int NumId { get; set; }
        /// <summary>
        /// The texture used by this particle definition.
        /// </summary>
        public Texture2D Texture { get; protected set; }
        /// <summary>
        /// True if particles using this particle definition should be drawn. Typically False for <see cref="ParticleEmitter"/> definitions.
        /// </summary>
        public virtual bool Draw => true;
        public abstract ParticleMaterial Material { get; }
        /// <summary>
        /// The main game instance.
        /// </summary>
        protected ICoreGame Game { get; private set; }
        protected ParticleRegistry ParticleRegister { get; private set; }
        internal ParticleTextureInfo _textureInfo;

        /// <summary>
        /// Called when the particle is initially spawned.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public abstract void Initialize(ParticleInstance particle);

        /// <summary>
        /// Called after the particle is initially registered. Not all particles will be registered at the time this is called.
        /// </summary>
        public virtual void OnRegister(ICoreMod mod)
        {
            
        }

        /// <summary>
        /// Called when the particle is updated. The particle can be destroyed by calling <see cref="Destroy(ParticleInstance)"/>
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public abstract void Update(ParticleManager particleManager, ParticleInstance particle, ICoreWorld world);

        /// <summary>
        /// Gets the size of the particle.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public abstract Vector2 GetSize(ParticleInstance particle);

        /// <summary>
        /// Gets the source rectangle of the texture for the particle.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public abstract Rectangle GetSrc(ParticleInstance particle);

        /// <summary>
        /// Gets the duration of the particle in seconds. Should be -1 if the particle should have infinite lifetime or for instant emitters.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        /// <returns>The duration of the particle, or -1 if the particle should live until destroyed.</returns>
        public abstract float GetDuration(ParticleInstance particle);

        /// <summary>
        /// Gets the direction of the particle as a matrix. Defaults to a billboard.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        /// <param name="cameraPos">The position of the camera.</param>
        /// <param name="cameraViewDirection">The view direction of the camera.</param>
        public virtual void GetDirection(ParticleInstance particle, ref Vector3 cameraPos, Vector3 cameraViewDirection, out Matrix matrix)
        {
            Vector3 pos = particle.Position;
            Vector3 up = Vector3.Up;
            Matrix.CreateBillboard(ref pos, ref cameraPos, ref up, cameraViewDirection, out matrix);
            return ;
        }

        /// <summary>
        /// Gets the color of the particle. Defaults to <see cref="Color.White"/> (no color)
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        public virtual Color GetColor(ParticleInstance particle)
        {
            return Color.White;
        }

        /// <summary>
        /// Sets the game instance for this particle definition.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public void SetGame(ICoreGame game)
        {
            Game = game;
        }

        /// <summary>
        /// Sets the particle register for this particle definition. This should only be called when registering a particle.
        /// </summary>
        /// <param name="particleRegister">The particle manager.</param>
        public void SetParticleRegister(ParticleRegistry particleRegister)
        {
            ParticleRegister = particleRegister;
        }

        /// <summary>
        /// Returns True if the particle is touching any non-passable block. Use this if the particle should have collision detection.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        /// <returns>True if the particle is touching any non-passable block.</returns>
        protected bool TouchingNonPassableBlock(ParticleInstance particle)
        {
            GlobalPoint3D p = new GlobalPoint3D((int)Math.Floor(particle.Position.X), (int)Math.Floor(particle.Position.Y), (int)Math.Floor(particle.Position.Z));
            Map map = (Map)Game.TMGame.World.Map;
            byte block = map.GetBlockID(p);
            if (block == (byte)Block.SnowLayer)
            {
                // Snow layer is non-passable, but we don't want
                // them to destroy particles so we return false.
                return false;
            }
            return !map.IsBlockPassable(block);
        }

        /// <summary>
        /// Destroys the particle.
        /// </summary>
        /// <param name="particle">The particle instance.</param>
        protected void Destroy(ParticleManager particleManager, ParticleInstance particle)
        {
            particleManager.DestroyParticle(particle);
        }
    }
}
