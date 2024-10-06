using System;
using UnityEngine;

public class ShadowClaw : MonoBehaviour
{
    [SerializeField] private int speed = 1;
    [SerializeField] private float grabDistance = 1.5f;  // Added grab distance setting
    [SerializeField] private LayerMask shadowLayer;

    private Rigidbody2D m_rigidbody2D;
    private SpriteRenderer m_spriteRenderer;
    private GameObject caster;
    private Action<GameObject> callback;

    private Vector2 startPosition;  // To store the initial position of the movement
    
    private void Awake() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Initiates the grab action.
    public void Grab(Vector2 direction, GameObject caster, Action<GameObject> callback)
    {
        startPosition = transform.position;
        m_rigidbody2D.velocity = direction.normalized * speed;
        m_spriteRenderer.flipX = direction.x < 0;
        this.caster = caster;
        this.callback = callback;
    }

    private void Update()
    {
        // Calculate the distance traveled from the start position
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        // If the distance exceeds the grabDistance, destroy the game object
        if (distanceTraveled > grabDistance)
        {
            Destroy(gameObject);
        }
    }

    // Trigger detection for handling something when the object collides with another
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the object colliding is not the caster, and the caster has an EnemyController component
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 20, shadowLayer);
        if(hit.collider == null) return;

        if (other.gameObject != caster && other.TryGetComponent(out EnemyController enemy))
        {
            // Assuming you want to run some callback logic here
            callback?.Invoke(enemy.gameObject);

            // Destroy the claw after the callback is invoked (optional)
            Destroy(gameObject);
        }
    }
}
