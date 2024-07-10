using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;

namespace DaveTheMonitor.Core
{
    internal sealed class CoreMap : ICoreMap
    {
        public Map BWMap => (Map)TMMap;
        public ITMMap TMMap { get; private set; }
        public float TileSize => TMMap.TileSize;
        public GlobalPoint3D MapSize => TMMap.MapSize;
        public Point3D ChunkSize => BWMap.ChunkSize;
        public Point3D RegionSize => BWMap.RegionSize;
        public BoxInt MapBound => BWMap.MapBound;
        public ushort SeaLevel => BWMap.SeaLevel;
        public int MapHeight => BWMap.MapHeight;

        public BlockDataXML GetBlockDataXml(Block blockID)
        {
            return TMMap.GetBlockData(blockID);
        }

        public void Commit()
        {
            TMMap.Commit();
        }

        public bool IsValidPoint(GlobalPoint3D p)
        {
            return TMMap.IsValidPoint(p);
        }

        public GlobalPoint3D GetPoint(Vector3 pos)
        {
            return TMMap.GetPoint(pos);
        }

        public Vector3 GetPosition(GlobalPoint3D p)
        {
            return TMMap.GetPosition(p);
        }
        public Vector3 GetBlockCenter(GlobalPoint3D p)
        {
            return TMMap.GetBlockCenter(p);
        }

        public Block GetBlockID(Vector3 pos)
        {
            return TMMap.GetBlockID(pos);
        }

        public Block GetBlockID(GlobalPoint3D p)
        {
            return TMMap.GetBlockID(p);
        }

        public Block GetBlockIDNoCache(GlobalPoint3D p)
        {
            return TMMap.GetBlockIDNoCache(p);
        }

        public MapBlock GetBlockData(GlobalPoint3D p)
        {
            return TMMap.GetBlockData(p);
        }

        public MapBlock GetBlockIDAndAux(GlobalPoint3D p)
        {
            return TMMap.GetBlockIDAndAux(p); ;
        }

        public MapBlock GetBlockIDAndAuxNoCache(GlobalPoint3D p)
        {
            return TMMap.GetBlockIDAndAuxNoCache(p);
        }

        public MapBlock GetBlockAndLight(GlobalPoint3D p)
        {
            return TMMap.GetBlockAndLight(p);
        }

        public bool IsBlockDataEqual(Vector3 pos, Block blockID, byte aux)
        {
            return TMMap.IsBlockDataEqual(pos, blockID, aux);
        }

        public void SetBlockData(GlobalPoint3D p, Block blockID, byte auxData, UpdateBlockMethod method, GamerID gamerId, bool transmit)
        {
            TMMap.SetBlockData(p, blockID, auxData, method, gamerId, transmit);
        }

        public void SetBlockData(GlobalPoint3D p, MapBlock oldBlockData, MapBlock newBlockData, UpdateBlockMethod method, GamerID gamerID, bool transmit)
        {
            TMMap.SetBlockData(p, oldBlockData, newBlockData, method, gamerID, transmit);
        }

        public ClearBlockResult ClearBlock(GlobalPoint3D p, UpdateBlockMethod method, GamerID gamerId, bool transmit)
        {
            return TMMap.ClearBlock(p, method, gamerId, transmit);
        }

        public byte GetAuxData(GlobalPoint3D p)
        {
            return TMMap.GetAuxData(p);
        }

        public byte GetAuxDataNoCache(GlobalPoint3D p)
        {
            return TMMap.GetAuxDataNoCache(p);
        }

        public byte GetAuxHighData(GlobalPoint3D p)
        {
            return TMMap.GetAuxHighData(p);
        }

        public byte GetAuxHighDataNoCache(GlobalPoint3D p)
        {
            return TMMap.GetAuxHighDataNoCache(p);
        }

        public ushort GetAuxFullData(GlobalPoint3D p)
        {
            return TMMap.GetAuxFullData(p);
        }

        public ushort GetAuxFullDataNoCache(GlobalPoint3D p)
        {
            return TMMap.GetAuxFullDataNoCache(p);
        }

        public bool HasChanged(byte auxData)
        {
            return TMMap.HasChanged(auxData);
        }

        public bool HasChanged(MapBlock blockData)
        {
            return TMMap.HasChanged(blockData);
        }

        public bool HasChanged(GlobalPoint3D p)
        {
            return TMMap.HasChanged(p);
        }

        public void SetAuxData(GlobalPoint3D p, byte auxData, UpdateBlockMethod method, GamerID gamerId, bool transmit)
        {
            TMMap.SetAuxData(p, auxData, method, gamerId, transmit);
        }

        public void SetAuxData(GlobalPoint3D p, byte oldAuxData, byte auxData, UpdateBlockMethod method, GamerID gamerId, bool transmit)
        {
            TMMap.SetAuxData(p, oldAuxData, auxData, method, gamerId, transmit);
        }

        public MapLight GetLight(GlobalPoint3D p)
        {
            return TMMap.GetLight(p);
        }

        public MapLight GetLightNoCache(GlobalPoint3D p)
        {
            return TMMap.GetLight(p);
        }

        public byte GetSunLight(GlobalPoint3D p)
        {
            return TMMap.GetSunLight(p);
        }

        public byte GetBlockLight(GlobalPoint3D p)
        {
            return TMMap.GetBlockLight(p);
        }

        public MapLight GetMaxNeighbourLight(GlobalPoint3D p)
        {
            return TMMap.GetMaxNeighbourLight(p);
        }

        public MapLight GetMaxNeighbourLight(GlobalPoint3D p, GlobalPoint3D op)
        {
            return TMMap.GetMaxNeighbourLight(p);
        }

        public byte GetMaxNeighbourSunLight(GlobalPoint3D p, GlobalPoint3D op)
        {
            return TMMap.GetMaxNeighbourSunLight(p, op);
        }

        public byte GetMaxNeighbourBlockLight(GlobalPoint3D p, GlobalPoint3D op)
        {
            return TMMap.GetMaxNeighbourBlockLight(p, op);
        }

        public bool CanBlockSeeTheSky(GlobalPoint3D p)
        {
            return TMMap.CanBlockSeeTheSky(p);
        }

        public float GetLightNormalized(byte light)
        {
            return TMMap.GetLightNormalized(light);
        }

        public float GetLightNormalized(GlobalPoint3D p)
        {
            return TMMap.GetLightNormalized(p);
        }

        public float GetLightNormalized(MapBlock data)
        {
            return TMMap.GetLightNormalized(data);
        }

        public float GetLightNormalized(MapLight data)
        {
            return TMMap.GetLightNormalized(data);
        }

        public float GetSunLightNormalized(GlobalPoint3D p)
        {
            return TMMap.GetSunLightNormalized(p);
        }

        public float GetBlockLightNormalized(GlobalPoint3D p)
        {
            return TMMap.GetBlockLightNormalized(p);
        }

        public Vector2 GetSunAndBlockLightNormalized(GlobalPoint3D p)
        {
            return TMMap.GetSunAndBlockLightNormalized(p);
        }

        public ITMInventory GetBlockInventory(GlobalPoint3D p, GamerID gamerId, bool createIfNotExist)
        {
            return TMMap.GetBlockInventory(p, gamerId, createIfNotExist);
        }

        public DataBlock GetOrAddDataBlock(GlobalPoint3D p)
        {
            return TMMap.GetOrAddDataBlock(p);
        }

        public CoreMap(ITMMap map)
        {
            TMMap = map;
        }
    }
}
