using UnityEngine;

public class DeadZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && other.transform.parent.TryGetComponent(out PlayerStats player))
        {
            player.Dead();
        }
    }
}