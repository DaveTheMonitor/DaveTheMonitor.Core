using DaveTheMonitor.Core.API;

namespace DaveTheMonitor.CSR
{
    [PluginEntry]
    public sealed class CSRPlugin : ICorePlugin
    {
        public static CSRPlugin Instance { get; private set; }
        public ICoreMod Mod { get; private set; }
        private ICoreGame _game;

        public void Initialize(ICoreMod mod)
        {
            Instance = this;
            Mod = mod;
        }

        public void InitializeGame(ICoreGame game)
        {
            _game = game;
        }
    }
}
