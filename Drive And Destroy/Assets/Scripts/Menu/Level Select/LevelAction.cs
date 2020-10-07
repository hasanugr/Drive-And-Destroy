using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelAction : MonoBehaviour
{
    public void StartLevel()
    {
        string levelName = transform.name;
        // There is need to some control to check loading scene is available..
        if (levelName == "Level-1")
        {
            SceneManager.LoadScene(levelName);
        }else
        {
            Debug.LogError("You can not access this level right now.!!!");
        } 
    }
}
