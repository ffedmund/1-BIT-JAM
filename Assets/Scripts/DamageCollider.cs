using UnityEngine;
using System.Collections;
using Unity.VisualScripting; // Required for Coroutines

public class DamageCollider : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageCooldown = 1.0f; // Set cooldown period (in seconds)

    private Collider2D damageTrigger; // Reference to the collider.
    private bool canDealDamage = true; // Flag to check if damage can be dealt.

    private void Start()
    {
        damageTrigger = GetComponent<Collider2D>(); // Get the trigger collider.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cooldown check - if the cooldown is active, do nothing
        if (!canDealDamage)
            return;

        if (other.transform.root.gameObject != transform.root.gameObject)
        {
            if (transform.root.CompareTag("Player") && other.TryGetComponent(out EnemyController enemy))
            {
                enemy.TakeDamage(damageAmount);
                StartCoroutine(DisableDamageForCooldown()); // Start cooldown after damage
            }
            else if (other.transform.parent?.TryGetComponent(out PlayerStats player) ?? false)
            {
                player.Hurt();
                StartCoroutine(DisableDamageForCooldown()); // Start cooldown after damage
            }
        }
    }

    // Coroutine to handle the cooldown process
    private IEnumerator DisableDamageForCooldown()
    {
        canDealDamage = false;          // Disable damage dealing.
        damageTrigger.enabled = false;  // Disable the trigger collider during cooldown.

        // Wait for the specified cooldown period
        yield return new WaitForSeconds(damageCooldown);

        // Re-enable after cooldown
        damageTrigger.enabled = true;   
        canDealDamage = true;           // Allow damage dealing again.
    }
}
