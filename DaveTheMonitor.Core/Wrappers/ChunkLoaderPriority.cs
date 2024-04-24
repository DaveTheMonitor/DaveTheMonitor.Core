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
    internal struct ChunkLoaderPriority
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;

        public IThreadWorkItem ChunkLoaderPriorityObject { get; private set; }

        static ChunkLoaderPriority()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.ChunkLoaderPriority");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(Map)
            });
        }

        public ChunkLoaderPriority(ICoreGame game, ICoreMap map)
        {
            ChunkLoaderPriorityObject = (IThreadWorkItem)_ctor.Invoke(new object[] { game.TMGame, map.TMMap });
        }

        public ChunkLoaderPriority(IThreadWorkItem chunkLoaderPriority)
        {
            if (chunkLoaderPriority.GetType() != Type)
            {
                throw new ArgumentException("ChunkLoaderPriority is invalid.", nameof(chunkLoaderPriority));
            }
            ChunkLoaderPriorityObject = chunkLoaderPriority;
        }
    }
}
