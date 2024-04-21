using DaveTheMonitor.Core.Shaders;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Allows mods to add modifiers to shader parameters used by the game.
    /// </summary>
    public interface IGameShader
    {
        // TODO: implement TintColor and SkyColor.
        /// <summary>
        /// The fog color used by the game shader.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this value to modify it, use <see cref="AddFogColorModifier(ShaderModifier{Vector4})"/></para>
        /// </remarks>
        public Vector4 FogColor { get; set; }

        /// <summary>
        /// The start distance of the fog used by the game shader.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this value to modify it, use <see cref="AddFogStartModifier(ShaderModifier{float})"/></para>
        /// </remarks>
        public float FogStart { get; set; }

        /// <summary>
        /// The end distance of the fog used by the game shader.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this value to modify it, use <see cref="AddFogStartModifier(ShaderModifier{float})"/></para>
        /// </remarks>
        public float FogEnd { get; set; }

        /// <summary>
        /// The lanturn (held light) color used by the game shader.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this value to modify it, use <see cref="AddLanturnColorModifier(ShaderModifier{Vector3})"/></para>
        /// </remarks>
        public Vector3 LanturnColor { get; set; }

        /// <summary>
        /// The lanturn (held light) radius used by the game shader.
        /// </summary>
        /// <remarks>
        /// <para>Do not set this value to modify it, use <see cref="AddLanturnRangeModifier(ShaderModifier{float})"/></para>
        /// </remarks>
        public float LanturnRange { get; set; }
        //public Vector4 TintColor { get; set; }
        //public Vector4 SkyColor { get; set; }

        /// <summary>
        /// Adds a modifier that changes the fog color.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        void AddFogColorModifier(ShaderModifier<Vector4> modifier);

        /// <summary>
        /// Adds a modifier that changes the start distance of the fog.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        void AddFogStartModifier(ShaderModifier<float> modifier);

        /// <summary>
        /// Adds a modifier that changes the end distance of the fog.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        void AddFogEndModifier(ShaderModifier<float> modifier);

        /// <summary>
        /// Adds a modifier that changes the lanturn (held light) color.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        void AddLanturnColorModifier(ShaderModifier<Vector3> modifier);

        /// <summary>
        /// Adds a modifier that changes the lanturn (held light) radius.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        void AddLanturnRangeModifier(ShaderModifier<float> modifier);
        //void AddTintColorModifier(ShaderModifier<Vector4> modifier);
        //void AddSkyColorModifier(ShaderModifier<Vector4> modifier);

        /// <summary>
        /// Applies all fog color modifiers to <paramref name="value"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="value">The value to modify.</param>
        void ApplyFogColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value);

        /// <summary>
        /// Applies all fog start distance modifiers to <paramref name="value"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="value">The value to modify.</param>
        void ApplyFogStartModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value);

        /// <summary>
        /// Applies all fog end distance modifiers to <paramref name="value"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="value">The value to modify.</param>
        void ApplyFogEndModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value);

        /// <summary>
        /// Applies all lanturn (held light) color modifiers to <paramref name="value"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="value">The value to modify.</param>
        void ApplyLanturnColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector3 value);

        /// <summary>
        /// Applies all lanturn (held light) radius modifiers to <paramref name="value"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="value">The value to modify.</param>
        void ApplyLanturnRangeModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref float value);
        //void ApplyTintColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value);
        //void ApplySkyColorModifiers(ICorePlayer player, ITMPlayer virtualPlayer, ref Vector4 value);
    }
}
