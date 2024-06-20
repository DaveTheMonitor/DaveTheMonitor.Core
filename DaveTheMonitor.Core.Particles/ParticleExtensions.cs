using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.Core.Particles
{
    public static class ParticleExtensions
    {
        public static ParticleManager ParticleManager(this ICoreWorld world)
        {
            return world.GetData<ParticleWorldData>().ParticleManager;
        }

        public static ParticleRegistry ParticleRegistry(this ICoreGame game)
        {
            return game.GetData<ParticleGameData>().ParticleRegistry;
        }
    }
}
