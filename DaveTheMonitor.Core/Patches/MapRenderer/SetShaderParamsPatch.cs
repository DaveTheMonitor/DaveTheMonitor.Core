using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Patches.MapRenderer
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "SetShaderParams")]
    public static class SetShaderParamsPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(SetShaderParamsPatch);

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);
            // The first time SetValue is called, all of the locals (except sky/tint color)
            // have been set, so that's when we apply modifiers.
            // The first call is GraphicStatics.GlobalShader.LanturnColor.SetValue(Vector3)
            int index = list.FindIndex(i => i.Calls(AccessTools.Method(typeof(EffectParameter), nameof(EffectParameter.SetValue), new Type[] { typeof(Vector3) })));
            if (index == -1)
            {
                return list;
            }

            index -= 2;
            list.InsertRange(index, new CodeInstruction[]
            {
                // Fog color
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(2),
                CodeInstruction.LoadLocal(10),
                CodeInstruction.Call(type, nameof(FogColor)),
                CodeInstruction.StoreLocal(10),
                // Fog start
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(2),
                CodeInstruction.LoadLocal(12),
                CodeInstruction.Call(type, nameof(FogStart)),
                CodeInstruction.StoreLocal(12),
                // Fog end
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(2),
                CodeInstruction.LoadLocal(13),
                CodeInstruction.Call(type, nameof(FogEnd)),
                CodeInstruction.StoreLocal(13),
                // Lanturn range
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(2),
                CodeInstruction.LoadLocal(15),
                CodeInstruction.Call(type, nameof(LanturnRange)),
                CodeInstruction.StoreLocal(15),
                // Lanturn color
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(2),
                CodeInstruction.LoadLocal(16),
                CodeInstruction.Call(type, nameof(LanturnColor)),
                CodeInstruction.StoreLocal(16),
            });

            return list;
        }

        public static float FogStart(ITMPlayer player, ITMPlayer virtualPlayer, float v)
        {
            ICoreGame game = CorePlugin.Instance.Game;
            ICorePlayer p = game.GetPlayer(player);
            game.GameShader.ApplyFogStartModifiers(p, virtualPlayer, ref v);
            game.GameShader.FogStart = v;
            return v;
        }

        public static float FogEnd(ITMPlayer player, ITMPlayer virtualPlayer, float v)
        {
            ICoreGame game = CorePlugin.Instance.Game;
            ICorePlayer p = game.GetPlayer(player);
            game.GameShader.ApplyFogEndModifiers(p, virtualPlayer, ref v);
            game.GameShader.FogEnd = v;
            return v;
        }

        public static Vector4 FogColor(ITMPlayer player, ITMPlayer virtualPlayer, Vector4 v)
        {
            ICoreGame game = CorePlugin.Instance.Game;
            ICorePlayer p = game.GetPlayer(player);
            game.GameShader.ApplyFogColorModifiers(p, virtualPlayer, ref v);
            game.GameShader.FogColor = v;
            return v;
        }

        public static Vector3 LanturnColor(ITMPlayer player, ITMPlayer virtualPlayer, Vector3 v)
        {
            ICoreGame game = CorePlugin.Instance.Game;
            ICorePlayer p = game.GetPlayer(player);
            game.GameShader.ApplyLanturnColorModifiers(p, virtualPlayer, ref v);
            game.GameShader.LanturnColor = v;
            return v;
        }

        public static float LanturnRange(ITMPlayer player, ITMPlayer virtualPlayer, float v)
        {
            ICoreGame game = CorePlugin.Instance.Game;
            ICorePlayer p = game.GetPlayer(player);
            game.GameShader.ApplyLanturnRangeModifiers(p, virtualPlayer, ref v);
            game.GameShader.LanturnRange = v;
            return v;
        }
    }
}
