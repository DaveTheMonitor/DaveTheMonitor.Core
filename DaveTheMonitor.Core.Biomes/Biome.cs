using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Biomes
{
    public abstract class Biome : IDefinition
    {
        public abstract string Id { get; }
        public int NumId { get; set; }
        public abstract string DisplayName { get; }
        public Color Color { get; private set; }
        public float Temperature { get; private set; }
        public float Precipitation { get; private set; }

        public abstract void OnRegister(ICoreMod mod);

        public virtual bool HasFog => false;
        public virtual Color GetFogColor(ICoreWorld world) => Color.Transparent;
        public virtual float GetFogDistance(ICoreWorld world) => 100;
        public virtual Color GetTintColor(ICoreWorld world) => Color.Transparent;
        public virtual string ParticleEmitter => null;

        public abstract BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param);

        public virtual int GetRockDepth(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            return 4;
        }

        public virtual int GetGroundHeight(BiomeManager biomeManager, GlobalPoint3D p)
        {
            return p.Y;
        }

        public virtual bool CanPlaceDecoration(BiomeManager biomeManager, GlobalPoint3D p, Block block)
        {
            return block != Block.None;
        }

        protected void PlaceDecoration(string decorationId, GlobalPoint3D p, BiomeGenerationParams param)
        {
            DecorationRegistry register = CorePlugin.Instance.Game.DecorationRegistry();
            DecorationDefinition decoration = register.GetDefinition(decorationId);
            if (decoration == null)
            {
                return;
            }

            Map com = (Map)decoration.Component;
            CardinalDirection direction = decoration.CanRotate ? (CardinalDirection)param.Random.Next(4) : CardinalDirection.North;
            GlobalPoint3D offset = decoration.GetRotatedOffset(direction);
            com.CopyTo(param.Map, GlobalPoint3D.One, p + offset, com.MapSize, GlobalPoint3D.Zero, GlobalPoint3D.Zero, (int)direction, UpdateBlockMethod.Generation, decoration.CopyType, Map.CopyAccess.Full, GamerID.Sys1, false, null);
            //param.Map.SetBlockData(p, (byte)Block.BlueBox, 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
        }

        public virtual void PlaceDecoration(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {

        }

        public virtual void OnBiomeEnter(ICoreActor actor)
        {

        }

        public virtual void OnBiomeExit(ICoreActor actor)
        {

        }

        public Biome(float temperature, float precipitation, Color color)
        {
            Temperature = temperature;
            Precipitation = precipitation;
            Color = color;
        }
    }
}
