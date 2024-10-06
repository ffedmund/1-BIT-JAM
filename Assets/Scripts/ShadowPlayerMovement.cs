using UnityEngine;

public class ShadowPlayerMovement : PlayerMovement
{
    public Transform normalPlayer;  // Reference to the normal player
    public Camera mainCamera;       // Reference to the main camera
    public float raycastDistance = 20;
    public LayerMask shadowLayer;

    private PlayerState playerState;
    private ShadowPlayerController shadowPlayerController;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        playerState = transform.parent.GetComponent<PlayerState>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputHandler = transform.parent.GetComponent<InputHandler>();
        shadowPlayerController = GetComponent<ShadowPlayerController>();
    }

    private void OnEnable() {
        animator.CrossFade("shadow_spawn",0);
    }

    private void OnDisable() {
        if(inputHandler != null)
            inputHandler.inputLock = false;
    }

    protected override void Update()
    {
        base.Update();
        // Perform a raycast downwards to check for a shadow layer collider
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, shadowLayer);

        if (hit.collider == null)
        {
            // There is no shadow layer collider beneath the player, perform your desired action here
            playerState.SwitchState();
        }   

        GameObject attachedEnemy = shadowPlayerController.GetAttachedEnemy();
        if(attachedEnemy != null)
        {
            EnemyController enemyController = attachedEnemy.GetComponent<EnemyController>();
            if(enemyController.movementDirection == EnemyController.MovementDirection.Vertical)
            {
                rb.velocity = new Vector2(0, inputHandler.inputMovement.y != 0?  inputHandler.inputMovement.y:rb.velocity.y);
            }
        }
    }

    private void FixedUpdate() {
        ShadowMovementHandler();
    }

    private void ShadowMovementHandler()
    {
        Debug.Log(movement);
        Vector2 newPosition = rb.position + movement * Time.fixedDeltaTime;
        bool canMove = IsNormalPlayerWithinCameraBounds() || IsMovingTowardsNormalPlayer(newPosition);
        rb.velocity = canMove? movement:new Vector2(0,rb.velocity.y);
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

        // Allow movement if moving towards the normal player in either direction
        return isMovingHorizontallyTowards;
    }
}
