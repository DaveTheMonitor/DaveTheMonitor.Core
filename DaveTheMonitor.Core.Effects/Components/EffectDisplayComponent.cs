using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Effects.Components
{
    [Component("Core.EffectDisplay", "Display", "Effect")]
    public sealed class EffectDisplayComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.EffectDisplay";
        public string Background { get; private set; }
        public string Icon { get; private set; }
        public Rectangle? BackgroundSrc { get; private set; }
        public Rectangle? IconSrc { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            Background = DeserializationHelper.GetStringProperty(element, "Background");
            Icon = DeserializationHelper.GetStringProperty(element, "Icon");
            BackgroundSrc = DeserializationHelper.GetRectangleProperty(element, "BackgroundSrc");
            IconSrc = DeserializationHelper.GetRectangleProperty(element, "IconSrc");
            Name = DeserializationHelper.GetStringProperty(element, "Name");
            Description = DeserializationHelper.GetStringProperty(element, "Description");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (EffectDisplayComponent)replacement;
            if (component.Name != null) Name = component.Name;
            if (component.Description != null) Description = component.Description;

            if (component.Background != null)
            {
                Background = component.Background;
                BackgroundSrc = component.BackgroundSrc;
            }
            else if (component.BackgroundSrc.HasValue) BackgroundSrc = component.BackgroundSrc;

            if (component.Icon != null)
            {
                Icon = component.Icon;
                IconSrc = component.IconSrc;
            }
            else if (component.IconSrc.HasValue) IconSrc = component.IconSrc;
        }

        public override void SetDefaults()
        {
            Name ??= null;
            Description ??= null;
            Background ??= null;
            Icon ??= null;
            BackgroundSrc ??= null;
            Icon ??= null;
        }
    }
}
