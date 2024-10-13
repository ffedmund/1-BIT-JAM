using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Slider slider;
    public RectTransform hpBarTransform;
    public TextMeshProUGUI bossTitle;

    private int maxHp;
    private int curHp;

    public Action<int> SetUp(int maxHp, string bossName)
    {
        this.maxHp = maxHp;
        curHp = maxHp;
        slider = GetComponent<Slider>();
        slider.maxValue = maxHp;
        slider.value = curHp;
        bossTitle.SetText(bossName);

        hpBarTransform.anchoredPosition = new Vector2(0,81);
        hpBarTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.InOutQuad);

        return UpdateHp;
    }

    private void UpdateHp(int hp)
    {
        slider.DOValue(hp,0.5f);
    }
}
