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
    public bool SwitchState()
    {
        if (currentState == PlayerStates.Normal)
        {
            // Raycast from the shadow player's position downwards
            Vector2 rayOrigin = normalPlayer.transform.position + new Vector3(0,3);
            Vector2 rayDirection = Vector2.down; // Cast downwards

            float rayDistance = 5f; // Adjust this value based on how far down you want to check
            int shadowBlockLayer = 1 << 6; // Layer 6 for "shadow block"

            // Perform the raycast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, shadowBlockLayer);

            // Debugging the ray (optional)
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.green, 2f);

            // Check if the ray hit a collider on the shadow block layer
            if (hit.collider != null)
            {
                // Raycast hit something on the shadow block layer, allow switching to shadow state
                currentState = PlayerStates.Shadow;
                currentMovement.enabled = false;
                Rigidbody2D playerRigidbody2D = normalPlayer.GetComponent<Rigidbody2D>();
                playerRigidbody2D.velocity = Vector2.zero;
                playerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                shadowPlayer.transform.position = hit.point + new Vector2(0,0.5f);
                shadowPlayer.SetActive(true);
                currentMovement = shadowPlayer.GetComponent<PlayerMovement>();
                followingCamera.Follow = shadowPlayer.transform;
                return true;
            }
            else
            {
                // Raycast did not hit a shadow block, prevent the state change
                Debug.Log("Shadow player is not standing on a shadow block!");
            }
        }
        else
        {
            // Switching to normal state
            currentState = PlayerStates.Normal;
            shadowPlayer.GetComponent<PlayerController>().ReleaseKey();
            shadowPlayer.SetActive(false);
            currentMovement = normalPlayer.GetComponent<PlayerMovement>();
            currentMovement.enabled = true;
            normalPlayer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            followingCamera.Follow = normalPlayer.transform;
            return true;
        }

        return false;
    }
}
