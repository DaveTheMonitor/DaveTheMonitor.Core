using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.Engine;

namespace DaveTheMonitor.Core.Particles
{
    public sealed class ParticleInstance : IHasMovement, IHasColor
    {
        public int Id { get; private set; }
        public int Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Color Color { get; set; }
        public float Age { get; private set; }
        public float Data1 { get; set; }
        public float Data2 { get; set; }
        public float Data3 { get; set; }
        public float Data4 { get; set; }
        public bool Dead { get; internal set; }

        public void Update(ParticleManager particleManager, ParticleDefinition definition, ICoreWorld world)
        {
            float duration = definition.GetDuration(this);
            if (duration != -1 && Age >= duration)
            {
                particleManager.DestroyParticle(this);
                return;
            }

            definition.Update(particleManager, this, world);
            Age += Services.ElapsedTime;
            Position += Velocity * Services.ElapsedTime;
        }

        public void Initialize(ParticleDefinition definition, Vector3 position, Vector3 velocity)
        {
            Type = definition.NumId;
            Position = position;
            Velocity = velocity;
            Age = 0;
            Data1 = 0;
            Data2 = 0;
            Data3 = 0;
            Data4 = 0;
            Dead = false;
            definition.Initialize(this);
        }

        public bool ShouldRender(Vector2 size, BoundingFrustum frustum)
        {
            Vector3 sizeVector3 = new Vector3(size.X, size.Y, size.X);
            Vector3 min = Position - sizeVector3;
            Vector3 max = Position + sizeVector3;
            BoundingBox box = new BoundingBox(min, max);
            return frustum.Intersects(box);
        }

        public ParticleInstance(int id)
        {
            Id = id;
            Dead = true;
        }
    }
}
