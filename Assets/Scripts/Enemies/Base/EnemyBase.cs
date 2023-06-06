using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    // TODO: create custom constructor to take in params for instantiating enemies
    public class EnemyBase : MonoBehaviour
    {
        public EnemyState State;

        public int maxHealth, currentHealth;

        public int baseDamage, currentDamage;

        public float moveSpeed, currentSpeed;

        public bool isChasingTarget, isAttacking, isMoving, isDead;

        public float chaseRadius, attackRadius;

        public bool displayChaseRadiusCircle = true, displayAttackRadiusCircle = true;

        public bool attackCoolingDown = false;

        public GameObject target;

        [HideInInspector]
        public GameObject chaseCircle, attackCircle;

        [HideInInspector]
        public Vector3 chaseRadiusCircleCenter, attackRadiusCircleCenter;

        [HideInInspector]
        public SpriteRenderer enemySpriteRenderer;

        [HideInInspector]
        public List<AudioSource> EnemyAudioSources;

        [HideInInspector]
        private float previousAttackRadius, previousChaseRadius;


        public virtual void Start()
        {
            State = ConfigureEnemy();
            SetupCircleVisualizers();

            enemySpriteRenderer = GetComponent<SpriteRenderer>();
            EnemyAudioSources = GetEnemyAudioSounds();
        }

        public virtual List<AudioSource> GetEnemyAudioSounds()
        {
            List<AudioSource> enemySounds = new();
            return enemySounds;
        }

        public virtual void SetupCircleVisualizers()
        {
            chaseCircle = new GameObject { name = "Chase Circle" };
            chaseCircle.transform.SetParent(transform);
            chaseCircle.DrawCircle(chaseRadius, 0.02f, Color.magenta);

            attackCircle = new GameObject { name = "Attack Circle" };
            attackCircle.transform.SetParent(transform);
            attackCircle.DrawCircle(attackRadius, 0.02f, Color.red);
        }

        public virtual EnemyState ConfigureEnemy()
        {
            return new EnemyState()
            {
                BaseDamage = baseDamage,
                CurrentDamage = baseDamage,
                IsDead = false,
                IsMoving = false,
                CurrentSpeed = 0,
                IsAttacking = false,
                MaxHealth = maxHealth,
                CurrentHealth = maxHealth,
                MoveSpeed = moveSpeed,
                IsChasingTarget = false,
                SentDeathMessage = false,
                Target = target,
                ChaseRadius = chaseRadius,
                AttackRadius = attackRadius,
                AttackCoolingDown = attackCoolingDown
            };
        }

        public virtual void Update()
        {
            CheckForTarget();
            UpdateChaseCircle();
            UpdateAttackCircle();
            UpdateUnityUI();
        }

        public virtual void CheckForTarget()
        {
            chaseRadiusCircleCenter = transform.position;
            float targetDistanceFromChaseRadiusCenter = Vector3.Distance(State.Target.transform.position, chaseRadiusCircleCenter);

            if (targetDistanceFromChaseRadiusCenter > chaseRadius)
            {
                State.IsChasingTarget = false;
                State.CurrentSpeed = 0;
                State.IsMoving = false;
            }
            else
            {
                ChaseTarget();
            }
        }

        public virtual void ChaseTarget()
        {
            State.IsChasingTarget = true;
            State.CurrentSpeed = State.MoveSpeed;
            State.IsMoving = true;

            enemySpriteRenderer.flipX = ShouldFlipSpriteRenderer();

            if (!State.IsAttacking)
            {
                Vector3 targetPosition = State.Target.transform.position;
                Vector3 selfPosition = transform.position;
                float moveSpeed = State.MoveSpeed;
                float timeDelta = Time.deltaTime;

                transform.position = Vector3
                    .MoveTowards(selfPosition, targetPosition, moveSpeed * timeDelta);
            }

            CheckIfTargetInAttackRadius();
        }

        private bool ShouldFlipSpriteRenderer()
        {
            return State.Target.transform.position.x < transform.position.x;
        }

        public virtual void CheckIfTargetInAttackRadius()
        {
            attackRadiusCircleCenter = transform.position;

            float targetDistanceFromAttackRadiusCenter = Vector3.Distance(State.Target.transform.position, attackRadiusCircleCenter);

            State.IsAttacking = targetDistanceFromAttackRadiusCenter < attackRadius;

            if (!State.IsAttacking) return;

            AttackTarget();
        }

        public virtual void AttackTarget()
        {
            if (!State.AttackCoolingDown)
            {
                Debug.Log($"{transform.name} is attacking {State.Target.transform.name}");

                Invoke(nameof(AttackCooldown), 2);
                State.AttackCoolingDown = true;
            }
        }

        public virtual void AttackCooldown()
        {
            State.AttackCoolingDown = false;
        }

        public virtual void UpdateUnityUI()
        {
            maxHealth = State.MaxHealth;
            currentHealth = State.CurrentHealth;

            baseDamage = State.BaseDamage;
            currentDamage = State.CurrentDamage;

            isDead = State.IsDead;

            moveSpeed = State.MoveSpeed;
            isMoving = State.IsMoving;
            currentSpeed = State.CurrentSpeed;
            isChasingTarget = State.IsChasingTarget;

            isAttacking = State.IsAttacking;
            target = State.Target;
            attackCoolingDown = State.AttackCoolingDown;
        }

        public virtual void UpdateChaseCircle()
        {
            if (chaseRadius != previousChaseRadius)
            {
                chaseCircle.RemoveCircle();
                chaseCircle = new GameObject { name = "Chase Circle" };
                chaseCircle.transform.SetParent(transform);

                chaseCircle.DrawCircle(chaseRadius, 0.02f, Color.magenta);
            }
            chaseCircle.transform.position = transform.position;
            if (displayChaseRadiusCircle == false)
            {
                chaseCircle.SetActive(false);
            }
            else
            {
                chaseCircle.SetActive(true);
            }
            previousChaseRadius = chaseRadius;
            State.ChaseRadius = chaseRadius;
        }

        public virtual void UpdateAttackCircle()
        {
            if (attackRadius != previousAttackRadius)
            {
                attackCircle.RemoveCircle();
                attackCircle = new GameObject { name = "Attack Circle" };
                attackCircle.transform.SetParent(transform);

                attackCircle.DrawCircle(attackRadius, 0.02f, Color.red);
            }

            attackCircle.transform.position = transform.position;
            if (displayAttackRadiusCircle == false)
            {
                attackCircle.SetActive(false);
            }
            else
            {
                attackCircle.SetActive(true);
            }
            previousAttackRadius = attackRadius;
            State.AttackRadius = attackRadius;
        }
    }
}
