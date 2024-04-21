using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.TotalMiner;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// An actor defintion.
    /// </summary>
    [DebuggerDisplay("Actor : {Id}")]
    public sealed class CoreActor : IDefinition, IJsonType<CoreActor>
    {
        public string Id => Definition.ActorId;
        public int NumId { get => (int)ActorType; set => ActorType = (ActorType)value; }

        /// <summary>
        /// The <see cref="ActorType"/> of this actor.
        /// </summary>
        public ActorType ActorType { get; private set; }

        /// <summary>
        /// This actor's defined components.
        /// </summary>
        public ComponentCollection Components { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorDefinitionComponent"/>.
        /// </summary>
        public ActorDefinitionComponent Definition { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorDisplayComponent"/>.
        /// </summary>
        public ActorDisplayComponent Display { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorNaturalSpawnComponent"/>.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ActorNaturalSpawnComponent NaturalSpawn { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorImmuneToFireComponent"/>.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ActorImmuneToFireComponent ImmuneToFire { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorBreatheUnderwaterComponent"/>.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ActorBreatheUnderwaterComponent BreatheUnderwater { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorPassiveComponent"/>.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ActorPassiveComponent Passive { get; private set; }

        /// <summary>
        /// This actor's <see cref="ActorCombatComponent"/>.
        /// </summary>
        /// <remarks>This may be null.</remarks>
        public ActorCombatComponent Combat { get; private set; }

        /// <summary>
        /// True if this actor is immune to fire, otherwise false.
        /// </summary>
        public bool IsImmuneToFire => ImmuneToFire?.IsImmune ?? false;

        /// <summary>
        /// True if this actor can breathe underwater, otherwise false.
        /// </summary>
        public bool CanBreatheUnderwater => BreatheUnderwater?.CanBreatheUnderwater ?? false;

        /// <summary>
        /// True if this actor is passive, otherwise false.
        /// </summary>
        public bool IsPassive => Passive?.IsPassive ?? false;
        internal ICoreGame _game;

        public static CoreActor FromJson(string json)
        {
            JsonDocumentOptions docOptions = DeserializationHelper.DocumentOptionsTrailingCommasSkipComments;
            JsonSerializerOptions serializerOptions = DeserializationHelper.SerializerOptionsTrailingCommasSkipComments;
            ComponentCollection components = DeserializationHelper.ReadComponents(json, "Actor", docOptions, serializerOptions);

            if (!components.HasComponent<ActorDefinitionComponent>())
            {
                throw new InvalidOperationException("Actor must have a Definition component.");
            }

            return new CoreActor(ActorType.None, components);
        }

        public void ReplaceWith(ICoreMod mod, IJsonType<CoreActor> other)
        {
            other.Components.CopyTo(Components, true);
            LoadAssets(mod, other.Components);
            UpdateFields();
        }

        public void OnRegister(ICoreMod mod)
        {
            LoadAssets(mod, Components);
            UpdateFields();
        }

        private void LoadAssets(ICoreMod mod, ComponentCollection components)
        {
            
        }

        private void UpdateFields()
        {
            Definition = Components.GetComponent<ActorDefinitionComponent>();
            Display = Components.GetComponent<ActorDisplayComponent>();
            NaturalSpawn = Components.GetComponent<ActorNaturalSpawnComponent>();
            ImmuneToFire = Components.GetComponent<ActorImmuneToFireComponent>();
            BreatheUnderwater = Components.GetComponent<ActorBreatheUnderwaterComponent>();
            Passive = Components.GetComponent<ActorPassiveComponent>();
            Combat = Components.GetComponent<ActorCombatComponent>();
        }

        /// <summary>
        /// Creates a new <see cref="CoreActor"/> from an <see cref="ActorTypeDataXML"/>.
        /// </summary>
        /// <param name="data">The data to use.</param>
        /// <returns>A new <see cref="CoreActor"/> representing the specified <see cref="ActorTypeDataXML"/>.</returns>
        public static CoreActor FromActorTypeDataXML(ActorTypeDataXML data)
        {
            ComponentCollection components = new ComponentCollection();
            components.AddComponent(ActorDefinitionComponent.FromXML(data));
            components.AddComponent(ActorDisplayComponent.FromXML(data));
            components.AddComponent(ActorNaturalSpawnComponent.FromXML(data));
            components.AddComponent(ActorImmuneToFireComponent.FromXML(data));
            components.AddComponent(ActorBreatheUnderwaterComponent.FromXML(data));
            components.AddComponent(ActorPassiveComponent.FromXML(data));
            components.AddComponent(ActorCombatComponent.FromXML(data));
            components.SetComponentDefaults();
            CoreActor actor = new CoreActor(data.ActorType, components);

            return actor;
        }

        private CoreActor(ActorType actor, ComponentCollection components)
        {
            ActorType = actor;
            Components = components;
            UpdateFields();
        }
    }
}
