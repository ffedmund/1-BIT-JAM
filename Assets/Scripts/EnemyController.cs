using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;  // Speed at which the enemy moves
    public Transform leftRaycastPoint;  // Raycast point on the left side
    public Transform rightRaycastPoint; // Raycast point on the right side
    public LayerMask groundLayer;       // The ground layer to detect
    public float raycastDistance = 0.5f; // Distance for the raycasts

    public float movementBound;  // Left boundary for enemy movement
    public int hp = 3;       // Enemy health points (hp)

    private bool movingRight = true; // Determines the direction the enemy is moving

    private Vector3 startingPosition;

    void Start()
    {
        // Store the starting position
        startingPosition = transform.position;
    }

    void Update()
    {
        Move();
        CheckForGround();
        CheckBounds();
    }

    void Move()
    {
        // Move the enemy using Transform
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    void CheckForGround()
    {
        // Perform raycasts from the left and right raycast points
        RaycastHit2D leftGroundCheck = Physics2D.Raycast(leftRaycastPoint.position, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D rightGroundCheck = Physics2D.Raycast(rightRaycastPoint.position, Vector2.down, raycastDistance, groundLayer);

        // If no ground is detected on the left side and enemy is moving left, switch to moving right
        if (!leftGroundCheck && !movingRight)
        {
            movingRight = true;
            Flip();
        }
        // If no ground is detected on the right side and enemy is moving right, switch to moving left
        else if (!rightGroundCheck && movingRight)
        {
            movingRight = false;
            Flip();
        }
    }

    void CheckBounds()
    {
        // Get the current position of the enemy
        float currentPositionX = transform.position.x;

        // Calculate absolute positions using the starting position and the bounds
        float leftLimit = startingPosition.x - movementBound;
        float rightLimit = startingPosition.x + movementBound;

        // If the enemy goes beyond the left or right bounds, change direction
        if (currentPositionX <= leftLimit && !movingRight)
        {
            movingRight = true;
            Flip();
        }
        else if (currentPositionX >= rightLimit && movingRight)
        {
            movingRight = false;
            Flip();
        }
    }

    // Flip the enemy sprite to face the correct direction
    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;  // Flip the enemy horizontally by inverting the x scale
        transform.localScale = localScale;
    }

    // Function to reduce the enemy's HP and destroy the enemy if hp <= 0
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);  // Destroy the enemy object
        }
    }

    // Optional: Draw raycasts in the editor for visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw left raycast
        if (leftRaycastPoint != null)
            Gizmos.DrawLine(leftRaycastPoint.position, leftRaycastPoint.position + Vector3.down * raycastDistance);

        // Draw right raycast
        if (rightRaycastPoint != null)
            Gizmos.DrawLine(rightRaycastPoint.position, rightRaycastPoint.position + Vector3.down * raycastDistance);

        // Draw movement bounds
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(startingPosition.x - movementBound, transform.position.y, transform.position.z),
                       new Vector3(startingPosition.x + movementBound, transform.position.y, transform.position.z));
    }


    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player") && other.transform.parent.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.Hurt();
        }
    }
}
