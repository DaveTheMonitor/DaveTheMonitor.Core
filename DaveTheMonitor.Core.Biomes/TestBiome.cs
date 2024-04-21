using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Biomes
{
    [BiomeRegisterIgnore]
    public sealed class TestBiome : Biome
    {
        public override string Id => _id;
        public override string DisplayName => _id;
        private string _id;
        private Block _block;

        public override void OnRegister(ICoreMod mod)
        {
            
        }

        public override BlockAndAux GetBlock(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            if (p.Y <= param.GroundHeight)
            {
                return new BlockAndAux(_block, 0);
            }
            else
            {
                return BlockAndAux.None;
            }
        }

        public override bool CanPlaceDecoration(BiomeManager biomeManager, GlobalPoint3D p, Block block)
        {
            return block == _block;
        }

        public override void PlaceDecoration(BiomeManager biomeManager, GlobalPoint3D p, BiomeGenerationParams param)
        {
            return;
            DecorationRegistry register = CorePlugin.Instance.Game.DecorationRegistry();
            DecorationDefinition decoration = register.GetDefinition("Core.Test");
            Map com = (Map)decoration.Component;
            CardinalDirection direction = decoration.CanRotate ? (CardinalDirection)param.Random.Next(4) : CardinalDirection.North;
            GlobalPoint3D offset = decoration.GetRotatedOffset(direction);
            com.CopyTo(param.Map, GlobalPoint3D.One, p + offset, com.MapSize, GlobalPoint3D.Zero, GlobalPoint3D.Zero, (int)direction, UpdateBlockMethod.Generation, decoration.CopyType, Map.CopyAccess.Full, GamerID.Sys1, false, null);
            param.Map.SetBlockData(p, (byte)Block.BlueBox, 0, UpdateBlockMethod.Generation, GamerID.Sys1, false);
        }

        public TestBiome(string id, float temp, float prec, Color color, Block block) : base(temp, prec, color)
        {
            _id = id;
            _block = block;
        }
    }
}
