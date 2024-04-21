using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.Biomes
{
    public struct BlockAndAux
    {
        public static BlockAndAux None => new BlockAndAux(Block.None, 0);
        public Block Block { get; set; }
        public byte Aux { get; set; }

        public BlockAndAux(Block block, byte aux)
        {
            Block = block;
            Aux = aux;
        }
    }
}
