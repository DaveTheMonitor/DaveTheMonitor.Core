using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.GameConsole", "CmdHelp")]
    internal static class GameConsoleHelpPatch
    {
        public static void Postfix(string command, ITMGame game, ITMPlayer caller, ITMPlayer player, IOutputLog log)
        {
            if (command.Contains(' '))
            {
                return;
            }

            CommandRegistry registry = CorePlugin.Instance._game.CommandRegistry;
            foreach (CommandInfo cmd in registry)
            {
                log.WriteLine($"{cmd.Name}  -- {cmd.ShortHelp}");
            }
        }
    }
}
