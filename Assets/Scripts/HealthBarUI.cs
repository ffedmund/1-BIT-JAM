using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public int maxHp;
    public GameObject heartPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        maxHp = player.maxHp;
        player.OnHurt += UpdateHp;
        player.OnSpawn += SetMaxHp;
        SetMaxHp();
    }

    void UpdateHp(int newHp)
    {
        int diff = newHp - transform.childCount;
        for(int i = 0; i < Mathf.Abs(diff); i++)
        {
            if(diff > 0)
            {
                Instantiate(heartPrefab,transform);
            }
            else if(transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
        } 
    }

    public void SetMaxHp()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < maxHp; i++)
        {
            Instantiate(heartPrefab,transform);
        } 
    }
}
