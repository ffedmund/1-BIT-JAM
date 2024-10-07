using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MovementDirection { Horizontal, Vertical }
    public MovementDirection movementDirection = MovementDirection.Horizontal; // Determines if enemy moves left/right or up/down
    public float moveSpeed = 2f;  // Speed at which the enemy moves
    public Vector2 extraSpeedMultiplier = Vector2.one;  // Speed Multiplier base on direction
    public Transform leftRaycastPoint;  // Raycast point on the left side for ground check (horizontal)
    public Transform rightRaycastPoint; // Raycast point on the right side for ground check (horizontal)
    public Transform topRaycastPoint;  // Raycast point on the top side for ground check (vertical)
    public Transform bottomRaycastPoint; // Raycast point on the bottom side for ground check (vertical)
    public LayerMask groundLayer;       // The ground layer to detect
    public float raycastDistance = 0.5f; // Distance for the raycasts
    public float movementBound;  // Movement bounds (either horizontal or vertical)
    public int hp = 3;       // Enemy health points (hp)
    public float detectionRadius = 5f; // Radius for detecting the player
    public float attackRadius = 10f; // Radius for detecting the player
    public float attackCooldown = 1;
    public bool enabledAllTraceMovement;

    private bool movingRightOrUp = true; // True if moving right or up, false for left or down
    private Vector3 startingPosition;
    private Collider2D m_collider2D;
    private Attacker attacker;
    private GameObject player;  // Reference to the player object
    private bool isTracingPlayer = false; // Indicates whether the enemy is in trace mode
    private float attackTimer = 0;

    void Awake()
    {
        m_collider2D = GetComponent<Collider2D>();
    }

    void Start()
    {
        startingPosition = transform.position;
        attacker = GetComponent<Attacker>();
        player = FindAnyObjectByType<PlayerState>().normalPlayer;  // Find the player using its tag
    }

    void OnEnable()
    {
        RaycastToGround();
    }

    void Update()
    {
        if (isTracingPlayer)
        {
            TracePlayer();
        }
        else
        {
            Move();
            CheckForGround();
            CheckBounds();
            DetectPlayer();
            Attack();
        }
    }

    private void Attack()
    {
        if(attacker!=null)
        {
            attackTimer += Time.deltaTime;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if(attackRadius < distanceToPlayer || attackTimer <= attackCooldown) return;
            attackTimer = 0;
            Vector2 direction = (player.transform.position - transform.position).normalized;
            attacker?.Attack(direction + new Vector2(0,0.1f), gameObject);
        }
    }

    // Method to perform a raycast downward and move the enemy to the ground
    void RaycastToGround()
    {
        if (movementDirection == MovementDirection.Vertical)
            return;
        // Cast a ray from the center of the enemy, aiming downwards, to detect the ground
        Vector2 rayOrigin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, groundLayer);

        // Debugging, to visualize the ray if needed
        Debug.DrawRay(rayOrigin, Vector2.down * raycastDistance, Color.red, 2f);

        if (hit.collider != null)
        {
            // We have hit ground, now adjust the enemy's position
            float groundY = hit.point.y;

            // Adjust the enemy position so that the bottom of the collider is at the hit point 
            transform.position = new Vector2(transform.position.x, Mathf.Round((groundY + 0.5f * (transform.lossyScale.x > 1? transform.lossyScale.x:1)) * 10f) / 10f);
        }
        else
        {
            // No ground detected, log warning or leave as is
            Debug.LogWarning("No ground found below. Enemy may be placed incorrectly.");
        }
    }

    // Move the enemy in the specified direction
    void Move()
    {
        if (movementDirection == MovementDirection.Horizontal)
        {
            if (movingRightOrUp)
            {
                transform.Translate(Vector2.right * moveSpeed * extraSpeedMultiplier.y * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * extraSpeedMultiplier.x * Time.deltaTime);
            }
        }
        else // Vertical movement
        {
            if (movingRightOrUp)
            {
                transform.Translate(Vector2.up * moveSpeed * extraSpeedMultiplier.y * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.down * moveSpeed * extraSpeedMultiplier.x * Time.deltaTime);
            }
        }
    }

    // Perform ground checks to avoid falling off edges during horizontal movement
    bool CheckForGround()
    {
        Vector3 checkingPointN = movementDirection == MovementDirection.Vertical ? bottomRaycastPoint.position : leftRaycastPoint.position;
        Vector3 checkingPointP = movementDirection == MovementDirection.Vertical ? topRaycastPoint.position : rightRaycastPoint.position;

        // Perform raycasts from the left and right raycast points
        RaycastHit2D leftGroundCheck = Physics2D.Raycast(checkingPointN, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D rightGroundCheck = Physics2D.Raycast(checkingPointP, Vector2.down, raycastDistance, groundLayer);

        if (movementDirection == MovementDirection.Horizontal)
        {
            // If no ground is detected on the left side and enemy is moving left, switch to moving right
            if (!leftGroundCheck && !movingRightOrUp)
            {
                movingRightOrUp = true;
                Flip();
                return false;
            }
            // If no ground is detected on the right side and enemy is moving right, switch to moving left
            else if (!rightGroundCheck && movingRightOrUp)
            {
                movingRightOrUp = false;
                Flip();
                return false;
            }
        }
        else
        {
            if (leftGroundCheck && !movingRightOrUp)
            {
                movingRightOrUp = true;
                Flip();
                return false;
            }
            else if (rightGroundCheck && movingRightOrUp)
            {
                movingRightOrUp = false;
                Flip();
                return false;
            }
        }
        return true;
    }

    // Check if the enemy is within movement bounds and switch directions if needed
    void CheckBounds()
    {
        if (movementDirection == MovementDirection.Horizontal)
        {
            float currentPositionX = transform.position.x;
            float leftLimit = startingPosition.x - movementBound;
            float rightLimit = startingPosition.x + movementBound;

            if (currentPositionX <= leftLimit && !movingRightOrUp)
            {
                movingRightOrUp = true;
                Flip();
            }
            else if (currentPositionX >= rightLimit && movingRightOrUp)
            {
                movingRightOrUp = false;
                Flip();
            }
        }
        else // Vertical bounds check
        {
            float currentPositionY = transform.position.y;
            float bottomLimit = startingPosition.y - movementBound;
            float topLimit = startingPosition.y + movementBound;

            if (currentPositionY <= bottomLimit && !movingRightOrUp)
            {
                movingRightOrUp = true;
            }
            else if (currentPositionY >= topLimit && movingRightOrUp)
            {
                movingRightOrUp = false;
            }
        }
    }

    // Flip the enemy's horizontal movement
    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Detect if the player is within the detection radius
    void DetectPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Check if player is within detection radius
            if (distanceToPlayer <= detectionRadius)
            {
                // Perform a raycast from enemy to player to see if there is any obstruction (ground layer)
                Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, groundLayer);

                // Debugging, visualize the raycast
                Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.blue, 0.2f);

                // Check if the raycast hit anything that has the ground layer
                if (hit.collider == null)
                {
                    // No ground layer obstructing, so we can start tracing the player
                    isTracingPlayer = true;
                    Debug.Log("Player detected and clear path to player. Start tracing.");
                }
                else
                {
                    // Ground layer is obstructing the view between enemy and player, don't trace yet
                    Debug.Log("Player detected, but ground obstruction found. Cannot trace.");
                }
            }
            else
            {
                // Player is out of detection radius, stop tracing if was tracing before
                if (isTracingPlayer)
                {
                    Debug.Log("Player moved out of detection radius. Stop tracing.");
                    isTracingPlayer = false;
                }
            }
        }
    }

    // Enemy traces the player
    void TracePlayer()
    {
        if (movementDirection == MovementDirection.Vertical && !enabledAllTraceMovement)
        {
            isTracingPlayer = false;
            return;
        }
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            Vector2 direction = (player.transform.position - transform.position).normalized;
            movingRightOrUp = direction.x > 0;
            if (distanceToPlayer > detectionRadius || !CheckForGround())
            {
                isTracingPlayer = false;
                return;
            }
            // Move towards the player
            direction = new Vector2(direction.x, enabledAllTraceMovement ? direction.y : 0);
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    // Reduce the enemy's HP and destroy the enemy if hp <= 0
    public void TakeDamage(int damage)
    {
        hp -= damage;
        AudioManager.Singleton?.PlaySFX("Hit");
        if (hp <= 0)
        {
            AudioManager.Singleton?.PlaySFX("Dead");
            Destroy(gameObject);  // Destroy the enemy object
        }
    }

    // Optional: Draw raycasts and detection radius in the editor for visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw ground detection rays for horizontal movement
        if (movementDirection == MovementDirection.Horizontal)
        {
            if (leftRaycastPoint != null)
                Gizmos.DrawLine(leftRaycastPoint.position, leftRaycastPoint.position + Vector3.down * raycastDistance);

            if (rightRaycastPoint != null)
                Gizmos.DrawLine(rightRaycastPoint.position, rightRaycastPoint.position + Vector3.down * raycastDistance);
        }

        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw movement bounds
        Gizmos.color = Color.green;
        if (movementDirection == MovementDirection.Horizontal)
        {
            Gizmos.DrawLine(new Vector3(startingPosition.x - movementBound, transform.position.y, transform.position.z),
                           new Vector3(startingPosition.x + movementBound, transform.position.y, transform.position.z));
        }
        else
        {
            Gizmos.DrawLine(new Vector3(transform.position.x, startingPosition.y - movementBound, transform.position.z),
                           new Vector3(transform.position.x, startingPosition.y + movementBound, transform.position.z));
        }
    }
}
