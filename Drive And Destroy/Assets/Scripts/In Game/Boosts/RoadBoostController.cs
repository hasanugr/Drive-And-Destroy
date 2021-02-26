using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBoostController : MonoBehaviour
{
    public GameObject CountdownTimer;
    public GameObject GoldFirst;
    public GameObject GoldLast;
    public GameObject Turbo;
    public GameObject Heal;
    public bool IsBoostSpawnActive = true;

    private InGameManager _igm;
    private int _boostLuckLevel;

    private int[] HealLuckPercent = { 50, 30, 20, 10 };
    private int[] TurboLuckPercent = { 50, 40, 30, 20 };
    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        CountdownTimerAdd();

        if (IsBoostSpawnActive) { 
            BoostLuckController();
            TurboAdd();
            HealAdd();
        }
    }

    private void BoostLuckController()
    {
        switch (_igm.GetReachedLevel())
        {
            case 1:
                _boostLuckLevel = 0;
                break;
            case 2:
                _boostLuckLevel = 1;
                break;
            case 3:
                _boostLuckLevel = 2;
                break;
            default:
                _boostLuckLevel = 3;
                break;
        }
    }

    private void CountdownTimerAdd()
    {
        if (CountdownTimer != null)
        {
            if (_igm.StepToAddCountdownReset == 0)
            {
                CountdownTimer.SetActive(true);
                _igm.StepToAddCountdownReset = 3;
            }
            else
            {
                _igm.StepToAddCountdownReset -= 1;
            }
        }
    }

    private void TurboAdd()
    {
        if (Turbo != null)
        {
            if (IsShouldWork(TurboLuckPercent[_boostLuckLevel]))
            {
                Turbo.SetActive(true);
            }
        }
    }

    private void HealAdd()
    {
        if (Heal != null)
        {
            if (IsShouldWork(HealLuckPercent[_boostLuckLevel]))
            {
                Heal.SetActive(true);
            }
        }
    }

    private bool IsShouldWork(int percent)
    {
        int random = UnityEngine.Random.Range(0, 101);
        bool isIt = random <= percent;
        return isIt;
    }
}
