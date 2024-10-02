using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    bool gameStart = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && !gameStart)
        {
            GameManager.Singleton.NextLevel();
            gameStart = true;
        } 
    }
}
