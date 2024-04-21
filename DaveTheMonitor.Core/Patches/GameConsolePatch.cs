using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.GameConsole", "RunCommand")]
    internal static class GameConsolePatch
    {
        public static bool Prefix(string command, ITMGame ___game, ITMPlayer ___origPlayer, ITMPlayer ___player, IOutputLog ___consoleWin)
        {
            string[] split = command.Split(";", System.StringSplitOptions.TrimEntries);
            foreach (string cmd in split)
            {
                int index = cmd.IndexOf(' ');
                if (index == -1)
                {
                    index = cmd.Length;
                }

                string name = cmd.Substring(0, index);
                CommandRegistry register = CorePlugin.Instance._game.CommandRegister;
                if (!register.CommandExists(name))
                {
                    return true;
                }

                ICorePlayer origPlayer = CorePlugin.Instance.Game.GetPlayer(___origPlayer);
                ICorePlayer player = CorePlugin.Instance.Game.GetPlayer(___player);
                bool error = false;
                CommandArgs args = index != cmd.Length ? CommandArgs.FromString(cmd.Substring(index + 1), out error) : null;
                if (error)
                {
                    ___consoleWin.WriteLine("There was an error parsing arguments.");
                    return false;
                }
                register.RunCommand(name, origPlayer, ___consoleWin, args);
            }
            return false;
        }
    }
}
