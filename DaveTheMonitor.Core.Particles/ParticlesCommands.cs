using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Commands;
using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;

namespace DaveTheMonitor.Core.Particles
{
    internal class ParticlesCommands
    {
        [ConsoleCommand("spawnparticle", "Spawns a particle.", "Spawns a particle at the player's position or specified position.", "particle")]
        [ConsoleCommandArg(nameof(id), "id", "The id of the particle to spawn.", true, "i")]
        [ConsoleCommandArg(nameof(position), "position", "The position to spawn the particle at. The player position is used if this is omitted.", false, "p")]
        [ConsoleCommandArg(nameof(velocity), "velocity", "The velocity of the particle.", false, "v")]
        [ConsoleCommandArg(nameof(count), "count", "The number of particles to spawn.", false, "c")]
        public static void SpawnParticle(ICorePlayer player, IOutputLog log, string id, Vector3? position, Vector3? velocity, int? count)
        {
            position ??= player.Position;
            velocity ??= Vector3.Zero;
            count ??= 1;
            ParticleManager particleManager = player.World.ParticleManager();

            ParticleInstance instance = null;
            int spawned = 0;
            for (int i = 0; i < count; i++)
            {
                if (player.World.SpawnParticle(id, position.Value, velocity.Value, out object instanceObj))
                {
                    spawned++;
                    instance ??= (ParticleInstance)instanceObj;
                }
            }
            if (spawned != 0)
            {
                if (instance != null)
                {
                    log?.WriteLine($"Spawned {count} Particles Module {id}, {instance.Id}");
                }
                else
                {
                    log?.WriteLine($"Spawned {count} TM {id}");
                }
            }
            else
            {
                if (player.Game.ParticleRegistry().GetDefinition(id) == null)
                {
                    log?.WriteLine($"Cannot spawn particle; particle {id} does not exist.");
                }
                else if (particleManager.ActiveParticles >= particleManager.MaxParticles)
                {
                    log?.WriteLine("Cannot spawn particle; particle limit reached.");
                }
                else
                {
                    log?.WriteLine("Cannot spawn particle; unknown error.");
                }
            }
        }

        [ConsoleCommand("destroyallparticles", "Destroys all particles.", "Destroys all particles.", "dparticles")]
        public static void DestroyAllParticles(ICorePlayer player, IOutputLog log)
        {
            player.World.ParticleManager().DestroyAllParticles();
            log?.WriteLine("Destroyed all particles.");
        }

        [ConsoleCommand("maxparticles", "Sets the max number of particles.", "Sets the max number of particles.", "mparticles")]
        [ConsoleCommandArg(nameof(count), "count", "The max number of particles. If omitted, the current max particles is written.", false, "c")]
        public static void MaxParticles(ICorePlayer player, IOutputLog log, int? count)
        {
            if (!count.HasValue)
            {
                log?.WriteLine($"Mas particles: {player.World.ParticleManager().MaxParticles}");
                return;
            }

            ParticleManager particleManager = player.World.ParticleManager();
            if (particleManager.IsValidMax(count.Value))
            {
                particleManager.SetMaxParticles(count.Value);
                log?.WriteLine($"Set max particle count to {count}");
            }
            else
            {
                log?.WriteLine($"Max particle count must be a multiple of {ParticleManager.ParticleChunk.Count}");
            }
        }
    }
}
