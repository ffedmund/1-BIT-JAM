using UnityEngine;

public class ShadowAttacker : Attacker
{
    public GameObject clawPrefab;  // A reference to the ShadowClaw prefab.

    // Basic attack method that creates the claw and sends it in a direction.
    public override void Attack(Vector2 direction, GameObject player)
    {
        GameObject claw = Instantiate(clawPrefab, transform.position, Quaternion.identity);
        ShadowClaw shadowClaw = claw.GetComponent<ShadowClaw>();
        
        // Execute claw grab. The shadow player is responsible for attaching once grab completes.
        shadowClaw.Grab(direction, gameObject, enemy =>
        {
            if (player.TryGetComponent(out ShadowPlayerController shadowPlayer))
            {
                shadowPlayer.AttachToEnemy(enemy);
            }
        });
    }
}
