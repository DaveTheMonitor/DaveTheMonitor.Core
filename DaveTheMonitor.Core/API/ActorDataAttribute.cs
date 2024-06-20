using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Causes an <see cref="ICoreData{T}"/> implementation to be added to all actors of the specified types, or all actors if <see cref="Actors"/> is null.
    /// </summary>
    /// <remarks>
    /// If you want to add player-specific data, use <see cref="PlayerDataAttribute"/> instead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ActorDataAttribute : Attribute
    {
        /// <summary>
        /// <para>The string IDs of the actors this data should be added to.</para>
        /// <para>To add this data to all actors, set this to null.</para>
        /// </summary>
        public string[] Actors { get; set; }

        public ActorDataAttribute()
        {
            Actors = null;
        }

        public ActorDataAttribute(params string[] actors)
        {
            Actors = actors;
        }
    }
}
