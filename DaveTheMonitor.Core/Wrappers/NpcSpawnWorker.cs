using Accessibility;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Wrappers
{
    internal struct NpcSpawnWorker
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public IThreadWorkItem NpcSpawnWorkerObject { get; private set; }

        static NpcSpawnWorker()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.NpcSpawnWorker");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(PriorityLevel)
            });
        }

        public NpcSpawnWorker(ICoreGame game, PriorityLevel priority)
        {
            NpcSpawnWorkerObject = (IThreadWorkItem)_ctor.Invoke(new object[] { game.TMGame, priority });
        }

        public NpcSpawnWorker(IThreadWorkItem npcSpawnWorker)
        {
            if (npcSpawnWorker.GetType() != Type)
            {
                throw new ArgumentException("NpcSpawnWorker is invalid.", nameof(npcSpawnWorker));
            }
            NpcSpawnWorkerObject = npcSpawnWorker;
        }
    }
}
