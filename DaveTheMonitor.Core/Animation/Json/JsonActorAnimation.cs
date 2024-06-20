using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using DaveTheMonitor.Core.Json;

namespace DaveTheMonitor.Core.Animation.Json
{
    /// <summary>
    /// An animation parsed from a Json string. This should not be used for actual animation, convert it to an <see cref="ActorAnimation"/> first.
    /// </summary>
    [DebuggerDisplay("Length = {Length}, Parts = {_keyframes.Length}")]
    public sealed class JsonActorAnimation
    {
        /// <summary>
        /// The total length of this animation.
        /// </summary>
        public float Length { get; private set; }
        private Dictionary<string, KeyframeCollection<ActorPartKeyframe>[]> _keyframes;
        private KeyframeCollection<string> _events;

        /// <summary>
        /// Craets a new <see cref="JsonActorAnimation"/> from a Json string.
        /// </summary>
        /// <param name="json">The Json string to parse.</param>
        /// <returns>A new <see cref="JsonActorAnimation"/> from the Json string.</returns>
        public static JsonActorAnimation FromJson(string json)
        {
            JsonActorAnimation animation = new JsonActorAnimation();

            JsonDocument doc = JsonDocument.Parse(json, DeserializationHelper.DocumentOptionsTrailingCommasSkipComments);

            if (doc.RootElement.TryGetProperty("Events", out JsonElement eventsElement))
            {
                if (eventsElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidCoreJsonException("ActorAnimation Events must be an object.");
                }

                KeyframeCollection<string> events = KeyframeCollection<string>.FromJson(eventsElement, element =>
                {
                    if (element.ValueKind != JsonValueKind.String)
                    {
                        throw new InvalidCoreJsonException("ActorAnimation Events keyframe value must be a string.");
                    }

                    string @event = element.ToString();
                    if (string.IsNullOrWhiteSpace(@event))
                    {
                        throw new InvalidCoreJsonException("ActorAnimation Events keyframe value must not be empty.");
                    }

                    return @event;
                });

                animation.SetEvents(events);
            }

            if (!doc.RootElement.TryGetProperty("Parts", out JsonElement partsElement))
            {
                throw new InvalidCoreJsonException("ActorAnimation must define at least one part keyframe.");
            }

            if (partsElement.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidCoreJsonException("ActorAnimation Parts must be an object.");
            }

            foreach (JsonProperty property in partsElement.EnumerateObject())
            {
                ParseKeyframes(property.Value, animation, property.Name);
            }

            return animation;
        }

        private static void ParseKeyframes(JsonElement element, JsonActorAnimation animation, string partId)
        {
            foreach (JsonProperty prop in element.EnumerateObject())
            {
                ActorKeyframeChannel? type = GetKeyframeType(prop.Name);
                if (!type.HasValue)
                {
                    continue;
                }

                ParseKeyframes(prop.Value, animation, partId, type.Value);
            }
        }

        private static ActorKeyframeChannel? GetKeyframeType(string name)
        {
            return name switch
            {
                "Position" => ActorKeyframeChannel.Position,
                "Rotation" => ActorKeyframeChannel.Rotation,
                _ => null
            };
        }

        private static void ParseKeyframes(JsonElement element, JsonActorAnimation animation, string partId, ActorKeyframeChannel type)
        {
            KeyframeCollection<ActorPartKeyframe> keyframes = KeyframeCollection<ActorPartKeyframe>.FromJson(element, element =>
            {
                Vector3 value = DeserializationHelper.GetVector3(element);
                if (type == ActorKeyframeChannel.Rotation)
                {
                    value.X = MathHelper.ToRadians(value.X);
                    value.Y = MathHelper.ToRadians(value.Y);
                    value.Z = MathHelper.ToRadians(value.Z);
                }
                return new ActorPartKeyframe(value);
            });
            animation.SetKeyframes(partId, type, keyframes);
        }

        /// <summary>
        /// Sets the keyframes of this <see cref="JsonActorAnimation"/>.
        /// </summary>
        /// <param name="partId">The ID of the part to set the keyframes for.</param>
        /// <param name="channel">The channel to set.</param>
        /// <param name="keyframes">The keyframes.</param>
        /// <returns>this</returns>
        public JsonActorAnimation SetKeyframes(string partId, ActorKeyframeChannel channel, KeyframeCollection<ActorPartKeyframe> keyframes)
        {
            if (!_keyframes.TryGetValue(partId, out KeyframeCollection<ActorPartKeyframe>[] partKeyframes))
            {
                partKeyframes = new KeyframeCollection<ActorPartKeyframe>[ActorAnimation._typeCount];
                _keyframes.Add(partId, partKeyframes);
            }

            partKeyframes[(int)channel] = keyframes.Clone();

            if (Length < keyframes.Length)
            {
                Length = keyframes.Length;
            }
            return this;
        }

        /// <summary>
        /// Sets the events for this <see cref="JsonActorAnimation"/>.
        /// </summary>
        /// <param name="keyframes">The event keyframes.</param>
        /// <returns>this</returns>
        public JsonActorAnimation SetEvents(KeyframeCollection<string> keyframes)
        {
            _events = keyframes.Clone();
            if (keyframes.Length > Length)
            {
                Length = keyframes.Length;
            }
            return this;
        }

        /// <summary>
        /// Converts this <see cref="JsonActorAnimation"/> to a useable <see cref="ActorAnimation"/>.
        /// </summary>
        /// <param name="model">The model this animation will animate.</param>
        /// <returns>The created animation.</returns>
        public ActorAnimation ToActorAnimation(ActorModel model)
        {
            ActorAnimation animation = new ActorAnimation(model);
            foreach (KeyValuePair<string, KeyframeCollection<ActorPartKeyframe>[]> pair in _keyframes)
            {
                int index = pair.Key == "Model" ? -1 : model.GetPartIndex(pair.Key);

                for (int i = 0; i < pair.Value.Length; i++)
                {
                    KeyframeCollection<ActorPartKeyframe> keyframes = pair.Value[i];
                    // Keyframes will be null if not specified.
                    if (keyframes == null)
                    {
                        continue;
                    }

                    animation.SetKeyframes(index, (ActorKeyframeChannel)i, keyframes);
                }
            }

            if (_events != null)
            {
                animation.SetEvents(_events);
            }

            return animation;
        }

        /// <summary>
        /// Creates a new, empty <see cref="JsonActorAnimation"/>.
        /// </summary>
        public JsonActorAnimation()
        {
            _keyframes = new Dictionary<string, KeyframeCollection<ActorPartKeyframe>[]>();
        }
    }
}
