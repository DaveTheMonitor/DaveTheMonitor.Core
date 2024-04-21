using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Graphics.TexturePack", "LoadModTextures")]
    internal static class LoadModTexturesPatch
    {
        public static void Postfix(object __instance, ref Texture2D ___ItemTexture)
        {
            // Game will be null the first time this patch is run, as LoadModTextures
            // is initially called between ITMPlugin.Initialize and ITMPlugin.InitializeGame
            // In this case, textures will be stitched in ITMPlugin.InitializeGame
            if (CorePlugin.Instance.Game == null)
            {
#if DEBUG
                CorePlugin.Log("TextureStitching: Game is null");
#endif
                return;
            }
            ITMTexturePack tp = (ITMTexturePack)__instance;

            Texture2D texture = CorePlugin.Instance._game.StitchModItemTextures(___ItemTexture, tp.ItemTextureSize(), out bool changed);
            if (changed)
            {
                ___ItemTexture.Dispose();
                ___ItemTexture = texture;
            }
        }
    }
}
