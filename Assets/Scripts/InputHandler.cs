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
        if(InteractHandler != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                InteractHandler.Invoke((playerState.currentState == PlayerStates.Shadow? playerState.shadowPlayer: playerState.normalPlayer).transform);
            }
        }
    }
}
