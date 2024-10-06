using System.Collections;
using UnityEngine;

public class SwordAttacker : Attacker 
{
    public GameObject currentSwordPrefab;


    public override void Attack(Vector2 direction, GameObject player)
    {
        Sword sword = ObjectPool.Singleton.GetObject(currentSwordPrefab, transform.position, Quaternion.identity).GetComponent<Sword>();
        sword.Throw(direction,player);
        StartCoroutine(SwordDespawnCoroutine(currentSwordPrefab, sword.gameObject));
    }    

    IEnumerator SwordDespawnCoroutine(GameObject prefab, GameObject sword)
    {
        yield return new WaitForSeconds(2);
        ObjectPool.Singleton.ReturnObject(prefab, sword);
    }
}