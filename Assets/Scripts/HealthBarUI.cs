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
        UpdateHp(player.curHp);
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
            else
            {
                Destroy(transform.GetChild(0).gameObject);
            }
        } 
    }

}
