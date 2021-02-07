using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBoostController : MonoBehaviour
{
    public GameObject GoldFirst;
    public GameObject GoldLast;
    public GameObject Turbo;
    public GameObject Heal;

    private InGameManager _igm;
    private int _playerScore;
    private int _boostLuckLevel;

    private int[] HealLuckPercent = { 50, 30, 10 };
    private int[] TurboLuckPercent = { 50, 40, 30 };
    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _playerScore = _igm.GetPlayerPoint();
        BoostLuckController();

        TurboAdd();
        HealAdd();
    }

    private void BoostLuckController()
    {
        if (_playerScore < VariableController.FirstLevelEndPoint)
        {
            _boostLuckLevel = 0;
        }
        else if (_playerScore < VariableController.SecondLevelEndPoint)
        {
            _boostLuckLevel = 1;
        }
        else
        {
            _boostLuckLevel = 2;
        }
    }

    private void TurboAdd()
    {
        if (IsShouldWork(TurboLuckPercent[_boostLuckLevel]))
        {
            Turbo.SetActive(true);
        }
    }

    private void HealAdd()
    {
        if (IsShouldWork(HealLuckPercent[_boostLuckLevel]))
        {
            Heal.SetActive(true);
        }
    }

    private bool IsShouldWork(int percent)
    {
        int random = UnityEngine.Random.Range(0, 101);
        bool isIt = random <= percent;
        return isIt;
    }
}
