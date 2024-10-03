using UnityEngine;
using UnityEngine.SceneManagement; // Include necessary namespace for scene management

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    // public bool unlockShadowPower{get; private set;}
    public BackgroundMusicManager backgroundMusicManager;
    public bool unlockShadowPower;
    private int levelIndex = 0; // Starting with the first level in the list

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            backgroundMusicManager = GetComponent<BackgroundMusicManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        levelIndex++;
        LoadCurrentLevel();
        if(levelIndex == 1)
        {
            backgroundMusicManager.StartMusic();
        }
    }

    public void LoadLevelByIndex(int index)
    {
        if (index >= 0)
        {
            levelIndex = index;
            LoadCurrentLevel();
        }
        else
        {
            Debug.LogError("Invalid level index!");
        }
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        string sceneName = "Floor" + levelIndex;
        if (DoesSceneExist(sceneName))
        {
            SceneTransitionManager.Singleton.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' does not exist. Level index: {levelIndex}");
        }
    }

    private bool DoesSceneExist(string sceneName)
    {
        // Check if the scene is in the build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    public void GameOver()
    {
        RestartLevel();
    }

    public void RespawnPlayer(PlayerStats player)
    {
        GameObject respawnPoint = GameObject.FindWithTag("Respawn");
        PlayerState playerState = player.GetComponent<PlayerState>();
        GameObject normalPlayer = playerState.normalPlayer;

        normalPlayer.transform.position = respawnPoint.transform.position;
    }

    public void UnlockShadowPower() => unlockShadowPower = true;
}
