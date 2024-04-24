using Accessibility;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Wrappers
{
    internal struct CreativeModeHelper
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public object CreativeModeHelperObject { get; private set; }

        static CreativeModeHelper()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.CreativeModeHelper");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                AccessTools.TypeByName("StudioForge.TotalMiner.MapTM")
            });
        }

        public CreativeModeHelper(ICoreGame game, ICoreMap map)
        {
            CreativeModeHelperObject = _ctor.Invoke(new object[] { game.TMGame, map.TMMap });
        }

        public CreativeModeHelper(object creativeModeHelper)
        {
            if (creativeModeHelper.GetType() != Type)
            {
                throw new ArgumentException("CreativeModeHelper is invalid.", nameof(creativeModeHelper));
            }
            CreativeModeHelperObject = creativeModeHelper;
        }
    }
}
