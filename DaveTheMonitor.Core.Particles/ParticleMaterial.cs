using Microsoft.Xna.Framework.Graphics;

namespace DaveTheMonitor.Core.Particles
{
    public class ParticleMaterial
    {
        public static ParticleMaterial AlphaTest { get; private set; }
        public static ParticleMaterial AlphaTestNoFog { get; private set; }
        public static ParticleMaterial Opaque { get; private set; }
        public static ParticleMaterial OpaqueNoFog { get; private set; }
        public static ParticleMaterial AlphaBlend { get; private set; }
        public static ParticleMaterial AlphaBlendNoFog { get; private set; }
        public string Id { get; private set; }
        public BlendState BlendState { get; private set; }
        public RasterizerState RasterizerState { get; private set; }
        public bool FogEnabled { get; private set; }
        internal int _numId;

        static ParticleMaterial()
        {
            AlphaTest = new ParticleMaterial("AlphaTest", 0)
            {
                BlendState = BlendState.AlphaBlend,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = true
            };
            AlphaTestNoFog = new ParticleMaterial("AlphaTest_NoFog", 1)
            {
                BlendState = BlendState.NonPremultiplied,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = false
            };
            Opaque = new ParticleMaterial("Opaque", 2)
            {
                BlendState = BlendState.AlphaBlend,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = true
            };
            OpaqueNoFog = new ParticleMaterial("Opaque_NoFog", 3)
            {
                BlendState = BlendState.Opaque,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = false
            };
            AlphaBlend = new ParticleMaterial("AlphaBlend", 4)
            {
                BlendState = BlendState.AlphaBlend,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = true
            };
            AlphaBlendNoFog = new ParticleMaterial("AlphaBlend_NoFog", 5)
            {
                BlendState = BlendState.AlphaBlend,
                RasterizerState = RasterizerState.CullCounterClockwise,
                FogEnabled = false
            };
        }

        private ParticleMaterial(string id, int numId)
        {
            Id = id;
            _numId = numId;
        }
    }
}
