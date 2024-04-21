using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Shaders
{
    public delegate void ShaderModifier<T>(ICorePlayer player, ITMPlayer virtualPlayer, ref T parameter);
}
