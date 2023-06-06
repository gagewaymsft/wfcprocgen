using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The man himself.
/// </summary>
public class Player : MonoBehaviour
{
    //public fields are shown in unity editor,
    //could make private and just programatically set them but
    //this for now makes testing easier
    //will programatically set them if game gets close to shipping
    #region Public Fields
    public float WalkSpeed;
    public float SprintSpeed;
    public float CurrentMoveSpeed;

    public int MaxHealth;
    //Don't set "Current" anything in the editor, it's done programatically
    public int CurrentHealth;

    public int BaseMeleeAttackDamage;
    public int BaseRangedAttackDamage;

    // Same as above
    public int CurrentMeleeAttackDamage;
    public int CurrentRangedAttackDamage;

    #endregion

    #region Private Fields
    public PlayerState PlayerState;

    private float animatorSprintSpeed;
    private float animatorWalkSpeed;
    private float currentAnimatorSpeed;

    private Rigidbody2D playerRigidBody;
    private Vector3 change;
    private Animator animator;
    private SpriteRenderer playerSpriteRenderer;

    private readonly int SOUNDS_CHILD_INDEX = 1;

    private AudioSource footstepSound;
    private readonly int FOOTSTEP_SOUND_CHILD_INDEX = 0;

    private AudioSource blinkSound;
    private readonly int BLINK_SOUND_CHILD_INDEX = 1;

    private Crosshair Crosshair;

    private AudioSource playerAttackSound;
    private readonly int PLAYER_KNIFE_SLICE_SOUND_CHILD_INDEX = 2;

    private AudioSource playerDeathSound;
    private readonly int PLAYER_DEATH_SOUND_CHILD_INDEX = 3;

    private List<AudioSource> playerAudioSources;

    private float AttackCooldownTime;
    private bool AttackCoolingDown;

    private Vector3 mouseClampCircleCenter;
    private Vector3 playerLastPosition;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        PlayerConfiguration();
        CrosshairConfiguration();
    }

    private void Update()
    {
        if (!PlayerState.IsDead)
        {
            playerLastPosition = transform.position;
            CheckPlayerState();
            CheckMouseButtonControls();
            UpdateAnimationAndMovePlayer();
            CheckPlayerUpdates();

            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            // gui to show splash screen?
        }
    }

    #region Configuration

    #region Player Configuration
    private void PlayerConfiguration()
    {
        PlayerState = new PlayerState(MaxHealth, WalkSpeed, SprintSpeed, BaseMeleeAttackDamage, BaseRangedAttackDamage);

        playerRigidBody = GetComponent<Rigidbody2D>();

        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
        animatorWalkSpeed = animator.speed;
        animatorSprintSpeed = animator.speed * 1.2f;
        currentAnimatorSpeed = animatorWalkSpeed;

        PlayerSoundConfiguration();
    }

    private void PlayerSoundConfiguration()
    {
        playerAudioSources = GetPlayerAudioSounds();

    }

    private List<AudioSource> GetPlayerAudioSounds()
    {
        footstepSound = transform.GetChild(SOUNDS_CHILD_INDEX).GetChild(FOOTSTEP_SOUND_CHILD_INDEX).GetComponent<AudioSource>();
        blinkSound = transform.GetChild(SOUNDS_CHILD_INDEX).GetChild(BLINK_SOUND_CHILD_INDEX).GetComponent<AudioSource>();
        playerAttackSound = transform.GetChild(SOUNDS_CHILD_INDEX).GetChild(PLAYER_KNIFE_SLICE_SOUND_CHILD_INDEX).GetComponent<AudioSource>();
        playerDeathSound = transform.GetChild(SOUNDS_CHILD_INDEX).GetChild(PLAYER_DEATH_SOUND_CHILD_INDEX).GetComponent<AudioSource>();

        List<AudioSource> playerSounds = new()
        {
            footstepSound,
            blinkSound,
            playerDeathSound,
            playerAttackSound,
        };
        return playerSounds;
    }

    #endregion

    #region Crosshair Configuration
    private void CrosshairConfiguration()
    {
        Crosshair = transform.Find("Crosshair").GetComponent<Crosshair>();
    }

    #endregion 

    #endregion

    #region Frame by Frame Player Updates

    private bool CheckIfSprintKeyPressed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayerState.IsSprinting = true;
            PlayerState.CurrentMoveSpeed = PlayerState.SprintSpeed;
            animator.speed = animatorSprintSpeed;

            //TODO remove me, for testing
            PlayerState.CurrentMeleeAttackDamage = PlayerState.BaseMeleeAttackDamage + 4;
            //
            return true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            PlayerState.IsSprinting = false;
            PlayerState.CurrentMoveSpeed = PlayerState.WalkSpeed;
            animator.speed = animatorWalkSpeed;

            //TODO remove me, for testing
            PlayerState.CurrentMeleeAttackDamage = PlayerState.BaseMeleeAttackDamage;
            //
            return false;
        }
        return false;
    }

    private void CheckPlayerUpdates()
    {
        AnyPlayerInput();
    }

    private bool AnyPlayerInput()
    {
        var anyInput = IsMouseMoving() || IsMouseClicked() || Input.anyKey;
        return anyInput;
    }

    private bool IsMouseMoving()
    {
        var mouseMoving = Input.GetAxis(PlayerControllerConstants.MouseXAxis) != 0 || Input.GetAxis(PlayerControllerConstants.MouseYAxis) != 0;

        if (mouseMoving)
        {
            UpdatePlayerSprites();
        }
        return mouseMoving;
    }

    private bool IsMouseClicked()
    {
        return Input.GetMouseButtonDown(PlayerControllerConstants.LeftMouseButton)
            || Input.GetMouseButtonUp(PlayerControllerConstants.RightMouseButton);
    }

    private void UpdatePlayerSprites()
    {
        if (Crosshair.mouseCursorPosition.x < transform.position.x)
        {
            playerSpriteRenderer.flipX = true;
        }
        if (Crosshair.mouseCursorPosition.x > transform.position.x)
        {
            playerSpriteRenderer.flipX = false;
        }
    }

    /// SPIKE
    /// The methods above and below this clash when setting the animation flipX.
    /// Maybe set a precedence, mouse flip more important than movement flip?
    /// 

    private void UpdateAnimationAndMovePlayer()
    {
        if (change != Vector3.zero)
        {
            float moveSpeed = GetMoveSpeed();
            MovePlayer(moveSpeed);
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
            animator.SetBool("moving", false);
            CurrentMoveSpeed = 0;
            PlayerState.CurrentMoveSpeed = 0;
        }
    }

    private float GetMoveSpeed()
    {
        bool sprinting = CheckIfSprintKeyPressed();

        if (sprinting)
        {
            PlayerState.CurrentMoveSpeed = PlayerState.SprintSpeed;
        }

        else
        {
            PlayerState.CurrentMoveSpeed = PlayerState.WalkSpeed;
        }

        return PlayerState.CurrentMoveSpeed;
    }

    private void MovePlayer(float moveSpeed)
    {
        playerRigidBody.MovePosition(
             transform.position + change.normalized * moveSpeed * Time.fixedDeltaTime
            );
    }

    #endregion

    #region Actions
    private void PlayerAttack()
    {
        if (AttackCoolingDown == false)
        {
            PlayerState.IsAttacking = true;
            animator.SetBool("attacking", true);

            AttackCoolingDown = true;
            Debug.Log($"Attacking with {PlayerState.CurrentMeleeAttackDamage} damage.");

            Invoke(nameof(ResetAttackingAnimator), 0.25f);
            Invoke(nameof(ResetAttackCooldown), AttackCooldownTime);

        }
        else
        {
            Debug.Log("Cooling down");
        }
    }

    private void ResetAttackingAnimator()
    {
        animator.SetBool("attacking", false);
    }

    // HACK probably a better way to do this
    private void ResetAttackCooldown()
    {
        PlayerState.IsAttacking = false;
        AttackCoolingDown = false;
        Debug.Log("Attack cooled down");
    }

    private void PlayerInteract()
    {
        //TODO remove health decrement, testing
        Debug.Log("Taking 1 damage!");

        PlayerState.IsInteracting = true;
        PlayerState.CurrentHealth--;
    }

    #endregion

    #region Control Checks
    private void CheckMouseButtonControls()
    {
        if (Crosshair.currentMouseControlState.CrosshairState == CrosshairStateEnum.Searchlight)
        {
            DoSearchlightControls();
        }
        if (Crosshair.currentMouseControlState.CrosshairState == CrosshairStateEnum.Fight)
        {
            DoFightControls();
        }
        if (Crosshair.currentMouseControlState.CrosshairState == CrosshairStateEnum.Interact)
        {
            DoInteractControls();
        }
    }

    private void DoSearchlightControls()
    {
        if (Input.GetMouseButton(PlayerControllerConstants.LeftMouseButton))
        {
            Crosshair.DisableSearchlight();
        }
        if (Input.GetMouseButtonUp(PlayerControllerConstants.LeftMouseButton))
        {
            Crosshair.EnableSearchlight();
        }
    }

    private void DoInteractControls()
    {
        if (Input.GetMouseButtonDown(PlayerControllerConstants.LeftMouseButton))
        {
            PlayerInteract();
        }
        if (Input.GetMouseButtonUp(PlayerControllerConstants.LeftMouseButton))
        {
            PlayerState.IsInteracting = false;
        }
    }

    private void DoFightControls()
    {
        if (Input.GetMouseButtonDown(PlayerControllerConstants.LeftMouseButton))
        {
            PlayerAttack();
        }
        if (Input.GetMouseButtonUp(PlayerControllerConstants.LeftMouseButton))
        {
            //do nothing?
        }
    }

    #endregion

    #region Player State Frame By Frame Updates
    private void CheckPlayerState()
    {
        int currentHealth = CheckPlayerCurrentHealth();
        bool isDead = CheckIfPlayerDead();

        PlayerState.CurrentHealth = currentHealth;
        PlayerState.IsDead = isDead;

        UpdatePlayerState(PlayerState);
    }

    private void UpdatePlayerState(PlayerState playerState)
    {
        PlayerState = playerState;


        if (PlayerState.IsDead && !PlayerState.SentDeathMessage)
        {
            PlayerDie();
        }
        UpdateUnityUI(PlayerState);
    }

    private void UpdateUnityUI(PlayerState state)
    {
        WalkSpeed = state.WalkSpeed;
        SprintSpeed = state.SprintSpeed;
        MaxHealth = state.MaxHealth;
        CurrentHealth = state.CurrentHealth;
        BaseMeleeAttackDamage = state.BaseMeleeAttackDamage;
        BaseRangedAttackDamage = state.BaseRangedAttackDamage;
        CurrentMeleeAttackDamage = state.CurrentMeleeAttackDamage;
        CurrentRangedAttackDamage = state.CurrentRangeAttackDamage;
        CurrentMoveSpeed = state.CurrentMoveSpeed;
    }

    private void PlayerDie()
    {
        animator.SetTrigger("Die");
        PlayerState.SentDeathMessage = true;
        Debug.Log("Oh dear, you died.");
        playerDeathSound.Play();
    }

    private int CheckPlayerCurrentHealth()
    {
        return PlayerState.CurrentHealth;
    }

    private bool CheckIfPlayerDead()
    {
        return PlayerState.CurrentHealth <= 0;
    }

    #endregion

    #region Frame by Frame Sound Updates
    private void FootstepSound()
    {
        float min = 0.8f;
        float max = 1.2f;
        footstepSound.pitch = Random.Range(min, max);
        footstepSound.Play();
    }

    private void BlinkSound()
    {
        float min = 0.8f;
        float max = 1.2f;
        blinkSound.pitch = Random.Range(min, max);
        blinkSound.Play();
    }

    private void AttackSwordSound()
    {
        float min = 0.8f;
        float max = 1.2f;
        playerAttackSound.pitch = Random.Range(min, max);
        playerAttackSound.Play();
    }

    #endregion

    #region Code Graveyard
    // Hours lost trying to fix this: 4
    // Somehow, both the last frame and the current frames' position
    // of the player is the same even if the player is moving.
    //
    // It makes no sense, and thus this is broken.
    // This is to reset the state of the idle animation back to the beginning if the player moves the mouse
    // This is because there's a blink animation, with the character signifying "hey, im still here" as a joke
    // This doesn't work without this
    //
    //private void UpdatePlayerAnimationState()
    //{
    //    //var playerIdle = CheckIfPlayerIdle();
    //    //if (!playerIdle)
    //    //{
    //    //    ResetIdleAnimation();
    //    //}
    //}
    //private bool CheckIfPlayerIdle()
    //{
    //    var anyInput = AnyPlayerInput();
    //    var playerStandingStill = transform.position == playerLastPosition;

    //    Debug.Log("PLAYER CURRENT POS" + transform.position);
    //    Debug.Log("PLAYER PREVIOS POS" + playerLastPosition);
    //    Debug.Log("DISTANCE DIFFERENCE" + Vector3.Distance(transform.position, playerLastPosition));
    //    Debug.Log("Player Standing Still: " + playerStandingStill);

    //    if (anyInput)
    //        return false;
    //    if (!playerStandingStill)
    //        return false;

    //    return true;
    //}

    //private void ResetIdleAnimation()
    //{
    //    animator.Play("Idle", -1, 0f);
    //    playerLastPosition = transform.position;
    //}
    //----------------------------------------------------------------------------------------------------

    #endregion
}
