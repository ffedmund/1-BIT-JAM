using UnityEngine;

public class ShadowPlayerController : PlayerController
{
    [SerializeField] private Material invertColorMaterial;   // Material to apply when attached to an enemy.
    [SerializeField] private Material defaultColorMaterial;  // Material to reset to after detaching.

    // References to colliders for handling size/offset changes
    private BoxCollider2D shadowCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private GameObject attachedEnemy;
    private Collider2D m_collider2D;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;

    override protected void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider2D = GetComponent<Collider2D>();
        // Cache the shadow player's collider and save the original size/offset
        shadowCollider = GetComponent<BoxCollider2D>();
        if (shadowCollider != null)
        {
            originalColliderSize = shadowCollider.size;
            originalColliderOffset = shadowCollider.offset;
        }
    }

    public void AttachToEnemy(GameObject enemy)
    {
        if (attachedEnemy == null)
        {
            // Make shadow player a child of the enemy, position it in the same place
            transform.position = enemy.transform.position;
            // transform.parent = enemy.transform;
            enemy.transform.parent = transform;

            // Disable the enemy's SpriteRenderer to hide original appearance.
            SpriteRenderer enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
            if (enemySpriteRenderer != null)
            {
                enemySpriteRenderer.material = invertColorMaterial;
            }

            m_spriteRenderer.enabled = false;
            m_animator.enabled = false;
            m_collider2D.enabled = false;

            // Copy the collider properties from the enemy to the shadow player
            BoxCollider2D enemyCollider = enemy.GetComponent<BoxCollider2D>();
            if (enemyCollider != null)
            {
                shadowCollider.size = enemyCollider.size;
                shadowCollider.offset = enemyCollider.offset;
            }

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                enemyController.enabled = false;
            }
            // Store the attached enemy reference
            attachedEnemy = enemy;
        }
    }

    public void DetachFromEnemy(GameObject player)
    {
        if (attachedEnemy != null)
        {
            // Detach the shadow player from the enemy
            attachedEnemy.transform.SetParent(null);
            transform.position = player.transform.position;

            SpriteRenderer enemySpriteRenderer = attachedEnemy.GetComponent<SpriteRenderer>();
            if (enemySpriteRenderer != null)
            {
                enemySpriteRenderer.material = defaultColorMaterial;
                enemySpriteRenderer.enabled = true;
            }
            EnemyController enemyController = attachedEnemy.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                enemyController.enabled = true;
            }


            // Re-enable the shadow's own movement and reset necessary components
            m_spriteRenderer.enabled = true;
            m_animator.enabled = true;
            m_collider2D.enabled = true;
            // Restore the original shadow player collider size and offset
            shadowCollider.size = originalColliderSize;
            shadowCollider.offset = originalColliderOffset;

            // Clear the attached enemy reference
            attachedEnemy = null;
        }
    }

    // Exposes the attached enemy to other classes
    public GameObject GetAttachedEnemy()
    {
        return attachedEnemy;
    }
}
