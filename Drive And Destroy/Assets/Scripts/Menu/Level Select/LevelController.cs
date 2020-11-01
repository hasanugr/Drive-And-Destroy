using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public Texture2D starTexFull;
    public Texture2D starTexEmpty;

    private int SizeOfLevelGroups;
    private int ActiveLevelGroup;

    private string selectedLevel;

    GameManager gm;
    void OnEnable()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();

        int[] levels = gm.pd.levels;
        int levelReached = levels.Length;
        selectedLevel = levelReached.ToString();
        SizeOfLevelGroups = LevelGroups.Length - 1;
        ActiveLevelGroup = Mathf.FloorToInt(levelReached / 15);
        LevelGroups[ActiveLevelGroup].SetActive(true);
        SliderButtonsActiveControl();
        selectSelectedButton();

        PrepareLevelBoxes(levelReached, levels);

        /*for (int i = 1; i <= 45; i++)
        {
            GameObject asd = GameObject.Find("Level-" + i.ToString());
            Debug.Log(asd.transform.name);
        }*/
    }

    private void Update()
    {
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (selectedGameObject == null)
        {
            selectSelectedButton();
        }
    }

    

    void PrepareLevelBoxes(int levelReached, int[] levels)
    {
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            Button thisLevelButton = LevelButtons[i].GetComponent<Button>();
            Transform thisButtonTransform = LevelButtons[i].transform;
            Transform tbUnlockedTransform = thisButtonTransform.GetChild(0);
            Transform tbLockedTransform = thisButtonTransform.GetChild(1);
            GameObject tbUnlocked = tbUnlockedTransform.gameObject;
            GameObject tbLocked = tbLockedTransform.gameObject;

            // If this level is reached level do these. 
            if (i + 1 <= levelReached)
            {
                thisLevelButton.interactable = true;
                tbUnlocked.SetActive(true);
                tbLocked.SetActive(false);

                GameObject stars = tbUnlockedTransform.GetChild(1).gameObject;
                if (levels[i] == 0)
                {
                    // If this level active but not played yet or no gained star hide stars object.
                    stars.SetActive(false);
                }else
                {
                    // Show gained stars top of level object;
                    for (int j = 0; j < 3; j++)
                    {
                        GameObject thisStar = stars.transform.GetChild(j).gameObject;
                        thisStar.GetComponent<RawImage>().texture = (j + 1 <= levels[i]) ? starTexFull : starTexEmpty;
                    }
                }
            }else // If this level is not reached level do these.
            {
                thisLevelButton.interactable = false;
                tbUnlocked.SetActive(false);
                tbLocked.SetActive(true);
            }


            if (i + 1 > levelReached)
            {
                //Button thisLevelButton = LevelButtons[i].GetComponent<Button>();
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

    void selectSelectedButton()
    {
        GameObject selectedButton = GameObject.Find("Level-" + selectedLevel);
        if (selectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Level-" + selectedLevel));
        }
    }

    public void SlideLeft()
    {
        LevelGroups[ActiveLevelGroup].SetActive(false);
        ActiveLevelGroup -= 1;
        LevelGroups[ActiveLevelGroup].SetActive(true);

        SliderButtonsActiveControl();
        selectSelectedButton();
    }
    
    public void SlideRight()
    {
        LevelGroups[ActiveLevelGroup].SetActive(false);
        ActiveLevelGroup += 1;
        LevelGroups[ActiveLevelGroup].SetActive(true);

        SliderButtonsActiveControl();
        selectSelectedButton();
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
