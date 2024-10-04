using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour {
    protected virtual void Interact(GameObject player) {}
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Interact(other.gameObject);
        }
    }
}