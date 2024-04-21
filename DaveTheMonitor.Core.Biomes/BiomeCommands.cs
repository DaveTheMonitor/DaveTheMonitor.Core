using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Biomes;
using DaveTheMonitor.Core.Commands;
using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Diagnostics;
using System.IO;

namespace DaveTheMonitor.Core.Particles
{
    internal class BiomeCommands
    {
        [ConsoleCommand("savebiometex", "Saves the biome textures to the world folder.", "Saves the textures for the temperature, precipitation, and biome maps to the world folder.", "sbt")]
        [ConsoleCommandArg(nameof(open), "open-folder", "Opens the world folder if the game is not in fullscreen.", false, "o")]
        public static void GenerateTree(ICorePlayer player, IOutputLog log, bool? open)
        {
            try
            {
                string path = player.World.FullPath;
                string fullPath = Utils.StartsWithDriveLetter(path) ? path : Path.Combine(FileSystem.RootPath, path);
                if (!Directory.Exists(fullPath))
                {
                    log.WriteLine("The world must be saved before running savestate.");
                    return;
                }

                BiomesPlugin.Instance.SaveTextures(fullPath);
                log.WriteLine("Biome textures saved.");

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
    }
}
