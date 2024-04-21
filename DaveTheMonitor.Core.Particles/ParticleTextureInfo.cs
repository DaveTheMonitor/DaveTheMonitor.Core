namespace DaveTheMonitor.Core.Particles
{
    public struct ParticleTextureInfo
    {
        public int Size;
        public int X;
        public int Y;
        public int Depth;

        public ParticleTextureInfo(int size, int x, int y, int depth)
        {
            Size = size;
            X = x;
            Y = y;
            Depth = depth;
        }
    }
}
