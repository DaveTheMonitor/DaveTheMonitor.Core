using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace DaveTheMonitor.Core.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ActorEffectVertex : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            });
        public Vector3 Position;
        public Vector2 TexCoord0;
        public Vector2 TexCoord1;

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{{ Position: {Position}, TexCoord0: {TexCoord0}, TexCoord1: {TexCoord1} }}";
        }

        public static bool operator ==(ActorEffectVertex left, ActorEffectVertex right)
        {
            return left.Position == right.Position;
        }

        public static bool operator !=(ActorEffectVertex left, ActorEffectVertex right)
        {
            return left.Position != right.Position;
        }

        public override bool Equals(object obj)
        {
            return obj is ActorEffectVertex v && this == v;
        }

        public ActorEffectVertex(Vector2 position, Vector2 texCoord0, Vector2 texCoord1)
        {
            Position = new Vector3(position, 0);
            TexCoord0 = texCoord0;
            TexCoord1 = texCoord1;
        }
    }
}