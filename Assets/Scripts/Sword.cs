using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Sword : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] int speed = 5;

    private float rotationDuration = 1.0f;
    private Vector3 rotationAxis = new Vector3(0, 0, 360);
    private Rigidbody2D m_rigidbody2D;
    private GameObject attacker;

    private void Awake() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();   
    }

    private void OnEnable() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.DORotate(rotationAxis, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    public void Throw(Vector2 direction, GameObject attacker)
    {
        m_rigidbody2D.velocity = direction.normalized * speed;
        this.attacker = attacker;
        AudioManager.Singleton?.PlaySFX("Attack");
    }

    private void OnDisable() {
        DOTween.Kill(transform);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject != attacker)
        {
            if(other.TryGetComponent(out EnemyController enemy))
            {
                enemy.TakeDamage(damage);
                gameObject.SetActive(false);
            }
            if(!other.GetComponent<ShadowPlayerController>() && (other.transform.parent?.TryGetComponent(out PlayerStats playerStats) ?? false))
            {
                playerStats.Hurt(damage);
                gameObject.SetActive(false);
            }
        }
    }
}
