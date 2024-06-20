using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Causes an <see cref="ICoreData{T}"/> implementation to be added to all players.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class PlayerDataAttribute : Attribute
    {
        public PlayerDataAttribute()
        {
            
        }
    }
}
