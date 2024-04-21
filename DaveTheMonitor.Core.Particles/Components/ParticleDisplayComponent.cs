using DaveTheMonitor.Core.Animation;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Particles.Components
{
    [Component("Core.ParticleDisplay", "Display", "Particle")]
    public sealed class ParticleDisplayComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ParticleDisplay";
        public string Texture { get; private set; }
        public Rectangle? Src { get; private set; }
        public KeyframeCollection<Rectangle> SrcKeyframes { get; private set; }
        public Vector2? Size { get; private set; }
        public KeyframeCollection<Vector2> SizeKeyframes { get; private set; }
        public Color? ParticleColor { get; private set; }
        public KeyframeCollection<Color> ColorKeyframes { get; private set; }
        public string Material { get; private set; }
        public ParticleFaceType FaceType => _faceType.Value;
        private ParticleFaceType? _faceType;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            Texture = DeserializationHelper.GetStringProperty(element, "Texture");
            Material = DeserializationHelper.GetStringProperty(element, "Material");
            _faceType = DeserializationHelper.GetEnumProperty<ParticleFaceType>(element, "FaceType");

            if (element.TryGetProperty("Src", out JsonElement src))
            {
                if (src.ValueKind == JsonValueKind.Array)
                {
                    Src = DeserializationHelper.GetRectangle(src);
                    SrcKeyframes = null;
                }
                else
                {
                    Src = null;
                    SrcKeyframes = KeyframeCollection<Rectangle>.FromJson(src, DeserializationHelper.GetRectangle);
                }
            }

            if (element.TryGetProperty("Size", out JsonElement size))
            {
                if (size.ValueKind == JsonValueKind.Array)
                {
                    Size = DeserializationHelper.GetVector2(size);
                    SizeKeyframes = null;
                }
                else
                {
                    Size = null;
                    SizeKeyframes = KeyframeCollection<Vector2>.FromJson(size, DeserializationHelper.GetVector2);
                }
            }

            if (element.TryGetProperty("Color", out JsonElement color))
            {
                if (color.ValueKind == JsonValueKind.Array)
                {
                    ParticleColor = DeserializationHelper.GetColor(color);
                    ColorKeyframes = null;
                }
                else
                {
                    ParticleColor = null;
                    ColorKeyframes = KeyframeCollection<Color>.FromJson(color, DeserializationHelper.GetColor);
                }
            }
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ParticleDisplayComponent)replacement;
            if (component.Texture != null) Texture = component.Texture;
            if (component.Material != null) Material = component.Material;
            if (component._faceType.HasValue) _faceType = component._faceType;

            if (component.Src.HasValue)
            {
                Src = component.Src.Value;
                SrcKeyframes = null;
            }
            else if (component.SrcKeyframes != null)
            {
                Src = null;
                SrcKeyframes = component.SrcKeyframes;
            }

            if (component.Size.HasValue)
            {
                Size = component.Size.Value;
                SizeKeyframes = null;
            }
            else if (component.SizeKeyframes != null)
            {
                Size = null;
                SizeKeyframes = component.SizeKeyframes;
            }

            if (component.ParticleColor.HasValue)
            {
                ParticleColor = component.ParticleColor.Value;
                ColorKeyframes = null;
            }
            else if (component.ColorKeyframes != null)
            {
                ParticleColor = null;
                ColorKeyframes = component.ColorKeyframes;
            }
        }

        public override void SetDefaults()
        {
            Texture ??= null;
            Material ??= "AlphaTest";
            _faceType ??= ParticleFaceType.Billboard;
            if (!Size.HasValue && SizeKeyframes == null)
            {
                Size = new Vector2(1, 1);
            }
            if (!Src.HasValue && SrcKeyframes == null)
            {
                Src = Rectangle.Empty;
            }
            if (!ParticleColor.HasValue && ColorKeyframes == null)
            {
                ParticleColor = Color.White;
                ColorKeyframes = null;
            }
        }
    }
}
