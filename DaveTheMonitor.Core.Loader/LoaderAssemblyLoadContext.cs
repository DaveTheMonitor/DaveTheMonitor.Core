using System.Reflection;
using System.Runtime.Loader;

namespace DaveTheMonitor.Core.Loader
{
    internal sealed class LoaderAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return LoaderPlugin.Instance.Resolve(assemblyName);
        }

        public LoaderAssemblyLoadContext(string name, bool isCollectible = false) : base(name, isCollectible)
        {

        }
    }
}
