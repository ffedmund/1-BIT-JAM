using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    public Button restartButton;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(()=>GameManager.Singleton.RestartLevel());
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        LoadVolumeSettings();
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
        musicSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }
}   
