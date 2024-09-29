using UnityEngine;
using UnityEngine.SceneManagement; // Include necessary namespace for scene management

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

    private int levelIndex = 0; // Starting with the first level in the list

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

    public void NextLevel()
    {
        levelIndex++;
        LoadCurrentLevel();
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
            SceneManager.LoadScene(sceneName);
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
}
