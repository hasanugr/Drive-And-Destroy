using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public GameObject Highscore;
    public TextMeshProUGUI HighscoreText;
    public TextMeshProUGUI[] goldTextArr;

    private int currentGold;

    GameManager gm;
    private void Start()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        ChangeGoldText();
        HighscoreShow();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGold != gm.pd.gold)
        {
            currentGold = gm.pd.gold;
            ChangeGoldText();
        }
    }

    private void ChangeGoldText()
    {
        foreach(TextMeshProUGUI goldText in goldTextArr)
        {
            goldText.text = currentGold.ToString();
        }
    }

    private void HighscoreShow()
    {
        if (gm.pd.highScore > 0)
        {
            HighscoreText.text = gm.pd.highScore.ToString();
            Highscore.SetActive(true);
        }
    }
}
