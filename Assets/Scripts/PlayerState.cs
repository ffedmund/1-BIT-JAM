using Cinemachine;
using UnityEngine;

public enum PlayerStates
{
    Normal,
    Shadow
}

public class PlayerState : MonoBehaviour
{
    public GameObject normalPlayer;      // Reference to the normal state player GameObject
    public GameObject shadowPlayer;      // Reference to the shadow state player GameObject
    public PlayerStates currentState;    // Current state of the player
    public CinemachineVirtualCamera followingCamera;

    private PlayerMovement currentMovement; // Reference to the current active PlayerMovement script

    void Start()
    {
        // Initialize the current state to normal
        currentState = PlayerStates.Normal;

        // Enable the current active player and disable the other
        normalPlayer.SetActive(true);
        shadowPlayer.SetActive(false);

        // Set the current movement script reference
        currentMovement = normalPlayer.GetComponent<PlayerMovement>();
    }

    // Method to switch between normal and shadow state
    public void SwitchState()
    {
        if (currentState == PlayerStates.Normal)
        {
            // Switching to shadow state
            currentState = PlayerStates.Shadow;
            currentMovement.enabled = false;
            normalPlayer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            shadowPlayer.transform.position = normalPlayer.transform.position;
            shadowPlayer.SetActive(true);
            currentMovement = shadowPlayer.GetComponent<PlayerMovement>();
            followingCamera.Follow = shadowPlayer.transform;
        }
        else
        {
            // Switching to normal state
            currentState = PlayerStates.Normal;
            shadowPlayer.SetActive(false);
            currentMovement = normalPlayer.GetComponent<PlayerMovement>();
            currentMovement.enabled = true;
            normalPlayer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            followingCamera.Follow = normalPlayer.transform;
        }
    }
}
