using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    bool gameStart = false;

    private void Start() {
        AudioManager.Singleton?.PlayMusic("Menu");
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && !gameStart)
        {
            // AudioManager.Singleton?.StopMusic();
            GameManager.Singleton.NextLevel();
            gameStart = true;
        } 
    }
}
