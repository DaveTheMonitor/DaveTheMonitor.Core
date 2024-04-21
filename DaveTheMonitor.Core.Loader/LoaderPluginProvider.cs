using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Loader
{
    internal sealed class LoaderPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin() => new LoaderPlugin();
        public ITMPluginArcade GetPluginArcade() => null;
        public ITMPluginBiome GetPluginBiome() => null;
        public ITMPluginBlocks GetPluginBlocks() => null;
        public ITMPluginConfig GetPluginConfig() => null;
        public ITMPluginGUI GetPluginGUI() => null;
        public ITMPluginNet GetPluginNet() => null;
    }
}