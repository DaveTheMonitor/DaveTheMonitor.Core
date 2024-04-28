using Accessibility;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
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
    internal struct RockLayerTransitionMap
    {
        public static Type Type { get; private set; }
        private static Action<object, Map, byte> _generate;
        private static Func<object, int, int, int, byte> _getValue;
        private static AccessTools.FieldRef<object, byte[]> _mapField;
        private static AccessTools.FieldRef<object, byte> _rangeField;
        private static AccessTools.FieldRef<object, Point> _sizeField;

        public object RockLayerTransitionMapObject { get; private set; }

        public byte[] Map
        {
            get => _mapField.Invoke(RockLayerTransitionMapObject);
            set => _mapField.Invoke(RockLayerTransitionMapObject) = value;
        }

        public byte Range
        {
            get => _rangeField.Invoke(RockLayerTransitionMapObject);
            set => _rangeField.Invoke(RockLayerTransitionMapObject) = value;
        }

        public Point Size
        {
            get => _sizeField.Invoke(RockLayerTransitionMapObject);
            set => _sizeField.Invoke(RockLayerTransitionMapObject) = value;
        }

        static RockLayerTransitionMap()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Renderers.MapRenderer");
            Type player = AccessTools.TypeByName("StudioForge.TotalMiner.Player");
            _generate = AccessTools.Method(Type, "Generate").CreateInvoker<Action<object, Map, byte>>();
            _getValue = AccessTools.Method(Type, "GetValue").CreateInvoker<Func<object, int, int, int, byte>>();
            _mapField = AccessTools.FieldRefAccess<byte[]>(Type, "Map");
            _rangeField = AccessTools.FieldRefAccess<byte>(Type, "Range");
            _sizeField = AccessTools.FieldRefAccess<Point>(Type, "Size");
        }

        public void Generate(Map map, byte range)
        {
            _generate(RockLayerTransitionMapObject, map, range);
        }

        public byte GetValue(int x, int z, int d)
        {
            return _getValue(RockLayerTransitionMapObject, x, z, d);
        }

        public RockLayerTransitionMap()
        {
            RockLayerTransitionMapObject = Type.CreateInstance();
        }
    }
}
