using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Effects.Components
{
    [Component("Core.EffectHealth", "Health", "Effect")]
    public sealed class EffectHealthComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.EffectHealth";
        public float Health => _health.Value;
        private float? _health;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            _health = DeserializationHelper.GetSingleProperty(element, "Amount");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (EffectHealthComponent)replacement;
            if (component._health.HasValue) _health = component._health.Value;
        }

        public override void SetDefaults()
        {
            _health ??= 0;
        }
    }
}
