using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Helpers;
using DaveTheMonitor.Core.Plugin;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Patches
{
    [Patch("StudioForge.TotalMiner.Actor", "Struck",
        "StudioForge.TotalMiner.Actor",
        "StudioForge.BlockWorld.SkillType",
        "StudioForge.TotalMiner.Item",
        "Microsoft.Xna.Framework.Vector3",
        "System.Boolean",
        "System.Single&")]
    internal static class ActorStruckPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);


            MethodInfo takeDamageAndDisplay = AccessTools.Method("StudioForge.TotalMiner.Actor:TakeDamageAndDisplay", new Type[]
            {
                typeof(DamageType),
                typeof(float),
                typeof(Vector3),
                AccessTools.TypeByName("StudioForge.TotalMiner.Actor"),
                typeof(Item),
                typeof(SkillType)
            });

            list.InsertFirst(i => i.Calls(takeDamageAndDisplay),
                -9, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadArgument(3),
                CodeInstruction.LoadArgument(6),
                CodeInstruction.LoadLocal(2, true),
                CodeInstruction.LoadLocal(1, true),
                CodeInstruction.Call(typeof(ActorStruckPatch), nameof(PreAttack)),
            });

            list.InsertFirst(i => i.Calls(takeDamageAndDisplay),
                2, new CodeInstruction[]
            {
                CodeInstruction.LoadArgument(0),
                CodeInstruction.LoadArgument(1),
                CodeInstruction.LoadLocal(1),
                CodeInstruction.LoadArgument(3),
                CodeInstruction.LoadArgument(6),
                new CodeInstruction(OpCodes.Ldind_R4),
                CodeInstruction.LoadLocal(2),
                CodeInstruction.Call(typeof(ActorStruckPatch), nameof(PostAttack))
            });

            return list;
        }

        public static void PreAttack(ITMActor tmActor, ITMActor tmAttacker, Item weaponId, ref float damage, ref Vector3 knockForce, ref DamageType damageType)
        {
            if (!CorePlugin.IsValid || !CorePlugin.Instance.Game.CombatEnabled)
            {
                return;
            }

            ICoreActor actor = tmActor.GetCoreActor();
            ICoreActor attacker = tmAttacker.GetCoreActor();
            CoreItem item = actor.Game.ItemRegistry[weaponId];
            AttackInfo attack = new AttackInfo(damage, damageType, knockForce);

            var enumerator = attacker.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData actorData)
                {
                    AttackInfo? result = actorData.PreAttackTarget(actor, item, attack, attack.Damage >= actor.Health);
                    if (result.HasValue)
                    {
                        attack = result.Value;
                    }
                }
            }

            enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData actorData)
                {
                    AttackInfo? result = actorData.PreAttacked(attacker, item, attack, attack.Damage >= actor.Health);
                    if (result.HasValue)
                    {
                        attack = result.Value;
                    }
                }
            }

            damage = attack.Damage;
            knockForce = attack.KnockForce;
            damageType = attack.DamageType;
        }

        public static void PostAttack(ITMActor tmActor, ITMActor tmAttacker, DamageType damageType, Item weaponId, float damage, Vector3 knockForce)
        {
            if (!CorePlugin.IsValid || !CorePlugin.Instance.Game.CombatEnabled)
            {
                return;
            }

            ICoreActor actor = tmActor.GetCoreActor();
            ICoreActor attacker = tmAttacker.GetCoreActor();
            CoreItem item = actor.Game.ItemRegistry[weaponId];
            AttackInfo attack = new AttackInfo(damage, damageType, knockForce);
            bool fatal = actor.Health <= 0;

            var enumerator = attacker.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData actorData)
                {
                    actorData.PostAttackTarget(actor, item, attack, fatal);
                }
            }

            enumerator = actor.GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ActorData actorData)
                {
                    actorData.PostAttacked(attacker, item, attack, fatal);
                }
            }
        }
    }
}
