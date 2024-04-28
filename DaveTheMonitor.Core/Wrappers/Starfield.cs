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
    public struct Starfield
    {
        public static Type Type { get; private set; }
        private static ConstructorInfo _ctor;
        private static AccessTools.FieldRef<object, VertexBuffer> _vertexBuffer;

        public IHasContent StarfieldObject { get; private set; }
        public VertexBuffer VertexBuffer => _vertexBuffer(StarfieldObject);

        static Starfield()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.Starfield");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance")
            });
            _vertexBuffer = AccessTools.FieldRefAccess<VertexBuffer>(Type, "VertexBuffer");
        }

        public Starfield(ICoreGame game)
        {
            StarfieldObject = (IHasContent)_ctor.Invoke(new object[] { game.TMGame });
        }

        public Starfield(IHasContent starfield)
        {
            if (starfield.GetType() != Type)
            {
                throw new ArgumentException("Starfield is invalid.", nameof(starfield));
            }
            StarfieldObject = starfield;
        }
    }
}
