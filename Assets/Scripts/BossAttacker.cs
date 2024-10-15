using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BossAttacker : Attacker
{
    // Fist attack settings
    public Transform leftFist;
    public Transform rightFist;
    public float fistAttackSpeed = 5f;  // Speed at which the fists fall down
    public float fistRaiseTime = 0.5f;  // Time to move the fist over the player
    public float fistCooldown = 1.5f;   // Time before the next fist attack

    // Ground slam settings
    public GameObject shockwavePrefab;  // The shockwave prefab
    public float shockwaveRadius = 5f;  // Area of effect for the ground slam shockwave
    public float groundSlamCooldown = 3f; // Time between ground slam attacks
    public float slamPauseDuration = 0.5f; // Time between fists hitting the ground

    public float sweepTime = 2f;        // Speed of the hand's sweep
    public float sweepRange = 10f;       // Distance the hand will sweep (e.g., from left to right)
    public float sweepCooldown = 5f;     // Cooldown between sweep attacks

    private Transform currentFist;  // Tracks which fist is currently attacking
    private bool useLeftFistNext;   // Flag to alternate between fists
    private bool isAttacking = false;
    
    // Fist origional positions for potential reset
    private Vector3 leftFistOriginalPos;
    private Vector3 rightFistOriginalPos;
    
    private Collider2D leftFistCollider2D;
    private Collider2D rightFistCollider2D;

    private GameObject player;  // Reference to the player
    private int hitTimes;

    // Initialization
    public void SetUp()
    {
        leftFistOriginalPos = leftFist.position;
        rightFistOriginalPos = rightFist.position;
        leftFistCollider2D = leftFist.GetComponent<Collider2D>();
        rightFistCollider2D = rightFist.GetComponent<Collider2D>();
        leftFistCollider2D.enabled = false;
        rightFistCollider2D.enabled = false;
        player = FindAnyObjectByType<PlayerState>().normalPlayer;
    }

    // Method to choose the starting fist based on proximity to the player
    private void ChooseStartingFist()
    {
        float distanceToLeftFist = Vector2.Distance(leftFist.position, player.transform.position);
        float distanceToRightFist = Vector2.Distance(rightFist.position, player.transform.position);
        useLeftFistNext = distanceToLeftFist < distanceToRightFist;
    }

    public void DisableDamageCollider()
    {
        leftFistCollider2D.enabled = false;
        rightFistCollider2D.enabled = false;
        isAttacking = true;
    }

    // Overrides Attack to execute the boss-specific attacks
    public override void Attack(Vector2 direction, GameObject player)
    {
        if (!isAttacking)
        {
            ChooseStartingFist();
            StartCoroutine(FistAttackPattern());
        }
    }

    public bool UseSlamAttack()
    {
        if(isAttacking) return false;
        StartCoroutine(GroundSlamAttack());
        return true;
    }

    public void SweeepAttack()
    {
        if (!isAttacking)
        {
            ChooseStartingFist();
            StartCoroutine(Sweep(useLeftFistNext ? leftFist : rightFist));
        }
    }

    // Handles fist attack alternation: decide which fist to raise and then slam
    private IEnumerator FistAttackPattern()
    {
        isAttacking = true;
        hitTimes = 0;

        while (true) // Looping through fist attacks until manually stopped
        {
            // Choose which fist to attack with next based on the `useLeftFistNext` flag
            currentFist = useLeftFistNext ? leftFist : rightFist;

            // Move the fist above the player and then slam down
            yield return StartCoroutine(MoveFistAbovePlayer(currentFist));
            yield return StartCoroutine(SlamFist(currentFist));

            // Switch fist for the next attack
            useLeftFistNext = !useLeftFistNext;

            // Fist attack cooldown before next attack
            yield return new WaitForSeconds(fistCooldown);
            
            // You can break the loop after several iterations or trigger a stop condition
            // Example: Stop after attacking 4 times or checking health/other conditions.
            if(++hitTimes >= 3)
            {
                break;
            }
        }

        isAttacking = false;
    }

        // Sweep attack where the left hand sweeps across the player's Y-position
    private IEnumerator Sweep(Transform fist)
    {
        // Check if already sweeping, prevent multiple overlaps
        if (isAttacking) yield break;
        
        isAttacking = true;

        // Get the player's current Y position
        float playerYPosition = player.transform.position.y;
        float direction = fist == rightFist? 1:-1;

        // Set the hand's Y position to the player's Y position but keep the original Z position and the hand's starting X.
        Vector3 startPosition = new Vector3(fist.position.x - sweepRange / 2 * direction, playerYPosition, fist.position.z);

        float elapsedTime = 0f;
        while (elapsedTime < sweepTime)
        {
            fist.position = Vector2.Lerp(fist.position, startPosition, elapsedTime / sweepTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        (fist == rightFist? rightFistCollider2D:leftFistCollider2D).enabled = true;

        // Now perform sweeping motion: Move hand from its current position across to opposite side
        Vector3 endPosition = new Vector3(fist.position.x + sweepRange * 0.75f * direction, fist.position.y, fist.position.z);

        elapsedTime = 0f;
        while (elapsedTime < sweepTime)
        {
            fist.position = Vector2.Lerp(fist.position, endPosition, elapsedTime / sweepTime);
            elapsedTime += Time.deltaTime;
            if(fist.position.x == endPosition.x)
            {
                (fist == rightFist? rightFistCollider2D:leftFistCollider2D).enabled = false;
            }
            yield return null;
        }
        yield return StartCoroutine(ResetFistPosition(fist));
        isAttacking = false;
    }


    // Move the selected fist directly above the player's current position (anticipating a slam)
    private IEnumerator MoveFistAbovePlayer(Transform fist)
    {
        float elapsedTime = 0f;
        Vector2 targetPosition = new Vector2(player.transform.position.x, fist.position.y + 3f);  // Adjust height as needed

        while (elapsedTime < fistRaiseTime)
        {
            fist.position = Vector2.Lerp(fist.position, targetPosition, elapsedTime / fistRaiseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // Brief pause before slamming down
    }

    // Move the selected fist directly above the player's current position (anticipating a slam)
    private IEnumerator MoveFistUp(Transform fist)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = (fist == leftFist) ? leftFistOriginalPos : rightFistOriginalPos;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y + 3f);  // Adjust height as needed

        while (elapsedTime < fistRaiseTime)
        {
            fist.position = Vector2.Lerp(fist.position, targetPosition, elapsedTime / fistRaiseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // Brief pause before slamming down
    }

    // Let the current fist slam toward the ground (assuming it's over the player)
    private IEnumerator SlamFist(Transform fist, bool shockwave = false)
    {
        Vector3 targetPosition = new Vector3(fist.position.x, player.transform.position.y - 0.5f, fist.position.z);  // Fist slams down to player Y position
        (fist == rightFist? rightFistCollider2D:leftFistCollider2D).enabled = true;

        while ((fist.position - targetPosition).sqrMagnitude > 0.01f)  // Move fist until close to target
        {
            fist.position = Vector2.MoveTowards(fist.position, targetPosition, fistAttackSpeed * Time.deltaTime);
            yield return null;
        }

        AudioManager.Singleton.PlaySFX("Fall");

        // Inflict damage or effects once fist hits the ground
        Debug.Log($"Fist hit player at {fist.position}");
        if(shockwave) InstantiateShockwave(fist);  
        // Reset fist back to its original position after the hit
        yield return StartCoroutine(ResetFistPosition(fist));
    }

    // Reset the current fist back to its original position after an attack
    private IEnumerator ResetFistPosition(Transform fist)
    {
        Vector3 originalPosition = (fist == leftFist) ? leftFistOriginalPos : rightFistOriginalPos;
        (fist == rightFist? rightFistCollider2D:leftFistCollider2D).enabled = false;

        while ((fist.position - originalPosition).sqrMagnitude > 0.01f)
        {
            fist.position = Vector2.MoveTowards(fist.position, originalPosition, fistAttackSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Method for ground slam attack using both fists and releasing a shockwave
    public IEnumerator GroundSlamAttack()
    {
        isAttacking = true;

        // Move both fists above the player's position quickly
        StartCoroutine(MoveFistUp(leftFist));
        StartCoroutine(MoveFistUp(rightFist));

        yield return new WaitForSeconds(1);

        // Slam both fists down sequentially
        StartCoroutine(SlamFist(leftFist,true));
        StartCoroutine(SlamFist(rightFist,true));

        yield return new WaitForSeconds(0.5f);      
        isAttacking = false;
    }

    // Method to instantiate shockwave effect and apply area damage
    private void InstantiateShockwave(Transform fist)
    {
        if (shockwavePrefab != null)
        {
            Vector2 attackDirection = (player.transform.position - fist.position).normalized;
            GameObject projectile = Instantiate(shockwavePrefab, fist.position+new Vector3(0,0.2f,0), Quaternion.identity);
            projectile.GetComponent<ShockWave>().SetUp(attackDirection);

            Debug.Log("Shockwave instantiated with radius: " + shockwaveRadius);
        }
    }
}
