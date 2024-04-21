using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using StudioForge.TotalMiner.API;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.GameInstance", "AddCentralCharacter")]
    internal static class AddCentralCharacterPatch
    {
        public static void Postfix(object __instance, object c)
        {
            ITMGame game = (ITMGame)__instance;
            ICoreActorManager actorManager = CorePlugin.Instance._game._currentWorld.ActorManager;
            // should never happen unless another mod changes the NPCManager or
            // creates a GameInstance
            if (actorManager.NpcManager != game.World.NpcManager)
            {
                return;
            }
            actorManager.AddActor((ITMActor)c);
        }
    }
}
