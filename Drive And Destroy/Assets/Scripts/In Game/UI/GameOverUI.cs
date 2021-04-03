using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI PanelTitle;
    public TextMeshProUGUI NewScoreName;
    public TextMeshProUGUI NewScoreValue;
    public TextMeshProUGUI HighScoreName;
    public TextMeshProUGUI HighScoreValue;
    public TextMeshProUGUI GoldValue;
    public Image GoldIcon;
    public Image RestartButtonBG;
    public TextMeshProUGUI RestartButtonText;
    public Image MenuButtonBG;
    public TextMeshProUGUI MenuButtonText;
    public GameObject TouchBlockPanel;

    [SerializeField]
    private int _newScore;
    [SerializeField]
    private int _highScore;
    private int _gold;

    private InGameManager _igm;
    private GameManager _gm;
    AdmobManager _admobManager;

    private void OnEnable()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _admobManager = FindObjectOfType<AdmobManager>();

        AddUIValues();
        CalculateGold();
        StartCoroutine(DisableTheTouchBlock(1.5f));

        Image GameOverUIImage = gameObject.GetComponent<Image>();
        LeanTween.value(gameObject, 0, 1f, 1f).setOnUpdate((float val) =>
        {
            ApplyImageColor(GameOverUIImage, Mathf.Clamp(val, 0, 0.95f));
            ApplyImageColor(GoldIcon, val);
            ApplyImageColor(RestartButtonBG, val);
            ApplyImageColor(MenuButtonBG, val);

            ApplyTextColor(PanelTitle, val);
            ApplyTextColor(NewScoreName, val);
            ApplyTextColor(NewScoreValue, val);
            ApplyTextColor(HighScoreName, val);
            ApplyTextColor(HighScoreValue, val);
            ApplyTextColor(GoldValue, val);
            ApplyTextColor(RestartButtonText, val);
            ApplyTextColor(MenuButtonText, val);

        }).setIgnoreTimeScale(true);

        _gm.SavePlayerData();
    }

    private void OnDisable()
    {
        TouchBlockPanel.SetActive(true);
    }

    private void CalculateGold()
    {
        var seq = LeanTween.sequence();

        _gold = _igm.GetCollectedGold() + (_newScore / 5);
        _gm.AddGold(_gold);

        seq.append(LeanTween.value(gameObject, 0, _gold, 3f).setOnUpdate((float val) =>
       {
           GoldValue.text = Mathf.RoundToInt(val).ToString();
       }).setIgnoreTimeScale(true));

        seq.append(LeanTween.scale(GoldIcon.gameObject.transform.parent.gameObject, new Vector3(1.2f, 1.2f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch());
    }

    private void AddUIValues()
    {
        _newScore = _igm.GetPlayerPoint();
        _highScore = _gm.pd.highScore;

        if (_newScore > _highScore)
        {
            _highScore = _newScore;
            _gm.pd.highScore = _newScore;
            LeanTween.scale(HighScoreName.gameObject.transform.parent.gameObject, new Vector3(1.2f, 1.2f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch().setLoopPingPong();
        }

        NewScoreValue.text = _newScore.ToString();
        HighScoreValue.text = _highScore.ToString();
    }

    private void ApplyImageColor(Image image, float val)
    {
        Color textColor = image.color;
        textColor.a = val;
        image.color = textColor;
    }
    
    private void ApplyTextColor(TextMeshProUGUI text, float val)
    {
        Color textColor = text.color;
        textColor.a = val;
        text.color = textColor;
    }

    private void WatchInterstitialAdsWithControl()
    {
        int playedGameCount = _gm.GetPlayedGameCount();
        if (playedGameCount % 2 == 0)
        {
            _admobManager.ShowInterstitial();
        }
    }

    IEnumerator DisableTheTouchBlock(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        WatchInterstitialAdsWithControl();

        TouchBlockPanel.SetActive(false);
    }
}
