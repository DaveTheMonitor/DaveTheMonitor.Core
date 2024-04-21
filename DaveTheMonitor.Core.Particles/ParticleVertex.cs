using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace DaveTheMonitor.Core.Particles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleVertex : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;
        public Vector3 Position;

        public ParticleVertex(Vector3 position)
        {
            Position = position;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{{Position: {Position}}}";
        }

        public static bool operator ==(ParticleVertex left, ParticleVertex right)
        {
            return left.Position == right.Position;
        }

        public static bool operator !=(ParticleVertex left, ParticleVertex right)
        {
            return left.Position != right.Position;
        }

        public override bool Equals(object obj)
        {
            return obj is ParticleVertex v && this == v;
        }

        static ParticleVertex()
        {
            VertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            });
        }
    }
}