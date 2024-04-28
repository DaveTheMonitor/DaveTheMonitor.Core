﻿using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Graphics;
using DaveTheMonitor.Core.Plugin;
using DaveTheMonitor.Core.Wrappers;
using HarmonyLib;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches.Rendering
{
    [Patch("StudioForge.TotalMiner.Renderers.MapRenderer", "DrawSunAndMoon")]
    internal static class DrawSunAndMoonPatch
    {
        private const WorldDrawStage _stage = WorldDrawStage.SunAndMoon;
        private const WorldDrawOptions _option = WorldDrawOptions.SunAndMoon;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            Type type = typeof(MapRendererUpdateCorePatch);

            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            int index = MapRendererUpdateCorePatch.FindSpaceWorldTest(list, out Label? label);
            if (index == -1)
            {
                return list;
            }

            index -= 2;
            list.RemoveRange(index, 5);
            list.InsertRange(index, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.Call(type, nameof(MapRendererUpdateCorePatch.IsSpaceWorld)),
                new CodeInstruction(OpCodes.Brtrue, label.Value)
            });

            return list;
        }

        public static bool Prefix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return true;
            }

            if ((DrawGlobals.WorldDrawOptions & _option) > 0)
            {
                ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
                CorePlugin.Instance.Game.RunPreDrawWorldMap(_stage, new MapRenderer((DrawableGameObjectBase)__instance).Map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
                return true;
            }

            return false;
        }

        public static void Postfix(object __instance, ITMPlayer player, ITMPlayer virtualPlayer)
        {
            if (CorePlugin.Instance == null)
            {
                return;
            }

            if ((DrawGlobals.WorldDrawOptions & _option) > 0)
            {
                ICorePlayer corePlayer = CorePlugin.Instance.Game.GetPlayer(player);
                CorePlugin.Instance.Game.RunPostDrawWorldMap(_stage, new MapRenderer((DrawableGameObjectBase)__instance).Map, corePlayer, virtualPlayer, DrawGlobals.WorldDrawOptions);
            }
        }
    }
}
