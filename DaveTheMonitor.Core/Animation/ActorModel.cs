using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Json;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DaveTheMonitor.Core.Animation
{
    /// <summary>
    /// A model for an actor. These models support skeletal animations.
    /// </summary>
    public sealed class ActorModel
    {
        /// <summary>
        /// The total height of the components in the model in component space. Used for scaling.
        /// </summary>
        public int BlockHeight { get; private set; }

        /// <summary>
        /// The total height of the model, in blocks.
        /// </summary>
        public float ModelHeight { get; private set; }
        
        /// <summary>
        /// The viewBounds that must be within the actor's view frustum for this model to render.
        /// </summary>
        public BoundingBox ViewBounds { get; private set; }

        /// <summary>
        /// The total number of parts in this model.
        /// </summary>
        public int TotalParts => _parts.Length;
        private ActorPart[] _parts;
        private Dictionary<string, int> _partIndexDictionary;

        /// <summary>
        /// Creates a new model from a Json string.
        /// </summary>
        /// <param name="json">The Json string to parse.</param>
        /// <param name="mod">The fallback mod for asset loading.</param>
        /// <param name="modManager">The mod manager.</param>
        /// <returns>A new model from the Json string.</returns>
        public static ActorModel FromJson(string json, ICoreMod mod)
        {
            JsonDocument doc = JsonDocument.Parse(json, DeserializationHelper.DocumentOptionsTrailingCommasSkipComments);

            ModVersion version = DeserializationHelper.GetVersionProperty(doc.RootElement, "Version")
                ?? throw new InvalidCoreJsonException("ActorModel Version must be specified.");

            if (version != new ModVersion(1, 0, 0))
            {
                throw new InvalidCoreJsonException($"ActorModel Version {version} not recognized.");
            }

            BoundingBox bounds = DeserializationHelper.GetBoundingBoxProperty(doc.RootElement, "ViewBounds")
                ?? throw new InvalidCoreJsonException("ActorModel ViewBounds must be specified.");
            int blockHeight = DeserializationHelper.GetInt32Property(doc.RootElement, "BlockHeight")
                ?? throw new InvalidCoreJsonException("ActorModel BlockHeight must be specified.");
            float modelHeight = DeserializationHelper.GetSingleProperty(doc.RootElement, "ModelHeight")
                ?? throw new InvalidCoreJsonException("ActorModel ModelHeight must be specified.");

            List<ActorPart> parts = new List<ActorPart>();
            if (!doc.RootElement.TryGetProperty("Parts", out JsonElement partsElement))
            {
                throw new InvalidCoreJsonException("ActorModel must have at least one part.");
            }

            if (partsElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("ActorModel Parts must be an object.");
            }

            foreach (JsonProperty property in partsElement.EnumerateObject())
            {
                if (string.IsNullOrWhiteSpace(property.Name))
                {
                    throw new InvalidCoreJsonException("ActorModel part name must not be empty.");
                }

                if (parts.Exists(p => p.Id == property.Name))
                {
                    throw new InvalidCoreJsonException($"ActorModel part name \"{property.Name}\" must be unique.");
                }

                ActorPart.FromJson(property.Name, property.Value, version, parts, -1, mod);
            }

            return new ActorModel(blockHeight, modelHeight, parts.ToArray(), bounds);
        }

        /// <summary>
        /// Gets the part with the specified index.
        /// </summary>
        /// <param name="index">The index of the part to get.</param>
        /// <returns>The part of the model with the specified index.</returns>
        public ActorPart GetPart(int index)
        {
            return _parts[index];
        }

        /// <summary>
        /// Gets the part with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the part to get.</param>
        /// <returns>The part of the model with the specified ID.</returns>
        /// <remarks>
        /// This method is considerably slower than <see cref="GetPart(int)"/>. If you will be calling this several times, consider caching the part.
        /// </remarks>
        public ActorPart GetPart(string id)
        {
            if (_partIndexDictionary.TryGetValue(id, out int index))
            {
                return _parts[index];
            }

            return null;
        }

        /// <summary>
        /// Gets the index of the part with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the part to get.</param>
        /// <returns>The index of the part of the model with the specified ID.</returns>
        /// <remarks>
        /// This method is considerably slower than <see cref="GetPart(int)"/>. If you will be calling this several times, consider caching the index.
        /// </remarks>
        public int GetPartIndex(string id)
        {
            if (_partIndexDictionary.TryGetValue(id, out int index))
            {
                return index;
            }

            return -1;
        }

        /// <summary>
        /// Creates a new model with the specified parts.
        /// </summary>
        /// <param name="blockHeight">The total height of the components in component space.</param>
        /// <param name="modelHeight">The total height of the model in blocks.</param>
        /// <param name="parts">The parts of the model.</param>
        /// <param name="viewBounds">The view viewBounds of the model.</param>
        public ActorModel(int blockHeight, float modelHeight, ActorPart[] parts, BoundingBox viewBounds)
        {
            BlockHeight = blockHeight;
            ModelHeight = modelHeight;
            ViewBounds = viewBounds;
            _parts = new ActorPart[parts.Length];
            Array.Copy(parts, _parts, parts.Length);

            _partIndexDictionary = new Dictionary<string, int>();
            for (int i = 0; i < _parts.Length; i++)
            {
                ActorPart part = _parts[i];
                if (_partIndexDictionary.ContainsKey(part.Id))
                {
                    throw new ArgumentException("All model parts must have unique IDs.", nameof(parts));
                }

                _partIndexDictionary.Add(part.Id, i);
            }
        }
    }
}
