using DaveTheMonitor.Core.API;
using HarmonyLib;
using StudioForge.TotalMiner.API;
using System;
using System.IO;
using System.Reflection;

namespace DaveTheMonitor.Core.Storage
{
    internal sealed class MapComponentLoader : IMapComponentLoader
    {
        private object _voxelModelManager;
        private static Type _voxelModelManagerType;
        private static MethodInfo _readComponentDataMethod;
        private static FastInvokeHandler _readComponentDataInvoker;

        static MapComponentLoader()
        {
            _voxelModelManagerType = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.VoxelModelManager");
            _readComponentDataMethod = AccessTools.Method(_voxelModelManagerType, "ReadComponentData");
            _readComponentDataInvoker = MethodInvoker.GetHandler(_readComponentDataMethod);
        }

        public ITMMap LoadComponent(string path)
        {
            using Stream stream = File.OpenRead(path);
            using BinaryReader reader = new BinaryReader(stream);
            return LoadComponent(reader);
        }

        public ITMMap LoadComponent(Stream stream)
        {
            using BinaryReader reader = new BinaryReader(stream);
            return LoadComponent(reader);
        }

        public ITMMap LoadComponent(BinaryReader reader)
        {
            int version = reader.ReadInt32();
            ITMMap map = (ITMMap)_readComponentDataInvoker(_voxelModelManager, reader, version);
            return map;
        }

        public MapComponentLoader(ICoreGame game) : this(game.TMGame)
        {
            
        }

        private MapComponentLoader(ITMGame game)
        {
            _voxelModelManager = new Traverse(game).Field("VoxelModelManager").GetValue();
        }
    }
}
