using UnityEngine;

public class ShadowPlayerController : PlayerController
{
    [SerializeField] private Material invertColorMaterial;   // Material to apply when attached to an enemy.
    [SerializeField] private Material defaultColorMaterial;  // Material to reset to after detaching.

    // References to colliders for handling size/offset changes
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private LayerMask originalLayer;
    private GameObject attachedEnemy;
    private BoxCollider2D m_collider2D;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;

    override protected void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider2D = GetComponent<BoxCollider2D>();
        if (m_collider2D != null)
        {
            originalColliderSize = m_collider2D.size;
            originalColliderOffset = m_collider2D.offset;
        }
        originalLayer = gameObject.layer;
    }

    public void AttachToEnemy(GameObject enemy)
    {
        if (attachedEnemy == null)
        {
            // Make shadow player a child of the enemy, position it in the same place
            gameObject.layer = enemy.layer;
            transform.position = enemy.transform.position;
            enemy.transform.parent = transform;

            // Disable the enemy's SpriteRenderer to hide original appearance.
            SpriteRenderer enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
            if (enemySpriteRenderer != null)
            {
                enemySpriteRenderer.material = invertColorMaterial;
            }

            m_spriteRenderer.enabled = false;
            m_animator.enabled = false;

            // Copy the collider properties from the enemy to the shadow player
            BoxCollider2D enemyCollider = enemy.GetComponent<BoxCollider2D>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
                m_collider2D.size = enemyCollider.size;
                m_collider2D.offset = enemyCollider.offset;
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
            gameObject.layer = originalLayer;

            SpriteRenderer enemySpriteRenderer = attachedEnemy.GetComponent<SpriteRenderer>();
            EnemyController enemyController = attachedEnemy.GetComponent<EnemyController>();
            BoxCollider2D enemyCollider = attachedEnemy.GetComponent<BoxCollider2D>();
            if (enemySpriteRenderer != null)
            {
                enemySpriteRenderer.material = defaultColorMaterial;
                enemySpriteRenderer.enabled = true;
            }
            if(enemyController != null)
            {
                enemyController.enabled = true;
            }
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true;
            }


            // Re-enable the shadow's own movement and reset necessary components
            m_spriteRenderer.enabled = true;
            m_animator.enabled = true;
            // Restore the original shadow player collider size and offset
            m_collider2D.size = originalColliderSize;
            m_collider2D.offset = originalColliderOffset;

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
