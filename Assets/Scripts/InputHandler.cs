using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 inputMovement { get; private set; }
    public bool jumpInput { get; private set; }
    public PlayerState playerState;

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
    }
}
