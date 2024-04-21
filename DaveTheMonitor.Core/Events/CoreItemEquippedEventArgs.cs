using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Events
{
    public struct CoreItemEquippedEventArgs
    {
        public ICoreActor Actor { get; private set; }
        public CoreItem Item { get; private set; }
        public EquipIndex Slot { get; private set; }

        public CoreItemEquippedEventArgs(ICoreActor actor, CoreItem item, EquipIndex slot)
        {
            Actor = actor;
            Item = item;
            Slot = slot;
        }
    }
}
