using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public event Action<int> OnHurt;
    public int maxHp = 3;
    public int curHp = 0;

    void Awake()
    {
        curHp = maxHp;
    }

    public void Hurt()
    {
        curHp--;
        OnHurt?.Invoke(curHp);
    }
}