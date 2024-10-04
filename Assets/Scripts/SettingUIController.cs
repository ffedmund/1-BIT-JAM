using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    public Button restartButton;
    public Button attackKeyButton;
    public TextMeshProUGUI attackKeyText;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(()=>GameManager.Singleton.RestartLevel());
        attackKeyButton.onClick.AddListener(()=>SwapAttackKey());
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        LoadVolumeSettings();
        LoadAttackKey();
    }

    private void LoadAttackKey()
    {
        string attackKey = PlayerPrefs.GetString("AttackKey", "J");
        attackKeyText.SetText(attackKey);
    }

    private void SwapAttackKey()
    {
        string attackKey = PlayerPrefs.GetString("AttackKey", "J");
        PlayerPrefs.SetString("AttackKey", attackKey.Equals("J")?"F":"J");
        PlayerPrefs.Save();
        attackKeyText.SetText(attackKey.Equals("J")?"F":"J");
    }

    void OnEnable()
    {
        AudioManager.Singleton?.PlaySFX("UIOpen");
    }

    void OnDisable()
    {
        AudioManager.Singleton?.PlaySFX("UIClose");
    }

    private void SetMusicVolume(float arg0)
    {
        AudioManager.Singleton.SetMusicVolume(arg0);
    }

    private void SetSFXVolume(float arg0)
    {
        AudioManager.Singleton.SetSFXVolume(arg0);
    }

    private void LoadVolumeSettings()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
}   
