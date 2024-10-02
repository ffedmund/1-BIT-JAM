using UnityEngine;

public class DamageCollider : MonoBehaviour {
    private Rigidbody2D m_rigidbody2D;
    private float force;

    private void Start() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        force = m_rigidbody2D.velocity.magnitude;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (m_rigidbody2D != null && force > 0)
        {
            if(other.collider.TryGetComponent(out EnemyController enemy))
            {
                enemy.TakeDamage(10);
            }
        }
    }
}