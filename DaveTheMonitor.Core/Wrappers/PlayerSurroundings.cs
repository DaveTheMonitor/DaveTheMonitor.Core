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
    internal struct PlayerSurroundings
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public IThreadWorkItem PlayerSurroundingsObject { get; private set; }

        static PlayerSurroundings()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.PlayerSurroundings");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(PriorityLevel)
            });
        }

        public PlayerSurroundings(ICoreGame game, PriorityLevel priority)
        {
            PlayerSurroundingsObject = (IThreadWorkItem)_ctor.Invoke(new object[] { game.TMGame, priority });
        }

        public PlayerSurroundings(IThreadWorkItem playerSurroundings)
        {
            if (playerSurroundings.GetType() != Type)
            {
                throw new ArgumentException("PlayerSurroundings is invalid.", nameof(playerSurroundings));
            }
            PlayerSurroundingsObject = playerSurroundings;
        }
    }
}
