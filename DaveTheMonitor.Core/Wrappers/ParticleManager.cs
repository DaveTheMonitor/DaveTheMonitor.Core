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
    internal struct ParticleManager
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public GameObjectBase ParticleManagerObject { get; private set; }

        static ParticleManager()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.ParticleManager");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                AccessTools.TypeByName("StudioForge.TotalMiner.MapTM")
            });
        }

        public ParticleManager(ICoreGame game, ICoreMap map)
        {
            ParticleManagerObject = (GameObjectBase)_ctor.Invoke(new object[] { game.TMGame, map.TMMap });
        }

        public ParticleManager(GameObjectBase ParticleManager)
        {
            if (ParticleManager.GetType() != Type)
            {
                throw new ArgumentException("ParticleManager is invalid.", nameof(ParticleManager));
            }
            ParticleManagerObject = ParticleManager;
        }
    }
}
