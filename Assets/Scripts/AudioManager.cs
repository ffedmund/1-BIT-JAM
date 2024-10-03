using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Sound Effects (SFX)")]
    public List<Sound> sfxSounds;
    public AudioSource sfxSource;

    [Header("Music")]
    public List<Sound> musicTracks;
    public AudioSource musicSource;

    private float sfxVolume = 1f;
    private float musicVolume = 0.5f;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    #region SFX Functions

    public void PlaySFX(string soundName)
    {
        Sound s = sfxSounds.Find(sfx => sfx.name == soundName);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFX sound not found: " + soundName);
        }
    }

    #endregion

    #region Music Functions

    public void PlayMusic(string trackName, float fadeDuration = 1.0f)
    {
        Sound s = musicTracks.Find(music => music.name == trackName);
        if (s != null)
        {
            // If music is already playing, fade out the current track before switching
            if (musicSource.isPlaying)
            {
                StartCoroutine(FadeOutMusicAndChangeTrack(s, fadeDuration));
            }
            else
            {
                // Play immediately if no music is playing
                musicSource.clip = s.clip;
                musicSource.volume = musicVolume;
                musicSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("Music track not found: " + trackName);
        }
    }

    private IEnumerator FadeOutMusicAndChangeTrack(Sound newTrack, float fadeDuration)
    {
        float startVolume = musicSource.volume;

        // Gradually decrease the volume of the current music
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newTrack.clip;
        musicSource.volume = 0;  // Start the new track at volume 0
        musicSource.Play();

        // Gradually increase the volume of the new track
        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += musicVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = musicVolume;  // Ensure the volume reaches the final level
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }


    #endregion

    #region Volume Control

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume; // Update live music volume
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = musicVolume;
    }

    #endregion
}
