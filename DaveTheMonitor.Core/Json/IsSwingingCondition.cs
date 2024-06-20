using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using Microsoft.Xna.Framework;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Json
{
    /// <summary>
    /// A <see cref="JsonCondition"/> that tests if the actor is currently swinging.
    /// </summary>
    [JsonCondition("Core.IsSwinging")]
    public sealed class IsSwingingCondition : BooleanCondition
    {
        /// <summary>
        /// The hand to test. Both hands will be tested if this is <see cref="InventoryHand.None"/>.
        /// </summary>
        public InventoryHand Hand { get; private set; }

        /// <inheritdoc/>
        public override bool Evaluate(ICoreActor actor)
        {
            if (Hand == InventoryHand.None)
            {
                return actor.RightHand.IsSwinging || actor.LeftHand.IsSwinging;
            }

            ICoreHand hand = Hand == InventoryHand.Left ? actor.LeftHand : actor.RightHand;
            return hand.IsSwinging == Value;
        }

        /// <inheritdoc/>
        protected override void ReadFromJson(JsonElement element)
        {
            base.ReadFromJson(element);
            if (element.TryGetProperty("Hand", out JsonElement handElement))
            {
                if (handElement.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidOperationException("JsonCondition Hand must be a valid hand type: Left, Right, Both");
                }

                string hand = handElement.GetString();
                Hand = hand switch
                {
                    "Left" => InventoryHand.Left,
                    "Right" => InventoryHand.Right,
                    "Both" => InventoryHand.None,
                    _ => throw new InvalidOperationException("JsonCondition Hand must be a valid hand type: Left, Right, Both")
                };
            }
            else
            {
                Hand = InventoryHand.None;
            }
        }
    }
}
