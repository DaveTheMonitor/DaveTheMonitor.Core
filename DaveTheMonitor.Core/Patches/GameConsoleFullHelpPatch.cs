using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.GameConsoleHelp", "GetFullHelp")]
    internal static class GameConsoleFullHelpPatch
    {
        public static bool Prefix(string cmd, ref string __result)
        {
            if (!CorePlugin.IsValid)
            {
                return true;
            }

            CommandRegistry registry = CorePlugin.Instance._game.CommandRegistry;
            if (!registry.CommandExists(cmd))
            {
                return true;
            }

            CommandInfo info = registry.GetCommand(cmd);
            __result = info.GetFullHelpString();
            return false;
        }
    }
}
