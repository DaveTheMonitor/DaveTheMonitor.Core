using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;

namespace DaveTheMonitor.Core.API
{
    public interface ICoreMap
    {
        /// <summary>
        /// The game's ITMMap implementation. Don't use this unless you're absolutely sure you need it.
        /// </summary>
        public ITMMap TMMap { get; }
        float TileSize { get; }
        GlobalPoint3D MapSize { get; }
        Point3D ChunkSize { get; }
        Point3D RegionSize { get; }
        BoxInt MapBound { get; }
        ushort SeaLevel { get; }

        BlockDataXML GetBlockDataXml(Block blockID);
        void Commit();
        bool IsValidPoint(GlobalPoint3D p);
        GlobalPoint3D GetPoint(Vector3 pos);
        Vector3 GetPosition(GlobalPoint3D p);
        Vector3 GetBlockCenter(GlobalPoint3D p);
        Block GetBlockID(Vector3 pos);
        Block GetBlockID(GlobalPoint3D p);
        Block GetBlockIDNoCache(GlobalPoint3D p);
        MapBlock GetBlockData(GlobalPoint3D p);
        MapBlock GetBlockIDAndAux(GlobalPoint3D p);
        MapBlock GetBlockIDAndAuxNoCache(GlobalPoint3D p);
        MapBlock GetBlockAndLight(GlobalPoint3D p);
        bool IsBlockDataEqual(Vector3 pos, Block blockID, byte aux);
        void SetBlockData(GlobalPoint3D p, Block blockID, byte auxData, UpdateBlockMethod method, GamerID gamerId, bool transmit);
        void SetBlockData(GlobalPoint3D p, MapBlock oldBlockData, MapBlock newBlockData, UpdateBlockMethod method, GamerID gamerID, bool transmit);
        ClearBlockResult ClearBlock(GlobalPoint3D p, UpdateBlockMethod method, GamerID gamerId, bool transmit);
        byte GetAuxData(GlobalPoint3D p);
        byte GetAuxDataNoCache(GlobalPoint3D p);
        byte GetAuxHighData(GlobalPoint3D p);
        byte GetAuxHighDataNoCache(GlobalPoint3D p);
        ushort GetAuxFullData(GlobalPoint3D p);
        ushort GetAuxFullDataNoCache(GlobalPoint3D p);
        bool HasChanged(byte auxData);
        bool HasChanged(MapBlock blockData);
        bool HasChanged(GlobalPoint3D p);
        void SetAuxData(GlobalPoint3D p, byte auxData, UpdateBlockMethod method, GamerID gamerId, bool transmit);
        void SetAuxData(GlobalPoint3D p, byte oldAuxData, byte auxData, UpdateBlockMethod method, GamerID gamerID, bool transmit);
        MapLight GetLight(GlobalPoint3D p);
        MapLight GetLightNoCache(GlobalPoint3D p);
        byte GetSunLight(GlobalPoint3D p);
        byte GetBlockLight(GlobalPoint3D p);
        MapLight GetMaxNeighbourLight(GlobalPoint3D p);
        MapLight GetMaxNeighbourLight(GlobalPoint3D p, GlobalPoint3D op);
        byte GetMaxNeighbourSunLight(GlobalPoint3D p, GlobalPoint3D op);
        byte GetMaxNeighbourBlockLight(GlobalPoint3D p, GlobalPoint3D op);
        bool CanBlockSeeTheSky(GlobalPoint3D p);
        float GetLightNormalized(byte light);
        float GetLightNormalized(GlobalPoint3D p);
        float GetLightNormalized(MapBlock data);
        float GetLightNormalized(MapLight data);
        float GetSunLightNormalized(GlobalPoint3D p);
        float GetBlockLightNormalized(GlobalPoint3D p);
        Vector2 GetSunAndBlockLightNormalized(GlobalPoint3D p);
        ITMInventory GetBlockInventory(GlobalPoint3D p, GamerID gamerId, bool createIfNotExist);
        DataBlock GetOrAddDataBlock(GlobalPoint3D p);
    }
}
