using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner.API;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Shaders
{
    internal class GameShader : IGameShader
    {
        public Vector4 FogColor { get; set; }
        public float FogStart { get; set; }
        public float FogEnd { get; set; }
        public Vector3 LanturnColor { get; set; }
        public float LanturnRange { get; set; }
        public Vector4 TintColor { get; set; }
        public Vector4 SkyColor { get; set; }
        private List<ShaderModifier<Vector4>> _fogColorModifiers;
        private List<ShaderModifier<float>> _fogStartModifiers;
        private List<ShaderModifier<float>> _fogEndModifiers;
        private List<ShaderModifier<Vector3>> _lanturnColorModifiers;
        private List<ShaderModifier<float>> _lanturnRangeModifiers;
        private List<ShaderModifier<Vector4>> _tintColorModifiers;
        private List<ShaderModifier<Vector4>> _skyColorModifiers;

        public void AddFogColorModifier(ShaderModifier<Vector4> modifier)
        {
            _fogColorModifiers.Add(modifier);
        }

        public void AddFogStartModifier(ShaderModifier<float> modifier)
        {
            _fogStartModifiers.Add(modifier);
        }

        public void AddFogEndModifier(ShaderModifier<float> modifier)
        {
            _fogEndModifiers.Add(modifier);
        }

        public void AddLanturnColorModifier(ShaderModifier<Vector3> modifier)
        {
            _lanturnColorModifiers.Add(modifier);
        }

        public void AddLanturnRangeModifier(ShaderModifier<float> modifier)
        {
            _lanturnRangeModifiers.Add(modifier);
        }

        public void AddTintColorModifier(ShaderModifier<Vector4> modifier)
        {
            _tintColorModifiers.Add(modifier);
        }

        public void AddSkyColorModifier(ShaderModifier<Vector4> modifier)
        {
            _skyColorModifiers.Add(modifier);
        }

        private void ApplyModifiers<T>(ICorePlayer player, ITMPlayer virtualPlayer, List<ShaderModifier<T>> modifiers, ref T value)
        {
            foreach (ShaderModifier<T> modifier in modifiers)
            {
                modifier(player, virtualPlayer, ref value);
            }
        }

        public void ApplyFogColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value)
        {
            ApplyModifiers(player, virtualPlayer, _fogColorModifiers, ref value);
        }

        public void ApplyFogStartModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            ApplyModifiers(player, virtualPlayer, _fogStartModifiers, ref value);
        }

        public void ApplyFogEndModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            ApplyModifiers(player, virtualPlayer, _fogEndModifiers, ref value);
        }

        public void ApplyLanturnColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector3 value)
        {
            ApplyModifiers(player, virtualPlayer, _lanturnColorModifiers, ref value);
        }

        public void ApplyLanturnRangeModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value)
        {
            ApplyModifiers(player, virtualPlayer, _lanturnRangeModifiers, ref value);
        }

        public void ApplyTintColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value)
        {
            ApplyModifiers(player, virtualPlayer, _tintColorModifiers, ref value);
        }

        public void ApplySkyColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value)
        {
            ApplyModifiers(player, virtualPlayer, _skyColorModifiers, ref value);
        }

        public GameShader()
        {
            _fogColorModifiers = new List<ShaderModifier<Vector4>>();
            _fogStartModifiers = new List<ShaderModifier<float>>();
            _fogEndModifiers = new List<ShaderModifier<float>>();
            _lanturnColorModifiers = new List<ShaderModifier<Vector3>>();
            _lanturnRangeModifiers = new List<ShaderModifier<float>>();
            _tintColorModifiers = new List<ShaderModifier<Vector4>>();
            _skyColorModifiers = new List<ShaderModifier<Vector4>>();
        }
    }
}
