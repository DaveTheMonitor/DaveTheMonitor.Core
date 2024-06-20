using DaveTheMonitor.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using StudioForge.BlockWorld;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StudioForge.Engine;
using System.Diagnostics;
using System.Text.Json;
using StudioForge.TotalMiner;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A part of an <see cref="ActorModel"/>.
    /// </summary>
    [DebuggerDisplay("Id = {Id}")]
    public sealed class ActorPart
    {
        /// <summary>
        /// The component used by this part.
        /// </summary>
        public ICoreMap Component { get; private set; }

        /// <summary>
        /// The position of this part in component space. This is the lowest coordinates of the component.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// The pivot of this part in component space, relative to this part's position.
        /// </summary>
        public Vector3 Pivot { get; private set; }

        /// <summary>
        /// The index of the parent of this part.
        /// </summary>
        public int Parent { get; private set; }

        /// <summary>
        /// This part's vertex buffer. Using this requires an index buffer for quads.
        /// </summary>
        public VertexBuffer VertexBuffer { get; private set; }

        /// <summary>
        /// The number of vertices in this part.
        /// </summary>
        public int VertexCount { get; private set; }

        /// <summary>
        /// True if this part is ready to render (mesh loaded)
        /// </summary>
        public bool ReadyToRender { get; private set; }

        /// <summary>
        /// The ID of this part.
        /// </summary>
        public string Id { get; private set; }
        private MapChunk _chunk;

        internal static void FromJson(string id, JsonElement element, ModVersion version, List<ActorPart> parts, int parentIndex, ICoreMod mod)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("ActorModel part must be an object.");
            }

            string componentName = DeserializationHelper.GetStringProperty(element, "Component")
                ?? throw new InvalidCoreJsonException("ActorModel part must specify a component.");

            ICoreMap component = mod.ModManager.LoadComponent(mod, componentName)
                ?? throw new InvalidCoreJsonException($"ActorModel component \"{componentName}\" not valid.");

            Vector3 position = DeserializationHelper.GetVector3Property(element, "Position") ?? Vector3.Zero;
            Vector3 pivot = DeserializationHelper.GetVector3Property(element, "Pivot") ?? Vector3.Zero;

            int index = parts.Count;
            parts.Add(new ActorPart(id, component, parentIndex, position, pivot));

            if (element.TryGetProperty("Children", out JsonElement childrenElement))
            {
                if (childrenElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidCoreJsonException("ActorModel part Children must be an object.");
                }

                foreach (JsonProperty property in childrenElement.EnumerateObject())
                {
                    if (string.IsNullOrWhiteSpace(property.Name))
                    {
                        throw new InvalidCoreJsonException("ActorModel part name must not be empty.");
                    }

                    if (parts.Exists(p => p.Id == property.Name))
                    {
                        throw new InvalidCoreJsonException($"ActorModel part name \"{property.Name}\" must be unique.");
                    }

                    FromJson(property.Name, property.Value, version, parts, index, mod);
                }
            }
        }

        private void OnMeshLoaded(bool success, object state)
        {
            if (success)
            {
                MapChunk chunk = ((Map)Component.TMMap).GetChunk(GlobalPoint3D.Zero);
                Traverse contentData = new Traverse(chunk).Field("Content").Method("GetVertexData");
                VertexBuffer = contentData.Field<VertexBuffer>("VertexBuffer").Value;
                VertexCount = contentData.Field<int>("VertexCount").Value;
                ReadyToRender = true;
            }
            else
            {
                throw new Exception("Could not load component mesh");
            }
        }

        public bool CheckForMeshLoaded()
        {
            if (!ReadyToRender && _chunk.IsMeshLoaded)
            {
                OnMeshLoaded(true, null);
                return true;
            }
            return false;
        }

        public ActorPart(string id, ICoreMap component, int parent, Vector3 position, Vector3 pivot)
        {
            Map map = (Map)component.TMMap;
            Component = component;
            Position = position;
            Pivot = pivot;
            Parent = parent;
            ReadyToRender = false;
            Id = id;

            _chunk = map.GetChunk(GlobalPoint3D.Zero);
            // We don't attach OnChunkLoaded here as if multiple parts
            // are using the same component, not all will be visible
            if (_chunk.IsMeshLoaded)
            {
                OnMeshLoaded(true, null);
            }
            else if (!_chunk.IsChunkFlagSet(ChunkFlags.MeshLoading))
            {
                _chunk.LoadMesh(false, true, null, null);
            }
        }
    }
}
