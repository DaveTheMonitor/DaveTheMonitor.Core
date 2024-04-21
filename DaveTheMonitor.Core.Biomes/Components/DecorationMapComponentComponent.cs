using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Biomes.Components
{
    [Component(Id, "Component", "Decoration")]
    public sealed class DecorationMapComponentComponent : Component, IComponentDeserializable
    {
        private const string Id = "Core.DecorationMapComponent";
        public override string ComponentId => Id;
        public string DecorationId { get; private set; }
        public bool CanRotate => _canRotate.Value;
        public GlobalPoint3D? Origin { get; private set; }
        public Block? OriginBlock { get; private set; }
        public Block? OriginReplaceBlock { get; private set; }
        public byte? OriginReplaceAux { get; private set; }
        public Map.CopyType CopyType => _copyType.Value;
        private bool? _canRotate;
        private Map.CopyType? _copyType;

        Type IComponentDeserializable.GetDeserializeType(ModVersion version) => typeof(JsonElement);
        void IComponentDeserializable.ReadFrom(ModVersion version, object obj)
        {
            JsonElement element = (JsonElement)obj;
            DecorationId = DeserializationHelper.GetStringProperty(element, "ID");
            _canRotate = DeserializationHelper.GetBoolProperty(element, "CanRotate");
            _copyType = DeserializationHelper.GetEnumProperty<Map.CopyType>(element, "Merge");

            Origin = DeserializationHelper.GetGlobalPoint3DProperty(element, "Origin");
            if (!Origin.HasValue)
            {
                if (element.TryGetProperty("OriginBlock", out JsonElement origin) && origin.ValueKind == JsonValueKind.Object)
                {
                    OriginBlock = DeserializationHelper.GetEnumProperty<Block>(origin, "Target");
                    OriginReplaceBlock = DeserializationHelper.GetEnumProperty<Block>(origin, "ReplaceBlock");
                    OriginReplaceAux = DeserializationHelper.GetByteProperty(origin, "ReplaceAux");
                }
            }
        }

        public override void ReplaceWith(Component replacement)
        {
            var component = (DecorationMapComponentComponent)replacement;
            if (component.DecorationId != null) DecorationId = component.DecorationId;
            if (component._canRotate.HasValue) _canRotate = component._canRotate;
            if (component._copyType.HasValue) _copyType = component._copyType;

            if (component.Origin.HasValue)
            {
                Origin = component.Origin;
                OriginBlock = null;
                OriginReplaceBlock = null;
                OriginReplaceAux = null;
            }
            else if (component.OriginBlock.HasValue)
            {
                Origin = null;
                OriginBlock = component.OriginBlock;
                OriginReplaceBlock = component.OriginReplaceBlock;
                OriginReplaceAux = component.OriginReplaceAux;
            }
        }

        public override void SetDefaults()
        {
            DecorationId ??= null;
            _canRotate ??= true;
            _copyType ??= Map.CopyType.NoOverwrite;

            if (!Origin.HasValue && !OriginBlock.HasValue)
            {
                Origin = GlobalPoint3D.Zero;
                OriginReplaceBlock = null;
                OriginReplaceAux = null;
            }
        }
    }
}
