using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Components.Actors
{
    [Component("Core.ActorNaturalSpawn", "NaturalSpawn", "Actor")]
    public sealed class ActorNaturalSpawnComponent : Component, IComponentDeserializable
    {
        public override string ComponentId => "Core.ActorNaturalSpawn";
        public string Behavior { get; private set; }
        public float SpawnFrequency { get; private set; }
        private float? _spawnFrequency;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            Behavior = DeserializationHelper.GetStringProperty(element, "Behavior");
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (ActorNaturalSpawnComponent)replacement;
            if (component.Behavior != null) Behavior = component.Behavior;
            if (component._spawnFrequency.HasValue) _spawnFrequency = component._spawnFrequency;
        }

        public void ReplaceXmlData(ActorTypeDataXML data)
        {
            if (Behavior != null) data.NaturalBehaviour = Behavior;
            if (_spawnFrequency.HasValue) data.NaturalSpawnFreq = SpawnFrequency;
        }

        public override void SetDefaults()
        {
            Behavior ??= @"System\AI\Passive";
            _spawnFrequency ??= 60;
        }

        public static ActorNaturalSpawnComponent FromXML(ActorTypeDataXML data)
        {
            var component = new ActorNaturalSpawnComponent()
            {
                Behavior = data.NaturalBehaviour,
                _spawnFrequency = data.NaturalSpawnFreq,
            };

            return component;
        }
    }
}
