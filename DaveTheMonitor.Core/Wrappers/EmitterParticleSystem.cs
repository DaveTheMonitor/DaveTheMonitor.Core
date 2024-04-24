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
    internal struct EmitterParticleSystem
    {
        public static Type Type { get; private set; }
        private static Action<object, ITMMap> _initialize;
        private static Action<object> _loadContent;

        public object EmitterParticleSystemObject { get; private set; }

        static EmitterParticleSystem()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.EmitterParticleSystem");
            _initialize = AccessTools.Method(Type, "Initialize").CreateInvoker<Action<object, ITMMap>>();
            _loadContent = AccessTools.Method(Type, "LoadContent").CreateInvoker<Action<object>>();
        }

        public void Initialize(ICoreMap map)
        {
            _initialize(EmitterParticleSystemObject, map.TMMap);
        }

        public void LoadContent()
        {
            _loadContent(EmitterParticleSystemObject);
        }

        public EmitterParticleSystem()
        {
            EmitterParticleSystemObject = Type.CreateInstance();
        }

        public EmitterParticleSystem(GameObjectBase EmitterParticleSystem)
        {
            if (EmitterParticleSystem.GetType() != Type)
            {
                throw new ArgumentException("EmitterParticleSystem is invalid.", nameof(EmitterParticleSystem));
            }
            EmitterParticleSystemObject = EmitterParticleSystem;
        }
    }
}
