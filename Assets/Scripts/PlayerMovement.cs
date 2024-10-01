using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;          // Speed at which the player moves
    public float jumpForce = 10f;         // Force applied when the player jumps
    public Transform groundCheck;         // A position marking where to check if the player is grounded
    public LayerMask groundLayer;   // Tag to identify ground objects

    protected Rigidbody2D rb;
    protected Vector2 movement;
    protected InputHandler inputHandler;
    protected bool isGrounded;
    private float groundCheckRadius = 0.2f; // Radius of the ground check

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = transform.parent.GetComponent<InputHandler>();
    }

    protected virtual void Update()
    {
        // Get horizontal input from the player
        movement.x = inputHandler.inputMovement.x;
        
        // Move the player horizontally
        movement = new Vector2(movement.x * moveSpeed, rb.velocity.y);

        // Check if the player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = false;
        if(colliders.Length > 0)
        {
            isGrounded = true;
        }

        // Check if the player pressed the jump button and is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Apply the velocity to the Rigidbody2D
        rb.velocity = movement;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public Vector2 GetMovement() => movement;
}
