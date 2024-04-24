using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Helpers
{
    /// <summary>
    /// Contains extensions that allow access to certain methods not normally available through the TM API.
    /// </summary>
    public static class CoreExtensions
    {
        #region Game

        private static AccessTools.FieldRef<object, Vector3> _windVelocity =
            AccessTools.FieldRefAccess<object, Vector3>(AccessTools.Field("StudioForge.TotalMiner.Wind:WindVelocity"));

        private static AccessTools.FieldRef<object, object> _wind =
            AccessTools.FieldRefAccess<object, object>(AccessTools.Field("StudioForge.TotalMiner.GameInstance:Wind"));

        private static AccessTools.FieldRef<object, SaveMapHead> _header =
            AccessTools.FieldRefAccess<object, SaveMapHead>(AccessTools.Field("StudioForge.TotalMiner.GameInstance:Header"));

        #endregion

        #region Actor

        private static AccessTools.FieldRef<object, float> _farClip =
            AccessTools.FieldRefAccess<object, float>(AccessTools.Field("StudioForge.TotalMiner.Actor:farClip"));

        private static AccessTools.FieldRef<object, Matrix> _viewMatrix =
            AccessTools.FieldRefAccess<object, Matrix>(AccessTools.Field("StudioForge.TotalMiner.Actor:ViewMatrix"));

        private static AccessTools.FieldRef<object, Matrix> _viewMatrixLocal =
            AccessTools.FieldRefAccess<object, Matrix>(AccessTools.Field("StudioForge.TotalMiner.Actor:ViewMatrixLocal"));

        #endregion

        #region Player

        private static AccessTools.FieldRef<object, int> _fogVisibility =
            AccessTools.FieldRefAccess<object, int>(AccessTools.Field("StudioForge.TotalMiner.Player:FogVisibility"));
        private static AccessTools.FieldRef<object, float> _fogIntensity =
            AccessTools.FieldRefAccess<object, float>(AccessTools.Field("StudioForge.TotalMiner.Player:FogIntensity"));

        #endregion

        #region ITMActor

        /// <summary>
        /// Gets the <see cref="ICoreActor"/> instance for the specified <see cref="ITMActor"/>.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns>The <see cref="ICoreActor"/> instance for the specified <see cref="ITMActor"/>.</returns>
        public static ICoreActor GetCoreActor(this ITMActor actor) => CoreGame.GetActor(actor);

        public static void SetViewMatrix(this ITMActor actor, Matrix viewMatrix) => _viewMatrix(actor) = viewMatrix;

        public static void SetViewMatrixLocal(this ITMActor actor, Matrix viewMatrixLocal) => _viewMatrixLocal(actor) = viewMatrixLocal;

        #endregion

        #region ITMPlayer

        /// <summary>
        /// Gets the far clip distance for the specified actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <returns>The far clip distance for the specified actor.</returns>
        public static float GetFarClip(this ITMPlayer player) => _farClip(player);

        /// <summary>
        /// Gets the fog visibility for the specified player. At 100% fog intensity, this is the distance before objects are no longer visible.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The fog visibility for the specified player.</returns>
        /// <remarks>
        /// <para>Shaders should use <see cref="IGameShader.FogEnd"/> instead.</para>
        /// </remarks>
        public static int GetFogVisibility(this ITMPlayer player) => _fogVisibility(player);

        /// <summary>
        /// Gets the intensity of the fog for the specified player. 
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The intensity of the fog for the specified player.</returns>
        /// <remarks>
        /// <para>Shaders should use <see cref="IGameShader.FogColor"/> instead.</para>
        /// </remarks>
        public static float GetFogIntensity(this ITMPlayer player) => _fogIntensity(player);

        /// <summary>
        /// Gets the <see cref="ICoreActor"/> instance for the specified <see cref="ITMPlayer"/>.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The <see cref="ICoreActor"/> instance for the specified <see cref="ITMPlayer"/>.</returns>
        public static ICorePlayer GetCorePlayer(this ITMPlayer player) => CoreGame.GetPlayer(player);

        #endregion

        #region ITMGame
        public static Vector3 GetWindVelocity(this ITMGame game) => _windVelocity(_wind(game));
        public static float GetWindFactor(this ITMGame game) => _header(game).WindFactor;

        #endregion

        #region ITMTexturePack

        /// <summary>
        /// Gets the texture for the specified item. For items, this is the item texture atlas, otherwise it's the block texture atlas.
        /// </summary>
        /// <param name="texturePack">The texture pack.</param>
        /// <param name="item">The item.</param>
        /// <returns>The texture for the item.</returns>
        public static Texture2D GetTexture(this ITMTexturePack texturePack, CoreItem item)
        {
            return texturePack.GetTextureForItem(item.ItemType);
        }

        /// <summary>
        /// Gets the rectangle that specifies the item's coordinates in the texture atlas.
        /// </summary>
        /// <param name="texturePack">The texture pack.</param>
        /// <param name="item">The item.</param>
        /// <returns>The item's src rectangle.</returns>
        public static Rectangle GetSrcRect(this ITMTexturePack texturePack, CoreItem item)
        {
            return texturePack.ItemSrcRect(item.ItemType);
        }

        #endregion

        #region Zone

        /// <summary>
        /// Gets the size of the zone in blocks.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>The size of the zone in blocks.</returns>
        public static Vector3 Size(this Zone zone)
        {
            float x = zone.Max.X - zone.Min.X;
            float y = zone.Max.Y - zone.Min.Y;
            float z = zone.Max.Z - zone.Min.Z;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Gets the volume of the zone in blocks.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <returns>The volume of the zone in blocks.</returns>
        public static float Volume(this Zone zone)
        {
            float x = zone.Max.X - zone.Min.X;
            float y = zone.Max.Y - zone.Min.Y;
            float z = zone.Max.Z - zone.Min.Z;
            return x * y * z;
        }

        /// <summary>
        /// Returns true if the specified point is in the zone, otherwise false.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <param name="map">The map containing the zone.</param>
        /// <param name="p">The point.</param>
        /// <returns>True if the specified point is in the zone, otherwise false.</returns>
        public static bool IsInZone(this Zone zone, Map map, GlobalPoint3D p)
        {
            return zone.IsInZone(map, map.GetPosition(p));
        }

        /// <summary>
        /// Returns true if the specified point is in the zone, otherwise false.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <param name="map">The map containing the zone.</param>
        /// <param name="p">The point.</param>
        /// <returns>True if the specified point is in the zone, otherwise false.</returns>
        public static bool IsInZone(this Zone zone, Map map, Vector3 p)
        {
            float tileSize = map.TileSize;
            Vector3 min = zone.Min * tileSize;
            Vector3 max = zone.Max * tileSize;
            BoundingBox box = new BoundingBox(min, max);
            return box.Contains(p) == ContainmentType.Contains;
        }

        #endregion

        private static ICoreGame CoreGame => CorePlugin.Instance.Game;
    }
}
