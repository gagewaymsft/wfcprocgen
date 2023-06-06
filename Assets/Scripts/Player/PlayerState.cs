public class PlayerState
{
    /// <summary>
    /// The current state of the player at any given frame.
    /// </summary>
    /// <param name="maxHealth">Maximum base health the player can have, outside of boosts</param>
    /// <param name="walkSpeed">How fast the player moves when they aren't sprinting</param>
    /// <param name="sprintSpeed">How fast the player moves when sprinting</param>
    /// <param name="baseMeleeAttackDamage">The base mêlée damage the player does before level ups, weapon upgrades, etc.</param>
    /// <param name="baseRangedAttackDamage">The base ranged damage the player does before level ups, weapon upgrades, etc.</param>
    public PlayerState(int maxHealth, float walkSpeed, float sprintSpeed, int baseMeleeAttackDamage, int baseRangedAttackDamage)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        WalkSpeed = walkSpeed;
        SprintSpeed = sprintSpeed;
        BaseMeleeAttackDamage = baseMeleeAttackDamage;
        BaseRangedAttackDamage = baseRangedAttackDamage;
        CurrentMeleeAttackDamage = BaseMeleeAttackDamage;
        CurrentRangeAttackDamage = BaseRangedAttackDamage;
    }

    // currently all public in case I decide any of them can be modified by certain events
    // i.e leveling up for maxhealth, etc
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public float WalkSpeed { get; set; }
    public float SprintSpeed { get; set; }
    public float CurrentMoveSpeed { get; set; }
    public bool IsDead { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsInteracting { get; set; }
    public bool SentDeathMessage { get; set; } = false;
    public int BaseMeleeAttackDamage { get; set; }
    public int BaseRangedAttackDamage { get; set; }
    public int CurrentMeleeAttackDamage { get; set; }
    public int CurrentRangeAttackDamage { get; set; }
}