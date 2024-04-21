using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using StudioForge.Engine.Integration;

namespace DaveTheMonitor.Core.Effects
{
    internal class EffectsCommands
    {
        [ConsoleCommand("addeffect", "Adds an effect to the target.", "Adds an actor effect to the target.", "effect")]
        [ConsoleCommandArg(nameof(id), "id", "The id of the effect to add.", true, "i")]
        [ConsoleCommandArg(nameof(duration), "duration", "The duration of the effect.", false, "d")]
        [ConsoleCommandArg(nameof(target), "target", "Specifies that the effect should be added to the player's target.", false, "t")]
        [ConsoleCommandArg(nameof(allowMultiple), "allow-multiple", "Specifies that the effect can be added if the target already has it.", false, "m")]
        public static void AddEffect(ICorePlayer player, IOutputLog log, string id, float? duration, bool? target, bool? allowMultiple)
        {
            duration ??= 5;
            target ??= false;
            allowMultiple ??= false;
            ICoreActor t = target == true ? player.ActorInReticle : player;
            if (t == null)
            {
                log?.WriteLine($"Cannot add effect; there is no target.");
                return;
            }

            ActorEffect effect = t.Effects().Add(id, duration.Value, allowMultiple.Value);
            if (effect != null)
            {
                log?.WriteLine($"Added {id} for {duration} seconds to {t.Name}.");
            }
            else
            {
                if (player.Game.EffectRegistry().GetDefinition(id) == null)
                {
                    log?.WriteLine($"Cannot add effect; effect {id} does not exist.");
                }
                else
                {
                    log?.WriteLine("Cannot add effect; unknown error.");
                }
            }
        }

        [ConsoleCommand("cleareffect", "Clears an effect from the target.", "Clears the specified effect from the target, or all effects if none is specified.", "ceffect")]
        [ConsoleCommandArg(nameof(id), "id", "The id of the effect to clear.", false, "i")]
        [ConsoleCommandArg(nameof(target), "target", "Specifies that the effects should be cleared from the player's target.", false, "t")]
        public static void ClearEffect(ICorePlayer player, IOutputLog log, string id, bool? target)
        {
            target ??= false;
            ICoreActor t = target == true ? player.ActorInReticle : player;
            if (t == null)
            {
                log?.WriteLine($"Cannot remove effect; there is no target.");
                return;
            }

            if (id != null)
            {
                if (t.Effects().Remove(id))
                {
                    log?.WriteLine($"Removed {id} from {t.Name}.");
                }
                else
                {
                    log?.WriteLine($"Cannot remove effect; the target does not have the specified effect.");
                }
            }
            else
            {
                t.Effects().Clear();
                log?.WriteLine($"Cleared all effects from {t.Name}.");
            }
        }
    }
}
