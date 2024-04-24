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
    internal struct ParticleEmitterWorker
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public IThreadWorkItem ParticleEmitterWorkerObject { get; private set; }

        static ParticleEmitterWorker()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.ParticleEmitterWorker");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(PriorityLevel)
            });
        }

        public ParticleEmitterWorker(ICoreGame game, PriorityLevel priority)
        {
            ParticleEmitterWorkerObject = (IThreadWorkItem)_ctor.Invoke(new object[] { game.TMGame, priority });
        }

        public ParticleEmitterWorker(IThreadWorkItem particleEmitterWorker)
        {
            if (particleEmitterWorker.GetType() != Type)
            {
                throw new ArgumentException("ParticleEmitterWorker is invalid.", nameof(particleEmitterWorker));
            }
            ParticleEmitterWorkerObject = particleEmitterWorker;
        }
    }
}
