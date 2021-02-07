using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject EarnGoldPopup;


    public void OpenPopup(string popupName)
    {
        MovePopupAsWant(popupName, 1);
    }

    public void ClosePopup(string popupName)
    {
        MovePopupAsWant(popupName, 0);
    }

    private void MovePopupAsWant(string popupName, int type)
    {
        switch (popupName)
        {
            case "EarnGold":
                LeanTween.scaleY(EarnGoldPopup, type, 0.1f);
                break;
        }
    }
}
