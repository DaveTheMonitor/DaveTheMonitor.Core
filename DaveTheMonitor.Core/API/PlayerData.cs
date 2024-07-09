using StudioForge.TotalMiner;
using System.IO;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Base class for player data that can listen to events. If your data doesn't require any events, consider using <see cref="ICoreData{T}"/> instead.
    /// </summary>
    public abstract class PlayerData : ActorData
    {
        /// <summary>
        /// The player this data belongs to.
        /// </summary>
        public ICorePlayer Player
        {
            get
            {
                return _player ??= (ICorePlayer)Actor;
            }
        }
        private ICorePlayer _player;

        /// <summary>
        /// Creates a new <see cref="PlayerData"/> instance.
        /// </summary>
        public PlayerData() : base()
        {

        }
    }
}
