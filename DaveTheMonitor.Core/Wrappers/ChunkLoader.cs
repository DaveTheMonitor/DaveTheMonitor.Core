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
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core.Wrappers
{
    internal struct ChunkLoader
    {
        public static Type Type { get; private set; }
        private static Action<object, object, ITMMap, ITMPlayer, bool> _initialize;
        private static AccessTools.FieldRef<object, ITMGame> _instance;
        private static AccessTools.FieldRef<object, ITMMap> _map;

        public IThreadWorkItem ChunkLoaderObject { get; private set; }
        public ITMGame Instance => _instance(ChunkLoaderObject);
        public ITMMap Map => _map(ChunkLoaderObject);

        static ChunkLoader()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.ChunkLoader");
            _initialize = AccessTools.Method(Type, "Initialize").CreateInvoker<Action<object, object, ITMMap, ITMPlayer, bool>>();
            _instance = AccessTools.FieldRefAccess<ITMGame>(Type, "instance");
            _map = AccessTools.FieldRefAccess<ITMMap>(Type, "map");
        }

        public void Initialize(ICoreGame instance, ICoreMap map, ICorePlayer player, bool isThreaded)
        {
            _initialize(ChunkLoaderObject, instance.TMGame, map.TMMap, player.TMPlayer, isThreaded);
        }

        public ChunkLoader()
        {
            ChunkLoaderObject = (IThreadWorkItem)Type.CreateInstance();
        }

        public ChunkLoader(IThreadWorkItem chunkLoader)
        {
            if (chunkLoader.GetType() != Type)
            {
                throw new ArgumentException("ChunkLoader is invalid.", nameof(chunkLoader));
            }
            ChunkLoaderObject = chunkLoader;
        }
    }
}
