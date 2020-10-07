using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public GameObject[] LevelButtons;
    public GameObject[] LevelGroups;
    public GameObject SlideLeftButton;
    public GameObject SlideRightButton;

    private int SizeOfLevelGroups;
    private int ActiveLevelGroup;

    private string selectedLevel;

    void OnEnable()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        SizeOfLevelGroups = LevelGroups.Length - 1;
        ActiveLevelGroup = Mathf.FloorToInt(levelReached / 15);
        LevelGroups[ActiveLevelGroup].SetActive(true);
        SliderButtonsActiveControl();

        PrepareLevelBoxes(levelReached);
    }

    void PrepareLevelBoxes(int levelReached)
    {
        EventSystem.current.SetSelectedGameObject(LevelButtons[levelReached - 1]);

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                Button thisLevelButton = LevelButtons[i].GetComponent<Button>();
                thisLevelButton.interactable = false;
            }
        }
    }

    void SliderButtonsActiveControl()
    {
        if (ActiveLevelGroup == 0)
        {
            SlideLeftButton.SetActive(false);
            SlideRightButton.SetActive(true);
        }
        else if (ActiveLevelGroup == SizeOfLevelGroups)
        {
            SlideRightButton.SetActive(false);
            SlideLeftButton.SetActive(true);
        }
        else
        {
            SlideLeftButton.SetActive(true);
            SlideRightButton.SetActive(true);
        }
    }

    public void SlideLeft()
    {
        LevelGroups[ActiveLevelGroup].SetActive(false);
        ActiveLevelGroup -= 1;
        LevelGroups[ActiveLevelGroup].SetActive(true);

        SliderButtonsActiveControl();
    }
    
    public void SlideRight()
    {
        LevelGroups[ActiveLevelGroup].SetActive(false);
        ActiveLevelGroup += 1;
        LevelGroups[ActiveLevelGroup].SetActive(true);

        SliderButtonsActiveControl();
    }

    void OnDisable()
    {
        ActiveLevelGroup = 0;

        for (int i = 0; i <= SizeOfLevelGroups; i++)
        {
            LevelGroups[i].SetActive(false);
        }

        SlideLeftButton.SetActive(true);
        SlideRightButton.SetActive(true);
    }

    public void SelectLevel(string levelName)
    {
        selectedLevel = levelName;
    }
    
    public void StartLevel()
    {
        // There is need to some control to check loading scene is available..
        if (selectedLevel == "1")
        {
            SceneManager.LoadScene(selectedLevel);
        }
        else
        {
            Debug.LogError("You can not access this level right now.!!!");
        }
    }
}
