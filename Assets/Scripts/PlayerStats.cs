using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public event Action<int> OnHurt;
    public event Action OnSpawn;
    public bool dead{get; private set;}
    public int maxHp = 3;
    public int curHp = 0;

    void Awake()
    {
        curHp = maxHp;
    }

    public void Hurt()
    {
        if(!dead)
        {
            curHp--;
            OnHurt?.Invoke(curHp);
            AudioManager.Singleton?.PlaySFX("Hurt");
            if(curHp <= 0)
            {
                Dead();
            }
        }
    }

    public void FallOff()
    {
        GameManager.Singleton.RespawnPlayer(this);
        Hurt();
    }

    void Dead()
    {
        curHp = 0;
        OnHurt?.Invoke(curHp);
        dead = true;
        GameManager.Singleton.GameOver();
    }
}