using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Effects.Components
{
    [Component("Core.EffectDefinition", "Definition", "Effect")]
    public sealed class EffectDefinitionComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.EffectDefinition";
        public string EffectId { get; private set; }
        public ActorEffectType EffectType => _effectType.Value;
        private ActorEffectType? _effectType;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            EffectId = DeserializationHelper.GetStringProperty(element, "ID");
            _effectType = DeserializationHelper.GetEnumProperty<ActorEffectType>(element, "Type");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (EffectDefinitionComponent)replacement;
            if (component.EffectId != null) EffectId = component.EffectId;
            if (component._effectType.HasValue) _effectType = component._effectType;
        }

        public override void SetDefaults()
        {
            EffectId ??= null;
            _effectType ??= ActorEffectType.Neutral;
        }
    }
}
