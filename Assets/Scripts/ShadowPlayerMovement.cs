using UnityEngine;

public class ShadowPlayerMovement : PlayerMovement
{
    public Transform normalPlayer;  // Reference to the normal player
    public Camera mainCamera;       // Reference to the main camera

    protected override void FixedUpdate()
    {
        Vector2 newPosition = rb.position + movement * Time.fixedDeltaTime;

        if (IsNormalPlayerWithinCameraBounds() || IsMovingTowardsNormalPlayer(newPosition))
        {
            // Call the base FixedUpdate to apply movement
            base.FixedUpdate();
        }
        else
        {
            // Only stop horizontal movement, allow vertical velocity (including gravity)
            rb.velocity = new Vector2(0, rb.velocity.y <= 0? rb.velocity.y:0);
        }
    }

    // Check if the normal player is within the camera bounds
    private bool IsNormalPlayerWithinCameraBounds()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(normalPlayer.position);
        float margin = 0.01f; // Smaller allowable area

        return screenPoint.x > margin && screenPoint.x < 1 - margin &&
               screenPoint.y > margin && screenPoint.y < 1 - margin;
    }

    // Check if the shadow player is moving towards the normal player
    private bool IsMovingTowardsNormalPlayer(Vector2 newPosition)
    {
        Vector2 toNormalPlayer = normalPlayer.position - transform.position;
        Vector2 movementDirection = newPosition - rb.position;

        // Check if moving towards the normal player horizontally
        bool isMovingHorizontallyTowards = Mathf.Sign(toNormalPlayer.x) == Mathf.Sign(movementDirection.x);
        
        // Check if moving towards the normal player vertically
        bool isMovingVerticallyTowards = Mathf.Sign(toNormalPlayer.y) == Mathf.Sign(movementDirection.y);

        // Allow movement if moving towards the normal player in either direction
        return isMovingHorizontallyTowards || isMovingVerticallyTowards;
    }
}
