using UnityEngine;

public class HealingItem : Item {
    protected override void Interact(GameObject player)
    {
        PlayerStats playerStats = player.transform.parent.GetComponent<PlayerStats>();
        playerStats.Heal();
        Destroy(gameObject);
    }
}