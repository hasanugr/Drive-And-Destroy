using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public TextMeshProUGUI[] goldTextArr;

    private int currentGold;

    GameManager gm;
    private void Start()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        ChangeGoldText();
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
}
