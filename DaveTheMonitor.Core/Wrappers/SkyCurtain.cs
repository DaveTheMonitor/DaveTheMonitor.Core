using Accessibility;
using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
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
    public struct SkyCurtain
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;
        private static AccessTools.FieldRef<object, Map> _map;
        private static AccessTools.FieldRef<object, VertexBuffer> _vertexBuffer;
        private static AccessTools.FieldRef<object, IndexBuffer> _indexBuffer;

        public IHasContent SkyCurtainObject { get; private set; }
        public Map Map
        {
            get => _map(SkyCurtainObject);
            set => _map(SkyCurtainObject) = value;
        }
        public VertexBuffer VertexBuffer => _vertexBuffer(SkyCurtainObject);
        public IndexBuffer IndexBuffer => _indexBuffer(SkyCurtainObject);

        static SkyCurtain()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.SkyCurtain");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance")
            });
            _map = AccessTools.FieldRefAccess<Map>(Type, "map");
            _vertexBuffer = AccessTools.FieldRefAccess<VertexBuffer>(Type, "VertexBuffer");
            _indexBuffer = AccessTools.FieldRefAccess<IndexBuffer>(Type, "IndexBuffer");
        }

        public SkyCurtain(ICoreGame game)
        {
            SkyCurtainObject = (IHasContent)_ctor.Invoke(new object[] { game.TMGame });
        }

        public SkyCurtain(IHasContent skyCurtain)
        {
            if (skyCurtain.GetType() != Type)
            {
                throw new ArgumentException("SkyCurtain is invalid.", nameof(skyCurtain));
            }
            SkyCurtainObject = skyCurtain;
        }
    }
}
