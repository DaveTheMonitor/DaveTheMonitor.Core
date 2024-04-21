using DaveTheMonitor.Core.API;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Biomes
{
    public abstract class DecorationDefinition : IDefinition
    {
        public abstract string Id { get; }
        public int NumId { get; set; }
        public abstract ITMMap Component { get; }
        public virtual bool CanRotate => true;
        public virtual GlobalPoint3D Origin => GlobalPoint3D.Zero;
        public virtual Map.CopyType CopyType => Map.CopyType.NoOverwrite;
        protected ICoreGame Game { get; private set; }

        public virtual void OnRegister(ICoreMod mod)
        {
            
        }

        public GlobalPoint3D GetRotatedOffset(CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.North => GetNorthOffset(),
                CardinalDirection.East => GetEastOffset(),
                CardinalDirection.South => GetSouthOffset(),
                CardinalDirection.West => GetWestOffset(),
                _ => Origin
            };
        }

        private GlobalPoint3D GetNorthOffset()
        {
            return new GlobalPoint3D(-Origin.X, -Origin.Y, -Origin.Z);
        }

        private GlobalPoint3D GetEastOffset()
        {
            return new GlobalPoint3D(Origin.Z, -Origin.Y, -Origin.X);
        }

        private GlobalPoint3D GetSouthOffset()
        {
            return new GlobalPoint3D(Origin.X, -Origin.Y, Origin.Z);
        }

        private GlobalPoint3D GetWestOffset()
        {
            return new GlobalPoint3D(-Origin.Z, -Origin.Y, Origin.X);
        }

        public void SetGame(ICoreGame game)
        {
            Game = game;
        }
    }
}
