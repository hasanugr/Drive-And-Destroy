using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupController : MonoBehaviour
{
    public GameObject EarnGoldPopup;
    public GameObject ExitGamePopup;
    public GameObject RewardGoldAddedPopup;
    public TextMeshProUGUI GoldCountText;
    public GameObject GoldIcon;
    public GameObject GoldEarnedParticleEffect;


    AudioManager _audioManager;

    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void OpenPopup(string popupName)
    {
        MovePopupAsWant(popupName, 1);
    }

    public void ClosePopup(string popupName)
    {
        _audioManager.Play("Button");
        MovePopupAsWant(popupName, 0);
    }

    public void ClosePopupInstantly(string popupName)
    {
        _audioManager.Play("Button");

        Vector3 closedScale = new Vector3(1, 0, 1);
        switch (popupName)
        {
            case "EarnGold":
                EarnGoldPopup.transform.localScale = closedScale;
                break;
            case "ExitGame":
                ExitGamePopup.transform.localScale = closedScale;
                break;
            case "RewardGoldAdded":
                RewardGoldAddedPopup.transform.localScale = closedScale;
                break;
        }
    }

    public void ChangeEarnedGoldCount(int goldCount)
    {
        _audioManager.Play("EarnedGold");
        GoldEarnedParticleEffect.SetActive(true);

        var seq = LeanTween.sequence();

        seq.append(LeanTween.value(gameObject, 0, goldCount, 1.5f).setOnUpdate((float val) =>
        {
            GoldCountText.text = Mathf.RoundToInt(val).ToString();
        }).setIgnoreTimeScale(true));

        seq.append(LeanTween.scale(GoldIcon, new Vector3(1.2f, 1.2f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch());
    }

    private void MovePopupAsWant(string popupName, int type)
    {
        switch (popupName)
        {
            case "EarnGold":
                LeanTween.scaleY(EarnGoldPopup, type, 0.1f);
                break;
            case "ExitGame":
                LeanTween.scaleY(ExitGamePopup, type, 0.1f);
                break;
            case "RewardGoldAdded":
                LeanTween.scaleY(RewardGoldAddedPopup, type, 0.1f);
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
