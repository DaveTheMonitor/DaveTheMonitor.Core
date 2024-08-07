﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Scripts;
using DaveTheMonitor.Scripts.Compiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DaveTheMonitor.Core
{
    internal static class CoreCommands
    {
        private static MemoryStream _stream;

        [ConsoleCommand("writestate", "Writes the current game state to a memory stream.", "Writes the current game state to a memory stream. The memory stream can be output to the world folder with SaveState or loaded with ReadState.", "ws")]
        public static void WriteState(ICorePlayer player, IOutputLog log)
        {
            try
            {
                _stream?.Close();
                _stream = new MemoryStream();
                using BinaryWriter writer = new BinaryWriter(_stream, Encoding.Default, true);
                player.Game.WriteState(writer);
                log.WriteLine("Game state written.");
            }
            catch (Exception e)
            {
                Debugger.Break();
                log.WriteLine($"Exception: {e.Message}");
            }
        }

        [ConsoleCommand("savestate", "Saves the current game state in the memory stream to the world folder.", "Saves the current game state in the memory stream to the world folder. This can be used if the game state needs to be inspected for debugging.", "ss")]
        [ConsoleCommandArg(nameof(open), "open-folder", "Opens the world folder if the game is not in fullscreen.", false, "o")]
        public static void SaveState(ICorePlayer player, IOutputLog log, bool? open)
        {
            try
            {
                if (_stream == null || !_stream.CanRead)
                {
                    log.WriteLine("There is no stream to read. Use writestate first.");
                    return;
                }
                string path = player.World.FullPath;
                string fullPath = Utils.StartsWithDriveLetter(path) ? path : Path.Combine(FileSystem.RootPath, path);
                if (!Directory.Exists(fullPath))
                {
                    log.WriteLine("The world must be saved before running savestate.");
                    return;
                }

                File.WriteAllBytes(Path.Combine(fullPath, "gamestate_out.dat"), _stream.ToArray());
                log.WriteLine("Game state saved.");

                if (open == true)
                {
                    if (Services.GetService<GraphicsDeviceManager>()?.IsFullScreen == true)
                    {
                        log.WriteLine("Cannot open folder in fullscreen.");
                        return;
                    }

                    Process.Start("explorer.exe", fullPath);
                }
            }
            catch (Exception e)
            {
                Debugger.Break();
                log.WriteLine($"Exception: {e.Message}");
            }
        }

        [ConsoleCommand("readstate", "Reads the current game state from the memory stream.", "Reads the current game state from the memory stream.", "rs")]
        public static void ReadState(ICorePlayer player, IOutputLog log)
        {
            try
            {
                if (_stream == null || !_stream.CanRead)
                {
                    log.WriteLine("There is no stream to read. Use writestate first.");
                    return;
                }
                _stream.Position = 0;
                using BinaryReader reader = new BinaryReader(_stream, Encoding.Default, true);
                player.Game.ReadState(reader, Globals1.SaveVersion, CoreGlobalData.CoreSaveVersion);
                log.WriteLine("Game state read.");
            }
            catch (Exception e)
            {
                Debugger.Break();
                log.WriteLine($"Exception: {e.Message}");
            }
        }

        [ConsoleCommand("closestate", "Closes the game state memory stream.", "Closes the game state memory stream. Use this when the stream is no longer needed.", "cs")]
        public static void CloseState(ICorePlayer player, IOutputLog log)
        {
            _stream?.Close();
            log.WriteLine("Closed stream.");
        }

        [ConsoleCommand("runscriptcommand", "Runs a single CSRScript command.", "Runs a single CSRScript command. Run multiple lines by separating them with '^^'", "scrc")]
        [ConsoleCommandArg(nameof(cmd), "cmd", "The command to run.", true, "c")]
        public static void RunScriptCommand(ICorePlayer player, IOutputLog log, string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return;
            }

            string[] lines = cmd.Split("^^");
            string src = $"\n{string.Join('\n', lines)}\n";
            ScriptCompiler compiler = new ScriptCompiler();
            compiler.SetSrc(src);
            Script script = compiler.Compile("tmp", ScriptRuntimeType.Mod, CompilerOptimization.Basic, new string[] { "totalminer" });
            if (script == null)
            {
                log.WriteLine("There was an error compiling the script.");
                return;
            }

            IScriptRuntime runtime = player.Game.ScriptRuntime;
            runtime.RunScript(script, new ScriptInVar("self", player), new ScriptInVar("world", player.World));
            log.WriteLine($"output: {runtime.ReturnedValue}");
        }

        [ConsoleCommand("runscriptfile", "Runs the CSRScript contained in the file.", "Runs the CSRScript contained in the specified file. The file is relative to '{ModPath}/Scripts'", "scrf")]
        [ConsoleCommandArg(nameof(modId), "mod", "The mod to run the script from.", true, "m")]
        [ConsoleCommandArg(nameof(file), "file", "The script file to run.", true, "f")]
        public static void RunScriptCommand(ICorePlayer player, IOutputLog log, string modId, string file)
        {
            ICoreGame game = player.Game;
            ICoreMod mod = game.ModManager.GetMod(modId);
            if (mod == null)
            {
                log.WriteLine($"Mod {modId} not found.");
                return;
            }

            string scriptsPath = Path.Combine(mod.FullPath, "Scripts");
            if (!Directory.Exists(scriptsPath))
            {
                log.WriteLine($"{modId} does not contain any scripts.");
                return;
            }

            if (!file.EndsWith(".scr") && !file.EndsWith(".txt"))
            {
                log.WriteLine("File must be a script.");
                return;
            }

            string name = Path.GetFileName(file);
            string scriptPath = Path.Combine(scriptsPath, file);
            if (!File.Exists(scriptPath))
            {
                log.WriteLine("Target script does not exist.");
                return;
            }

            string src = File.ReadAllText(scriptPath);
            ScriptCompiler compiler = new ScriptCompiler();
            compiler.ErrorHandler += (object sender, ScriptCompilerErrorEventArgs e) =>
            {
                log.WriteLine($"{e.Code} : {e.Header} : {e.Message}");
            };

            compiler.SetSrc(src);
            Script script = compiler.Compile("tmp", ScriptRuntimeType.Mod, CompilerOptimization.Basic, new string[] { "totalminer" });
            if (script == null)
            {
                log.WriteLine("There was an error compiling the script.");
                return;
            }

            IScriptRuntime runtime = player.Game.ScriptRuntime;
            if (runtime is ScriptRuntime r)
            {
                r.ErrorHandler += HandleRuntimeError;
            }

            runtime.RunScript(script, new ScriptInVar("self", player), new ScriptInVar("world", player.World));
            log.WriteLine($"output: {runtime.ReturnedValue}");

            if (runtime is ScriptRuntime r2)
            {
                r2.ErrorHandler -= HandleRuntimeError;
            }

            void HandleRuntimeError(object sender, ScriptErrorEventArgs e)
            {
                log.WriteLine($"{e.Code} : {e.Header} : {e.Message}");
            }
        }

        [ConsoleCommand("playsound", "Plays the specified sound asset.", "Plays the specified sound asset if it exists.", "sound")]
        [ConsoleCommandArg(nameof(id), "id", "The sound effect to play.", true, "i")]
        [ConsoleCommandArg(nameof(position), "position", "The position to play the sound at.", false, "p")]
        [ConsoleCommandArg(nameof(volume), "volume", "The volume to play the sound at.", false, "v")]
        [ConsoleCommandArg(nameof(pitch), "pitch", "The pitch to play the sound at.", false, "p")]
        [ConsoleCommandArg(nameof(pan), "pan", "The pan to play the sound at.", false)]
        public static void PlaySound(ICorePlayer player, IOutputLog log, string id, Vector3? position, float? volume, float? pitch, float? pan)
        {
            ICoreGame game = player.Game;
            SoundEffect sound = game.ModManager.LoadSound(CorePlugin.CoreMod, id);
            if (sound == null)
            {
                log.WriteLine($"Sound {id} not found. Are you missing a mod ID?");
                return;
            }

            volume ??= 1;
            pitch ??= 0;
            pan ??= 0;

            if (position.HasValue)
            {
                player.World.PlaySound(sound, position.Value, volume.Value, pitch.Value);
                log.WriteLine($"Playing sound {id} at {position}.");
            }
            else
            {
                player.World.PlaySound(sound, volume.Value, pitch.Value, pan.Value);
                log.WriteLine($"Playing sound {id}.");
            }
        }
    }
}
