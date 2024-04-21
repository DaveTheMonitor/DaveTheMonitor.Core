using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace DaveTheMonitor.Core.Particles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParticleInstanceVertex : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;
        public Vector3 Position;
        public Vector2 Size;
        public Color Color;
        public Vector3 TexCoord0;
        public Vector3 TexCoord1;

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{{Position: {Position} Size: {Size} Color: {Color} TexCoord0: {TexCoord0} TexCoord1: {TexCoord1}}}";
        }

        public static bool operator ==(ParticleInstanceVertex left, ParticleInstanceVertex right)
        {
            return left.Position == right.Position &&
                left.Size == right.Size &&
                left.Color == right.Color &&
                left.TexCoord0 == right.TexCoord0 &&
                left.TexCoord1 == right.TexCoord1;
        }

        public static bool operator !=(ParticleInstanceVertex left, ParticleInstanceVertex right)
        {
            return left.Position != right.Position ||
                left.Size != right.Size ||
                left.Color != right.Color ||
                left.TexCoord0 != right.TexCoord0 ||
                left.TexCoord1 != right.TexCoord1;
        }

        public override bool Equals(object obj)
        {
            return obj is ParticleInstanceVertex v && this == v;
        }

        static ParticleInstanceVertex()
        {
            VertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2)
            });
        }
    }
}