
using UnityEngine;

public class Attacker : MonoBehaviour {
    public virtual void Attack(Vector2 direction, GameObject player) => Debug.Log("Attack");
}