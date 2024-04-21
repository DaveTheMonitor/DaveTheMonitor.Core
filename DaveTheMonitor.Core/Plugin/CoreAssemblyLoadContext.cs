using System.Reflection;
using System.Runtime.Loader;

namespace DaveTheMonitor.Core.Plugin
{
    internal sealed class CoreAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Harmony, Core, Scripts
            Assembly assembly = CorePlugin.Instance.Resolve(assemblyName);
            if (assembly != null)
            {
                return assembly;
            }

            // Mods
            return CorePlugin.Instance._game.ModManager.Resolve(assemblyName);
        }

        public CoreAssemblyLoadContext(string name, bool isCollectible = false) : base(name, isCollectible)
        {

        }
    }
}
