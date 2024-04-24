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
    internal struct MapRenderer
    {
        public static Type Type { get; private set; }
        private static Action<object, ITMPlayer, ITMPlayer> _draw;
        private static Action<object, ITMPlayer, ITMPlayer> _setShaderParams;
        private static Action<object, ITMPlayer, ITMPlayer> _drawMapChunks;
        private static AccessTools.FieldRef<object, object> _fieldRef;
        private static ConstructorInfo _ctor;

        public DrawableGameObjectBase MapRendererObject { get; private set; }
        public ITMMap Map
        {
            get => (ITMMap)_fieldRef.Invoke(MapRendererObject);
            set => _fieldRef.Invoke(MapRendererObject) = value;
        }

        static MapRenderer()
        {
            Type = AccessTools.TypeByName("StudioForge.TotalMiner.Renderers.MapRenderer");
            Type player = AccessTools.TypeByName("StudioForge.TotalMiner.Player");
            _ctor = AccessTools.Constructor(Type, new Type[]
            {
                AccessTools.TypeByName("StudioForge.TotalMiner.GameInstance"),
                typeof(IProgressBar)
            });
            _draw = AccessTools.Method(Type, "Draw", new Type[]
            {
                player,
                player
            }).CreateInvoker<Action<object, ITMPlayer, ITMPlayer>>();
            _setShaderParams = AccessTools.Method(Type, "SetShaderParams").CreateInvoker<Action<object, ITMPlayer, ITMPlayer>>();
            _drawMapChunks = AccessTools.Method(Type, "DrawMapChunks", new Type[]
            {
                player,
                player
            }).CreateInvoker<Action<object, ITMPlayer, ITMPlayer>>();
            _fieldRef = AccessTools.FieldRefAccess<object>(Type, "map");
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer)
        {
            _draw(MapRendererObject, player.TMPlayer, virtualPlayer);
        }

        public void SetShaderParams(ICorePlayer player, ITMPlayer virtualPlayer)
        {
            _setShaderParams(MapRendererObject, player.TMPlayer, virtualPlayer);
        }

        public void DrawMapChunks(ICorePlayer player, ITMPlayer virtualPlayer)
        {
            _drawMapChunks(MapRendererObject, player.TMPlayer, virtualPlayer);
        }

        public MapRenderer(ICoreGame game, IProgressBar progressBar)
        {
            MapRendererObject = (DrawableGameObjectBase)_ctor.Invoke(new object[] { game.TMGame, progressBar });
        }

        public MapRenderer(DrawableGameObjectBase mapRenderer)
        {
            if (mapRenderer.GetType() != Type)
            {
                throw new ArgumentException("MapRenderer is invalid.", nameof(mapRenderer));
            }
            MapRendererObject = mapRenderer;
        }
    }
}
