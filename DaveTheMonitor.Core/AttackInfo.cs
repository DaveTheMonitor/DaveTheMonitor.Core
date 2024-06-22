using Microsoft.Xna.Framework;
using SharpDX.X3DAudio;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// Damage and knockback for an attack.
    /// </summary>
    [DebuggerDisplay("Damage = {Damage}, Type = {DamageType}")]
    public struct AttackInfo
    {
        /// <summary>
        /// The damage dealt.This may be zero if the attack was blocked (either by armor or a shield).
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// The type of damage dealt.
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        /// The knockback dealt.
        /// </summary>
        public Vector3 KnockForce { get; set; }

        /// <summary>
        /// Creates a new, empty <see cref="AttackInfo"/>.
        /// </summary>
        public AttackInfo()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="AttackInfo"/>.
        /// </summary>
        /// <param name="damage">The damage of the attack.</param>
        /// <param name="damageType">The type of damage.</param>
        /// <param name="knockForce">The knockback of the attack.</param>
        public AttackInfo(float damage, DamageType damageType, Vector3 knockForce)
        {
            Damage = damage;
            KnockForce = knockForce;
            DamageType = damageType;
        }
    }
}
