using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform keyHolder;  // The position where the key will follow the player.
    public float floatAmplitude = 0.5f;  // Amplitude of the floating effect.
    public float floatFrequency = 1f;    // Frequency of the floating effect.
    public InputHandler inputHandler;

    private GameObject collectedKey;
    private Vector3 keyInitialLocalPosition;
    private float floatTimer;
    private bool isShadowPlayer;

    protected virtual void Start()
    {
        inputHandler = transform.parent.GetComponent<InputHandler>();

        // If this player is a ShadowPlayerController, enabling specific shadow behavior.
        isShadowPlayer = GetType() == typeof(ShadowPlayerController);
    }

    private void Update()
    {
        if (isShadowPlayer)
        {
            HandleShadowSpecificBehavior();
        }

        HandleKeyFloatingEffect();
        HandleCollision();
    }

    private void HandleShadowSpecificBehavior()
    {
        Vector2 targetPos = (Vector2)transform.position + new Vector2(inputHandler.inputMovement.x, 0) * 0.25f;
        Collider2D hit = Physics2D.OverlapPoint(targetPos);
        if (hit?.TryGetComponent(out PushableBlock pushableBlock) ?? false)
        {
            pushableBlock.TryMove(new Vector2(inputHandler.inputMovement.x, 0));
        }
    }

    private void HandleKeyFloatingEffect()
    {
        if (collectedKey != null)
        {
            // Update the floating effect.
            floatTimer += Time.deltaTime * floatFrequency;
            float newY = Mathf.Sin(floatTimer) * floatAmplitude;
            collectedKey.transform.localPosition = keyInitialLocalPosition + new Vector3(0, newY, 0);
        }
    }

    private void HandleCollision()
    {
        // Logic to handle player collision with pushable blocks.
        if (collectedKey == null) return;  // Early exit if no key is collected.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Key collection.
        if (other.CompareTag("Key") && !other.transform.root.CompareTag("Player"))
        {
            collectedKey = other.gameObject;
            collectedKey.transform.SetParent(transform);
            keyInitialLocalPosition = keyHolder.localPosition;
            collectedKey.transform.localPosition = keyInitialLocalPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Pushable") && other.gameObject.TryGetComponent(out PushableBlock pushableBlock))
        {
            Vector2 targetPos = (Vector2)transform.position + new Vector2(inputHandler.inputMovement.x, 0);
            Collider2D hit = Physics2D.OverlapPoint(targetPos);
            if (hit) pushableBlock.TryMove(new Vector2(inputHandler.inputMovement.x, 0));
        }
    }

    // Methods to handle keys
    public bool HasKey() => collectedKey != null;

    public void ReleaseKey(){
        if(collectedKey != null)
        {
            collectedKey.transform.SetParent(null);
            collectedKey = null;
        }
    }

    public void UseKey() {
        if (collectedKey != null)
        {
            AudioManager.Singleton?.PlaySFX("Open");
            Destroy(collectedKey);
        }
    }
}
