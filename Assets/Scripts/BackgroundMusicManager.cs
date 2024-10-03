using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Music Playback Settings")]
    public float minDelayBetweenTracks = 30f; // Minimum delay in seconds
    public float maxDelayBetweenTracks = 120f; // Maximum delay in seconds

    private Coroutine musicCoroutine;

    private IEnumerator PlayBackgroundMusic()
    {
        while (true)
        {
            // Choose a random music track from AudioManager
            AudioManager.Sound s = GetRandomMusicTrack();

            if (s != null)
            {
                // Play the track using AudioManager
                AudioManager.Singleton.PlayMusic(s.name);

                // Wait until the track finishes playing
                yield return new WaitWhile(() => AudioManager.Singleton.IsMusicPlaying());

                // Wait for a random delay before playing the next track
                float delay = Random.Range(minDelayBetweenTracks, maxDelayBetweenTracks);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                Debug.LogWarning("No music tracks available to play.");
                yield break; // Exit the coroutine if no tracks are available
            }
        }
    }

    private AudioManager.Sound GetRandomMusicTrack()
    {
        List<AudioManager.Sound> musicTracks = AudioManager.Singleton.musicTracks;

        if (musicTracks != null && musicTracks.Count > 0)
        {
            // int randomIndex = Random.Range(0, musicTracks.Count);
            return musicTracks[1];
        }

        return null;
    }

    public void StopMusic()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }
        AudioManager.Singleton.StopMusic();
    }

    public void StartMusic()
    {
        // Start the background music coroutine
        if (musicCoroutine == null)
        {
            musicCoroutine = StartCoroutine(PlayBackgroundMusic());
        }
    }

}
