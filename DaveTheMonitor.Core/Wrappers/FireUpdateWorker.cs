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
    internal struct FireUpdateWorker
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public IThreadWorkItem FireUpdateWorkerObject { get; private set; }

        static FireUpdateWorker()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.FireUpdateWorker");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(PriorityLevel)
            });
        }

        public FireUpdateWorker(ICoreGame game, PriorityLevel priority)
        {
            FireUpdateWorkerObject = (IThreadWorkItem)_ctor.Invoke(new object[] { game.TMGame, priority });
        }

        public FireUpdateWorker(IThreadWorkItem fireUpdateWorker)
        {
            if (fireUpdateWorker.GetType() != Type)
            {
                throw new ArgumentException("FireUpdateWorker is invalid.", nameof(fireUpdateWorker));
            }
            FireUpdateWorkerObject = fireUpdateWorker;
        }
    }
}
