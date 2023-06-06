using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class TestEnemy : EnemyBase
    {
        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }

        public override EnemyState ConfigureEnemy()
        {
            return base.ConfigureEnemy();
        }

        public override void CheckForTarget()
        {
            base.CheckForTarget();
        }

        public override void ChaseTarget()
        {
            base.ChaseTarget();
        }

        public override void CheckIfTargetInAttackRadius()
        {
            base.CheckIfTargetInAttackRadius();
        }

        public override void AttackTarget()
        {
            base.AttackTarget();
        }

        public override void AttackCooldown()
        {
            base.AttackCooldown();
        }

        public override void SetupCircleVisualizers()
        {
            base.SetupCircleVisualizers();
        }

        public override void UpdateAttackCircle()
        {
            base.UpdateAttackCircle();
        }

        public override void UpdateChaseCircle()
        {
            base.UpdateChaseCircle();
        }

        public override void UpdateUnityUI()
        {
            base.UpdateUnityUI();
        }

        public override List<AudioSource> GetEnemyAudioSounds()
        {
            return base.GetEnemyAudioSounds();
        }
    }
}
