using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core
{
    internal sealed class Hand : ICoreHand
    {
        public ITMHand TMHand { get; private set; }
        public ICoreActor Owner { get; private set; }
        public ICorePlayer Player { get; private set; }
        public bool IsPlayer => Owner.IsPlayer;
        public CoreItem Item => Owner.Game.ItemRegistry.GetItem(TMHand.ItemID);
        public int HandIndex => TMHand.HandIndex;
        public InventoryHand HandType => TMHand.HandType;
        public bool IsSwinging => TMHand.IsSwinging;

        public void SetItem(CoreItem item)
        {
            TMHand.SetItem(item.ItemType);
        }

        public Hand(ICoreActor owner, ITMHand hand)
        {
            Owner = owner;
            Player = owner as ICorePlayer;
            TMHand = hand;
        }
    }
}
