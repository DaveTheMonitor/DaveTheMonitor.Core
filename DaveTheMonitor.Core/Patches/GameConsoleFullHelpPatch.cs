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
            CommandRegistry register = CorePlugin.Instance._game.CommandRegister;
            if (!register.CommandExists(cmd))
            {
                return true;
            }

            CommandInfo info = register.GetCommand(cmd);
            __result = info.GetFullHelpString();
            return false;
        }
    }
}
