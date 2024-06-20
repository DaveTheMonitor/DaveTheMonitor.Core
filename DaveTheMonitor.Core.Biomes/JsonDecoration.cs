using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Assets;
using DaveTheMonitor.Core.Biomes.Components;
using DaveTheMonitor.Core.Components;
using DaveTheMonitor.Core.Helpers;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Text.Json;

namespace DaveTheMonitor.Core.Biomes
{
    [DecorationRegisterIgnore]
    public sealed class JsonDecoration : DecorationDefinition, IJsonType<JsonDecoration>
    {
        public override string Id => _id;
        public ComponentCollection Components { get; private set; }
        public DecorationDefinitionComponent Definition { get; private set; }
        public DecorationMapComponentComponent MapComponent { get; private set; }
        public override ITMMap Component => _component;
        public override GlobalPoint3D Origin => _origin;
        public override Map.CopyType CopyType => _copyType;
        public override bool CanRotate => _canRotate;
        private string _id;
        private ITMMap _component;
        private GlobalPoint3D _origin;
        private Map.CopyType _copyType;
        private bool _canRotate;

        public static JsonDecoration FromJson(string json)
        {
            JsonDocumentOptions docOptions = DeserializationHelper.DocumentOptionsTrailingCommasSkipComments;
            JsonSerializerOptions serializerOptions = DeserializationHelper.SerializerOptionsTrailingCommasSkipComments;
            ComponentCollection components = DeserializationHelper.ReadComponents(json, "Decoration", docOptions, serializerOptions);

            if (!components.HasAllComponents<DecorationDefinitionComponent, DecorationMapComponentComponent>())
            {
                throw new InvalidOperationException("Decoration must have Definition and Component components.");
            }

            return new JsonDecoration(components);
        }

        public void ReplaceWith(ICoreMod mod, IJsonType<JsonDecoration> other)
        {
            other.Components.CopyTo(Components, true);
            UpdateFields();
            LoadAssets(mod, other.Components);
        }

        public override void OnRegister(ICoreMod mod)
        {
            UpdateFields();
            LoadAssets(mod, Components);
        }

        private void LoadAssets(ICoreMod mod, ComponentCollection components)
        {
            var mapComponent = components.HasComponent<DecorationMapComponentComponent>() ? MapComponent : null;
            if (mapComponent != null)
            {
                CoreMapAsset asset = Game.ModManager.LoadAsset<CoreMapAsset>(mod, $"Components/{mapComponent.ComponentId}");
                if (asset == null)
                {
                    return;
                }

                if (mapComponent.OriginReplaceBlock.HasValue)
                {
                    _component = Game.MapComponentLoader.LoadComponent(asset.FullPath);
                }
                else
                {
                    _component = asset.Map.TMMap;
                }

                if (mapComponent.Origin.HasValue)
                {
                    _origin = mapComponent.Origin.Value;
                    return;
                }

                Map map = (Map)_component;
                Block target = mapComponent.OriginBlock.Value;
                GlobalPoint3D origin = GlobalPoint3D.MinValue;
                for (int x = map.MapBound.Min.X; x < map.MapBound.Max.X; x++)
                {
                    for (int y = map.MapBound.Min.Y; x < map.MapBound.Max.Y; y++)
                    {
                        for (int z = map.MapBound.Min.Z; z < map.MapBound.Max.Z; z++)
                        {
                            GlobalPoint3D p = new GlobalPoint3D(x, y, z);
                            if (map.GetBlockID(p) == (byte)target)
                            {
                                origin = p;
                                break;
                            }
                        }
                    }
                }

                if (origin == GlobalPoint3D.MinValue)
                {
                    _origin = GlobalPoint3D.Zero;
                    return;
                }
                _origin = origin;

                if (mapComponent.OriginReplaceBlock.HasValue)
                {
                    map.SetBlockData(origin, (byte)mapComponent.OriginReplaceBlock.Value, mapComponent.OriginReplaceAux.Value, UpdateBlockMethod.Generation, GamerID.Sys1, false);
                }
            }
        }

        private void UpdateFields()
        {
            Definition = Components.GetComponent<DecorationDefinitionComponent>();
            MapComponent = Components.GetComponent<DecorationMapComponentComponent>();
            _id = Definition.DecorationId;
            _copyType = MapComponent.CopyType;
            _canRotate = MapComponent.CanRotate;
        }

        private JsonDecoration(ComponentCollection components)
        {
            Components = components;
            _id = Components.GetComponent<DecorationDefinitionComponent>().DecorationId;
        }
    }
}
