using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class EnemyState
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public float MoveSpeed { get; set; }
        public float CurrentSpeed { get; set; }

        public int BaseDamage { get; set; }
        public int CurrentDamage { get; set; }

        public bool IsDead { get; set; }
        public bool SentDeathMessage { get; set; } = false;
        public bool IsMoving { get; set; }
        public bool IsChasingTarget { get; set; }
        public float ChaseRadius { get; set; }
        public bool IsAttacking { get; set; }
        public bool AttackCoolingDown { get; set; }
        public float AttackRadius { get; set; }

        /// <summary>
        /// Likely the player, but could be another target
        /// </summary>
        public GameObject Target { get; set; }
    }
}
