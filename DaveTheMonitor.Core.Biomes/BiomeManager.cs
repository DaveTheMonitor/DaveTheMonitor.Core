using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using System;

namespace DaveTheMonitor.Core.Biomes
{
    public sealed class BiomeManager
    {
        private struct BiomeTile
        {
            public ushort Biome { get; set; }
            public ushort Border { get; set; }
            public float BorderBlend { get; set; }

            public BiomeTile(ushort biome)
            {
                Biome = biome;
                Border = ushort.MaxValue;
                BorderBlend = 0;
            }
        }

        public int Seed { get; private set; }
        private BiomeRegistry _register;
        private Biome _ocean;
        private ICoreWorld _world;
        private int[] _tempPerm;
        private int[] _mapPerm;
        private int _seaLevel;
        private BiomeParams _params;
        private BoxInt _bounds;
        private int _size;
        private BiomeTile[,] _biomeMap;

        public void GenerateBiomeMap()
        {
            _biomeMap = new BiomeTile[_size, _size];
            for (int x = _bounds.Min.X; x < _bounds.Max.X; x++)
            {
                for (int z = _bounds.Min.Z; z < _bounds.Max.Z; z++)
                {
                    Biome biome = GenerateBiome(x, z);
                    int xi = x - _bounds.Min.X;
                    int zi = z - _bounds.Min.Z;
                    _biomeMap[zi, xi] = new BiomeTile((ushort)biome.NumId);
                }
            }

            //BlendBiomes();
        }

        private void BlendBiomes()
        {
            int distance = 5;
            int sampleSize = (distance * 2) + 1;
            Span<int> surrounding = stackalloc int[sampleSize * sampleSize];
            for (int x = _bounds.Min.X; x < _bounds.Max.X; x++)
            {
                for (int z = _bounds.Min.Z; z < _bounds.Max.Z; z++)
                {
                    int xi = x - _bounds.Min.X;
                    int zi = z - _bounds.Min.Z;
                    BiomeTile tile = _biomeMap[zi, xi];

                    int i = 0;
                    int max = 0;
                    Biome maxBiome = null;
                    for (int sx = x - distance; sx <= x + distance; sx++)
                    {
                        for (int sz = z - distance; sz <= z + distance; sz++)
                        {
                            Biome biome = GetBiome(sx, sz);
                            if (biome != null)
                            {
                                int count = 0;
                                if (biome.NumId != tile.Biome)
                                {
                                    foreach (int v in surrounding)
                                    {
                                        if (v == biome.NumId)
                                        {
                                            count++;
                                        }
                                    }
                                    if (count > max)
                                    {
                                        max = count;
                                        maxBiome = biome;
                                    }
                                }
                                surrounding[i] = biome.NumId;
                            }
                            else
                            {
                                surrounding[i] = ushort.MaxValue;
                            }
                            i++;
                        }
                    }

                    if (maxBiome != null)
                    {
                        tile.Border = (ushort)maxBiome.NumId;
                        float amount = max / (float)(sampleSize * sampleSize);
                        float ease = (float)(amount < 0.5 ? 4 * amount * amount * amount : 1 - Math.Pow(-2 * amount + 2, 3) / 2);
                        tile.BorderBlend = ease;
                    }
                    else
                    {
                        tile.Border = ushort.MaxValue;
                        tile.BorderBlend = 0;
                    }

                    _biomeMap[zi, xi] = tile;
                }
            }
        }

        public Biome GetBiome(GlobalPoint3D p)
        {
            return GetBiome(p.X, p.Z);
        }

        public Biome GetBiome(Vector3 position)
        {
            return GetBiome((int)MathF.Floor(position.X), (int)MathF.Floor(position.Z));
        }

        public Biome GetBiome(float x, float z)
        {
            return GetBiome((int)MathF.Floor(x), (int)MathF.Floor(z));
        }

        public Biome GetBiome(int x, int z)
        {
            BiomeTile? tile = GetBiomeTile(x, z);
            return tile.HasValue ? _register.GetDefinition(tile.Value.Biome) : null;
        }

        public Biome GetBiome(GlobalPoint3D p, out Biome border, out float borderBlend)
        {
            return GetBiome(p.X, p.Z, out border, out borderBlend);
        }

        public Biome GetBiome(Vector3 position, out Biome border, out float borderBlend)
        {
            return GetBiome((int)MathF.Floor(position.X), (int)MathF.Floor(position.Z), out border, out borderBlend);
        }

        public Biome GetBiome(float x, float z, out Biome border, out float borderBlend)
        {
            return GetBiome((int)MathF.Floor(x), (int)MathF.Floor(z), out border, out borderBlend);
        }

        public Biome GetBiome(int x, int z, out Biome border, out float borderBlend)
        {
            BiomeTile? tile = GetBiomeTile(x, z);
            if (tile.HasValue)
            {
                border = tile.Value.Border != ushort.MaxValue ? _register.GetDefinition(tile.Value.Border) : null;
                borderBlend = tile.Value.BorderBlend;
                return _register.GetDefinition(tile.Value.Biome);
            }
            else
            {
                border = null;
                borderBlend = 0;
                return null;
            }
        }

        public int GetGroundHeight(int x, int z)
        {
            int y = GetBaseGroundHeight(x, z);
            Biome biome = GetBiome(x, z, out Biome border, out float blend);
            if (biome == null)
            {
                return y;
            }

            GlobalPoint3D p = new GlobalPoint3D(x, y, z);
            if (border != null)
            {
                int mainHeight = biome.GetGroundHeight(this, p);
                int borderHeight = border.GetGroundHeight(this, p);
                return (int)MathHelper.Lerp(mainHeight, borderHeight, blend);
            }
            else
            {
                return biome.GetGroundHeight(this, p);
            }
        }

        internal int GetBaseGroundHeight(int x, int z)
        {
            int maxNoise = _params.MaxHeight + _params.MaxSeaDepth;
            maxNoise = (int)(maxNoise * 0.9f);
            float noise = GetBlockNoise(x + _params.OffsetX, z + _params.OffsetZ);
            int seaEffect = _seaLevel - _params.MaxSeaDepth + (maxNoise / 2) - _params.WaterSaturation;
            return (int)(noise * maxNoise + seaEffect);
        }

        private BiomeTile? GetBiomeTile(int x, int z)
        {
            x -= _bounds.Min.X;
            z -= _bounds.Min.Z;
            if (x < 0 || z < 0 || x >= _size || z >= _size)
            {
                return null;
            }
            return _biomeMap[z, x];
        }

        private Biome GenerateBiome(int x, int z)
        {
            float precipitation = GetPrecipitation(x + _params.OffsetX, z + _params.OffsetZ);
            if (precipitation == 1 && _ocean != null)
            {
                return _ocean;
            }

            Biome best = null;
            float bestScore = float.MaxValue;
            float temperature = GetTemperature(x - _bounds.Min.X, z - _bounds.Min.Z);
            foreach (Biome biome in _register)
            {
                if (biome == _ocean)
                {
                    continue;
                }

                float score = GetBiomeScore(biome, temperature, precipitation);
                if (score < bestScore)
                {
                    best = biome;
                    bestScore = score;
                }
            }
            return best;
        }

        private float GetBiomeScore(Biome biome, float temperature, float precipitation)
        {
            float tempScore = Math.Abs(biome.Temperature - temperature) * 1.2f;
            float precScore = Math.Abs(biome.Precipitation - precipitation);
            tempScore = MathF.Pow(tempScore * 100, 1.07f) / 100;
            //precScore = MathF.Pow(precScore, 1.2f);
            return tempScore + precScore;
        }

        internal float GetTemperature(int x, int z)
        {
            int[] perm = _tempPerm;
            float divisor = 256;
            float xin = x / divisor;
            float yin = z / divisor;
            float noise = SimplexNoise1.noise(xin, yin, perm);
            float amount = (float)z / _size;
            float baseT = amount;
            float result = (baseT * 0.8f) + (noise * 0.2f);
            return Math.Clamp(result, 0, 1);
        }

        internal float GetPrecipitation(int x, int z)
        {
            int maxNoise = _params.MaxHeight + _params.MaxSeaDepth;
            maxNoise = (int)(maxNoise * 0.9f);
            float noise = GetBlockNoise(x, z);
            int seaEffect = _seaLevel - _params.MaxSeaDepth + (maxNoise / 2) - _params.WaterSaturation;
            int y = (int)(noise * maxNoise + seaEffect);
            if (y <= _seaLevel)
            {
                return 1;
            }
            else
            {
                int max = _params.MaxHeight / 2;
                return Math.Max(1 - ((y - 200) / (float)max), 0);
            }
            //float result = (noise - seaHeight) / (1 - seaHeight);
            float result = y / 512f;
            return result;
        }

        internal float GetBlockNoise(int x, int z)
        {
            // the output of this method should exactly match BiomeBase.GetBlockNoise
            int[] perm = _mapPerm;
            float bigNoise = _params.BigDetailNoise;
            float mediumNoise = _params.MediumDetailNoise;
            float fineNoise = _params.FineDetailNoise;
            float bigMultiplier = _params.BigDetailMultiplier;
            float mediumMultiplier = _params.MediumDetailMultiplier;
            float fineMultiplier = _params.FineDetailMultiplier;
            float big = SimplexNoise1.noise(x / bigNoise, z / bigNoise, perm);
            float medium = SimplexNoise1.noise(x / mediumNoise, z / mediumNoise, perm);
            float fine = SimplexNoise1.noise(x / fineNoise, z / fineNoise, perm);
            return (big * bigMultiplier + medium * mediumMultiplier + fine * fineMultiplier) / _params.TotalNoiseDivisor;
        }

        public BiomeManager(ICoreWorld world)
        {
            SaveMapHead header = world.Header;
            _world = world;
            _register = _world.Game.BiomeRegistry();
            _ocean = _register.GetDefinition("Core.Ocean");
            Seed = header.MapSeed;
            _seaLevel = world.Map.SeaLevel;
            _bounds = world.Map.MapBound;
            _size = Math.Max(world.Map.MapSize.X, world.Map.MapSize.Z);
            _tempPerm = SimplexNoise1.GetSimplexNoisePermTablePCG((int)(Seed * 0.75f) << 1);
            _mapPerm = SimplexNoise1.GetSimplexNoisePermTable(Seed);
            _seaLevel = world.Map.SeaLevel;
            _params = header.BiomeParams.Clone();
        }
    }
}
