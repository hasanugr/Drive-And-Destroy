using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerForRewardedAd : MonoBehaviour
{
    [SerializeField]
    private GameObject rewardedAdCoinParticleSystem;

    GameManager _gm;
    AdmobManager _admobManager;
    PopupController _popupController;

    private bool _isUserEarnedRewardProccessin = false;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        _admobManager = FindObjectOfType<AdmobManager>();
        _popupController = GameObject.Find("Popup Controller").GetComponent<PopupController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_admobManager.IsUserEarnedReward && !_isUserEarnedRewardProccessin)
        {
            _isUserEarnedRewardProccessin = true;
            GiveRewardedGold();
        }else if(_isUserEarnedRewardProccessin && !_admobManager.IsUserEarnedReward)
        {
            _isUserEarnedRewardProccessin = false;
        }
    }

    public void GiveRewardedGold()
    {
        // When watched Rewarded Ads successfully.
        int earnedGoldCount = UnityEngine.Random.Range(100, 400);
        _popupController.ChangeEarnedGoldCount(earnedGoldCount);
        _popupController.OpenPopup("RewardGoldAdded");
        rewardedAdCoinParticleSystem.SetActive(true);
        _gm.AddGold(earnedGoldCount);

        StartCoroutine(WaitAndStopRewardProccess(1));
    }

    IEnumerator WaitAndStopRewardProccess(float time)
    {
        yield return new WaitForSeconds(time);
        _admobManager.IsUserEarnedReward = false;
    }
}
