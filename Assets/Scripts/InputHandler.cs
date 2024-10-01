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
    public float switchCooldownDuration = 0.5f; // Cooldown duration in seconds
    private float switchCooldownTimer = 0f;     // Timer to track cooldown

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if(playerStats.dead)
        {
            inputMovement = Vector2.zero;
            return;
        }
        // Decrease the cooldown timer if it's above zero
        if (switchCooldownTimer > 0f)
        {
            switchCooldownTimer -= Time.deltaTime;
        }

        // Gather input
        inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpInput = Input.GetButtonDown("Jump");

        // Check for state switch input with cooldown
        if (Input.GetKeyDown(KeyCode.Tab) && switchCooldownTimer <= 0f)
        {
            playerState.SwitchState();
            switchCooldownTimer = switchCooldownDuration; // Reset the cooldown timer
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
        if (Input.GetKeyDown(KeyCode.F)
            && playerState.currentState == PlayerStates.Normal
            && playerState.normalPlayer.TryGetComponent(out SwordAttacker attacker))
        {
            float directionX = inputMovement.x < 0 ? -1 : 1;
            attacker.Attack(new Vector2(directionX, inputMovement.y), playerState.normalPlayer);
        }
    }
}
