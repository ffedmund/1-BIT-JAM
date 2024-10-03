using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 inputMovement { get; private set; }
    public bool jumpInput { get; private set; }
    public PlayerState playerState;
    public PlayerStats playerStats;
    public event Action<Transform> InteractHandler;

    // Cooldown variables
    public float switchCooldownDuration = 0.4f; // Cooldown duration in seconds
    private float switchCooldownTimer = 0f;     // Timer to track cooldown
    public float attackCooldownDuration = 1f; // Cooldown duration in seconds
    private float attackCooldownTimer = 0f;     // Timer to track cooldown

    public bool inputLock;

    private GameObject settingUI;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerStats = GetComponent<PlayerStats>();
        inputLock = false;
    }

    void Update()
    {
        if(playerStats.dead || inputLock)
        {
            inputMovement = Vector2.zero;
            return;
        }
        // Decrease the cooldown timer if it's above zero
        if (switchCooldownTimer > 0f)
        {
            switchCooldownTimer -= Time.deltaTime;
        }
        // Decrease the cooldown timer if it's above zero
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        // Gather input
        inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpInput = Input.GetButtonDown("Jump");

        // Check for state switch input with cooldown
        if (GameManager.Singleton.unlockShadowPower && Input.GetKeyDown(KeyCode.Tab) && switchCooldownTimer <= 0f)
        {
            if(playerState.SwitchState())
                switchCooldownTimer = switchCooldownDuration; // Reset the cooldown timer
            else
                FindAnyObjectByType<HUDController>().SetInteractText("No Shadow!",1);
        }

        // Interact input
        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform currentPlayerTransform = (playerState.currentState == PlayerStates.Shadow
                ? playerState.shadowPlayer
                : playerState.normalPlayer).transform;
            InteractHandler?.Invoke(currentPlayerTransform);
        }

        // Attack input
        if (Input.GetKey(KeyCode.F)
            && playerState.currentState == PlayerStates.Normal
            && playerState.normalPlayer.TryGetComponent(out SwordAttacker attacker))
        {
            if(attackCooldownTimer <= 0f)
            {
                float directionX = inputMovement.x < 0 ? -1 : 1;
                attacker.Attack(new Vector2(directionX, inputMovement.y), playerState.normalPlayer);
                attackCooldownTimer = attackCooldownDuration; // Reset the cooldown timer
            }
            inputMovement = Vector2.zero;
        }

        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.O))
        {
            if(settingUI == null)
                settingUI = FindAnyObjectByType<SettingUIController>(FindObjectsInactive.Include).gameObject;
            settingUI.SetActive(!settingUI.activeSelf);
        }
    }
}
