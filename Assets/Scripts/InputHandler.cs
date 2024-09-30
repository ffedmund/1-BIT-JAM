using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 inputMovement { get; private set; }
    public bool jumpInput { get; private set; }
    public PlayerState playerState;
    public event Action<Transform> InteractHandler;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        // Gather input
        inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpInput = Input.GetButtonDown("Jump");
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerState.SwitchState();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            InteractHandler?.Invoke((playerState.currentState == PlayerStates.Shadow? playerState.shadowPlayer: playerState.normalPlayer).transform);
        }
        if(Input.GetKeyDown(KeyCode.F) && playerState.currentState == PlayerStates.Normal && playerState.normalPlayer.TryGetComponent(out SwordAttacker attacker))
        {
            attacker.Attack(new Vector2(inputMovement.x<0?-1:1,inputMovement.y), playerState.normalPlayer);
        }

    }
}
