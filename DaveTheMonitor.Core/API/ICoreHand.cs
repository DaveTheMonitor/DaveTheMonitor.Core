using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.API
{
    public interface ICoreHand
    {
        /// <summary>
        /// The game's <see cref="ITMHand"/> implementation. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        ITMHand TMHand { get; }

        /// <summary>
        /// This hand's owner.
        /// </summary>
        ICoreActor Owner { get; }

        /// <summary>
        /// This hand's owner as a <see cref="ICorePlayer"/>.
        /// </summary>
        /// <remarks>This can be null if the owner is not a player.</remarks>
        ICorePlayer Player { get; }

        /// <summary>
        /// True if this hand's owner is a player.
        /// </summary>
        bool IsPlayer { get; }

        /// <summary>
        /// The item held by this hand.
        /// </summary>
        CoreItem Item { get; }

        /// <summary>
        /// This hand's slot ID.
        /// </summary>
        int HandIndex { get; }

        /// <summary>
        /// This hand's type.
        /// </summary>
        InventoryHand HandType { get; }

        /// <summary>
        /// True if this hand is swinging.
        /// </summary>
        bool IsSwinging { get; }

        /// <summary>
        /// Sets this hand's held item.
        /// </summary>
        /// <param name="item"></param>
        void SetItem(CoreItem item);
    }
}
