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
    internal struct ParticleModifiers
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public object ParticleModifiersObject { get; private set; }

        static ParticleModifiers()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.ParticleModifiers");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(Map)
            });
        }

        public ParticleModifiers(ICoreGame game, ICoreMap map)
        {
            ParticleModifiersObject = _ctor.Invoke(new object[] { game.TMGame, map.TMMap });
        }

        public ParticleModifiers(object particleModifiers)
        {
            if (particleModifiers.GetType() != Type)
            {
                throw new ArgumentException("ParticleModifiers is invalid.", nameof(particleModifiers));
            }
            ParticleModifiersObject = particleModifiers;
        }
    }
}
