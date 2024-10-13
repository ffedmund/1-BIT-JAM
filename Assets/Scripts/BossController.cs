using System;
using System.Collections;
using UnityEngine;

public class BossController : EnemyController
{
    // Attack Settings
    public enum BossAttackType { Melee, Ranged, Special }  // Different types of boss attacks
    private BossAttackType currentAttack = BossAttackType.Melee;  // Default attack type
    public event Action<int> OnHurt;

    // Melee Attack Config
    [Header("Melee Attack Settings")]
    public float meleeRange = 2f;        // Range for melee attack
    public float meleeCooldown = 1.5f;

    // Ranged Attack Config
    [Header("Ranged Attack Settings")]
    public float rangedRange = 15f;      // Range for ranged attack
    public float rangedCooldown = 2f;
    public GameObject rangedProjectilePrefab;
    public Transform firePoint;          // Fire point for ranged projectile

    // Special Attack
    [Header("Special Attack Settings")]
    public float specialAttackCooldown = 5f;

    public bool spawned;
    private bool canUseSpecialAttack = false;
    
    // Cooldown for general attacks
    private float attackCooldownTimer = 0f;
    private int maxHp;

    protected override void Start()
    {
        base.Start();  // Call parent Start for setup

        // Initialize boss-specific configuration if needed
        maxHp = hp;
        StartCoroutine(SpecialAttackEnableChecker());
        m_collider2D.enabled = false;
    }

    protected override void Update()
    {
        if(!spawned)return;

        // Handle attack-type decision and perform the chosen attack
        if (isTracingPlayer)
        {
            DecideAttackType();
            Attack();
        }
        else
        {
            DetectPlayer();
        }
    }

    // Override the Attack method to include multiple attacks (melee, ranged, special)
    protected override void Attack()
    {
        attackCooldownTimer -= Time.deltaTime;  // Count down the attack cooldown timer
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        // Check if we're currently off cooldown and ready to attack
        if (attackCooldownTimer <= 0)
        {
            switch (currentAttack)
            {
                case BossAttackType.Melee:
                    if (distanceToPlayer <= meleeRange)  // Ensure player is within melee range
                    {
                        PerformMeleeAttack();
                        attackCooldownTimer = meleeCooldown;  // Reset melee cooldown
                    }
                    break;

                case BossAttackType.Ranged:
                    if (distanceToPlayer > meleeRange && distanceToPlayer <= rangedRange)  // Ensure player is within ranged range but outside melee range
                    {
                        PerformRangedAttack();
                        attackCooldownTimer = rangedCooldown;  // Reset ranged attack cooldown
                    }
                    break;

                case BossAttackType.Special:
                    if (canUseSpecialAttack)
                    {
                        PerformSpecialAttack();
                        attackCooldownTimer = meleeCooldown*2;
                    }
                    break;

                default:
                    Debug.LogWarning("Invalid attack type selected.");
                    break;
            }
        }
    }

    // Method to dynamically decide the boss's attack type based on the player's distance and boss's state
    private void DecideAttackType()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (canUseSpecialAttack)  // Prioritize special attack if it's available
        {
            currentAttack = BossAttackType.Special;
        }
        else if (distanceToPlayer <= meleeRange)  // Player is close enough for melee
        {
            currentAttack = BossAttackType.Melee;
        }
        else if (distanceToPlayer > meleeRange && distanceToPlayer <= rangedRange)  // Player is in ranged attack distance
        {
            currentAttack = BossAttackType.Ranged;
        }
    }

    // Flexible Configuration Method to adjust attack parameters dynamically (if needed)
    public void ConfigureAttack(BossAttackType attackType, float damage, float cooldown, float range)
    {
        switch (attackType)
        {
            case BossAttackType.Melee:
                meleeCooldown = cooldown;
                meleeRange = range;
                break;

            case BossAttackType.Ranged:
                rangedCooldown = cooldown;
                rangedRange = range;
                break;

            case BossAttackType.Special:
                specialAttackCooldown = cooldown;  // Special attack doesn't need damage and range (can have its own unique effects)
                break;

            default:
                Debug.LogWarning("Invalid attack type for configuration.");
                break;
        }
    }

    #region Attack Methods

    // Melee attack method
    private void PerformMeleeAttack()
    {
        Vector2 attackDirection = (player.transform.position - transform.position).normalized;

        // Execute melee attack (assumed to deal damage via mechanics not shown here)
        attacker?.Attack(attackDirection, gameObject);  // Assuming the base class `attacker` trigger is sufficient
    }

    // Ranged attack method
    private void PerformRangedAttack()
    {
        ((BossAttacker)attacker).SweeepAttack();
        if (rangedProjectilePrefab != null && firePoint != null)
        {
            Vector2 attackDirection = (player.transform.position - firePoint.position).normalized;
            GameObject projectile = Instantiate(rangedProjectilePrefab, firePoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = attackDirection * 10f;

            // Set projectile damage or other behavior
        }
    }

    // Special attack method
    private void PerformSpecialAttack()
    {
        if(((BossAttacker)attacker).UseSlamAttack())
        {
            StartCoroutine(SpecialAttackCooldown());
        }

    }

    #endregion

    #region Special Attack Handling

    // Coroutine to manage special attack cooldown duration
    private IEnumerator SpecialAttackCooldown()
    {
        canUseSpecialAttack = false;
        yield return new WaitForSeconds(specialAttackCooldown);  // Wait for the full cooldown period
        canUseSpecialAttack = true;
    }

    private IEnumerator SpecialAttackEnableChecker()
    {
        while(!canUseSpecialAttack)
        {
            if(hp <= maxHp/2)
            {
                canUseSpecialAttack = true;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    // Optionally override the TakeDamage method to respond to damage events
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);  // Call base damage processing
        OnHurt?.Invoke(hp);
        if (hp <= 1)
        {
            Debug.Log("Boss Enraged! Increasing attack power or speeding up behavior!");
        }
    }

    public void SpawnBoss()
    {
        ((BossAttacker)attacker).SetUp();
        GetComponent<BossHeadMovement>().StartHeadMovement();
        spawned = true;
        m_collider2D.enabled = true;
    }
}
