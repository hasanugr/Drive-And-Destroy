using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;

public class InGameManager : MonoBehaviour
{
    [Header("General")]
    [HideInInspector]
    public bool GameIsStarted = false;
    [HideInInspector]
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject GameOverUI;
    public GameObject inGameUI;
    public GameObject lateLoadObject;
    public PlayerStatusTextController PlayerStatusTextController;
    public GameObject cameraHolder;
    public GameObject spawnPoint;
    public GameObject StartPointEnvironment;

    [HideInInspector]
    public bool IsVirusEffected = false;
    [HideInInspector]
    public bool IsBossActivated = false;
    [HideInInspector]
    public bool IsBossAlive = false;
    [HideInInspector]
    public bool IsNextLevelTime = false;
    [HideInInspector]
    public int BossFullHealth;
    [HideInInspector]
    public int BossCurrentHealth;
    private int _createdBossRoad = 0;
    [SerializeField]
    private int _reachedLevel = 1;

    private GameObject _player;
    private int _playerScore = 0;
    private int _collectedGold = 0;

    [Header("Road Creator")]
    public GameObject StraightRoad;
    public GameObject SoftLeftRoad;
    public GameObject SoftRightRoad;
    public GameObject HardLeftRoad;
    public GameObject HardRightRoad;
    public GameObject BossModeRoad;
    public GameObject NextLevelRoad;
    public List<GameObject> activeRoads;
    public int StepToAddCountdownReset = 3;

    private Vector3 insPosition;
    private Quaternion insRotation;
    // private Vector3 insForward;

    private int blockStepLeft = 0;
    private int blockStepRight = 0;

    [Header("Game Countdown Timer")]
    public float TimerMaxValue = 3;
    public GameObject TimerUIObj;
    public TextMeshProUGUI TimerUI;

    private float _timerCountdown;

    [Header("Player Control UI")]
    public GameObject PlayerControlUIMode1;
    public GameObject PlayerControlUIMode2;
    public GameObject PlayerControlUIMode3;

    private int _selectedPlayerControlUIMode;

    [Header("Terrain Control")]
    public GameObject[] Terrains;

    private int _activeTerrain = 0;

    GameManager _gm;
    private void Start()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        _selectedPlayerControlUIMode = _gm.pd.vehicleControlType;
        Time.timeScale = 1f;
        _timerCountdown = TimerMaxValue;

        SpawnTheVehicle();
        StartCoroutine(CountdownToStart());
        cameraHolder.GetComponent<CameraFollow>().enabled = true;
    }

    IEnumerator CountdownToStart()
    {
        while (_timerCountdown > 0)
        {
            yield return new WaitForSeconds(1f);
            TimerUI.text = _timerCountdown.ToString();
            LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1), 0.5f).setEasePunch();
            _timerCountdown--;
        }

        yield return new WaitForSeconds(1f);
        TimerUI.text = "GO";
        LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1), 1f).setEaseInExpo();
        LeanTween.value(TimerUIObj, 0, 1f, 1f).setOnUpdate((float val) =>
        {
            Color textColor = TimerUI.color;
            textColor.a = 1 - val;
            TimerUI.color = textColor;
        });
        ProcessAfterLoad();
        GameIsStarted = true;
        cameraHolder.GetComponent<CameraFollow>().positionSmoothTime = 0.01f;
        yield return new WaitForSeconds(1f);
        TimerUIObj.transform.parent.gameObject.SetActive(false);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        inGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        inGameUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pauseMenuUI.SetActive(false);
        //inGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }
    public void GameOver()
    {
        CameraFollow cameraFollow = cameraHolder.GetComponent<CameraFollow>();
        float distance = cameraFollow.distance;
        float height = cameraFollow.height;
        LeanTween.value(GameOverUI, 0, 2f, 3f).setOnUpdate((float val) =>
        {
            cameraFollow.distance = distance + val;
            cameraFollow.height = height + val * 3;
        }).setIgnoreTimeScale(true);
        

        LeanTween.value(GameOverUI, 1, 0.5f, 1f).setOnUpdate((float val) =>
        {
            Time.timeScale = val;
        }).setIgnoreTimeScale(true);

        GameOverUI.SetActive(true);
        inGameUI.SetActive(false);
        CrossPlatformInputManager.SetButtonUp("Fire");
    }

    public void AddPlayerPoint(int point)
    {
        _playerScore += point;

        LevelController();
    }
    public int GetPlayerPoint()
    {
        return _playerScore;
    }

    public int GetReachedLevel()
    {
        return _reachedLevel;
    }

    public void AddGold(int count)
    {
        _collectedGold += count;
    }
    public int GetCollectedGold()
    {
        return _collectedGold;
    }

    public void ResetCountdownTimer()
    {
        PlayerStatusTextController.ResetTheCounter();

        if(StartPointEnvironment != null)
        {
            Destroy(StartPointEnvironment);
        }
    }

    public void ActivateVirusEffectStatus(string virusType, float duration)
    {
        PlayerStatusTextController.ActivateTheVirusCounter(virusType, duration);
    }

    public void RoadChangeTrigger()
    {
        if (activeRoads.Count > 6)
        {
            DestroyLastRoad();
        }
        CreateNewRoad();
    }

    public Vector3 GetActiveTerrainPosition()
    {
        return Terrains[_activeTerrain].transform.position;
    }

    public void ChangeTerrainPosition(Vector3 pos)
    {
        Transform activeTerrainTransform = Terrains[_activeTerrain].transform;
        activeTerrainTransform.position = pos;
    }

    public void ChangeActiveTerrain()
    {
        Vector3 tempPos = Terrains[_activeTerrain].transform.position;
        Terrains[_activeTerrain].SetActive(false);
        _activeTerrain = Mathf.Clamp((_reachedLevel - 1), 0, 3);
        Terrains[_activeTerrain].transform.position = new Vector3(tempPos.x, Terrains[_activeTerrain].transform.position.y, tempPos.z);
        Terrains[_activeTerrain].SetActive(true);
    }

    public void IncreaseReachedLevel()
    {
        _reachedLevel += 1;
        print("IncreaseReachedLevel --> " + _reachedLevel);
    }

    private void LevelController()
    {
        if ((_playerScore > VariableController.FirstLevelEndPoint && _reachedLevel == 1) || 
            (_playerScore > VariableController.SecondLevelEndPoint && _reachedLevel == 2) || 
            (_playerScore > VariableController.ThirthLevelEndPoint && _reachedLevel == 3) ||
            ((_reachedLevel >= 4) && (_playerScore > (VariableController.ThirthLevelEndPoint - VariableController.SecondLevelEndPoint) * _reachedLevel)))
        {
            ActivateTheBoss();
        }
    }

    private void SpawnTheVehicle()
    {
        Vector3 spawnPosition = spawnPoint.transform.position;
        Quaternion spawnRotation = spawnPoint.transform.rotation;

        _player = Instantiate(_gm.selectedVehicle, spawnPosition, spawnRotation);
        _player.transform.parent = transform;
    }

    private void ProcessAfterLoad()
    {
        lateLoadObject.SetActive(true);

        switch(_selectedPlayerControlUIMode)
        {
            case 0:
                PlayerControlUIMode1.SetActive(true);
                PlayerControlUIMode2.SetActive(false);
                PlayerControlUIMode3.SetActive(false);
                break;
            case 1:
                PlayerControlUIMode1.SetActive(false);
                PlayerControlUIMode2.SetActive(true);
                PlayerControlUIMode3.SetActive(false);
                break;
            case 2:
                PlayerControlUIMode1.SetActive(false);
                PlayerControlUIMode2.SetActive(false);
                PlayerControlUIMode3.SetActive(true);
                break;
        }
    }

    private void DestroyLastRoad()
    {
        Destroy(activeRoads[0]);
        activeRoads.RemoveAt(0);
    }

    private void CreateNewRoad()
    {
        Transform LastObjectChildTransform = activeRoads[activeRoads.Count - 1].transform.GetChild(1).transform;
        insPosition = LastObjectChildTransform.position;
        insRotation = LastObjectChildTransform.rotation;
        // insForward = LastObjectChildTransform.forward;

        GameObject _newRoad;
        do
        {
            _newRoad = GetNewRoad();
        } while (!IsFreeForNewRoad(_newRoad.name));
        
        GameObject _lastRoad = Instantiate(_newRoad, insPosition, insRotation);
        activeRoads.Add(_lastRoad);

        // The traps won't add to road when boss activated.
        if (IsBossActivated)
        {
            _createdBossRoad++;
            _lastRoad.GetComponent<BossRoadControl>().StartProccess(_reachedLevel, _createdBossRoad);
        }
        else
        {
            _lastRoad.GetComponent<RoadBarricadeControl>().AddBarricades(_reachedLevel);
        }
    }

    private bool IsFreeForNewRoad(string selectedNewRoadName)
    {
        bool _isFree = true;

        if ((selectedNewRoadName == "Soft Left Road" || selectedNewRoadName == "Hard Left Road"))
        {
            blockStepLeft = 2;
            blockStepRight = 0;
        }
        else if ((selectedNewRoadName == "Soft Right Road" || selectedNewRoadName == "Hard Right Road"))
        {
            blockStepRight = 2;
            blockStepLeft = 0;
        }

        return _isFree;
    }

    private GameObject GetNewRoad()
    {
        GameObject[] _roads = { StraightRoad, SoftLeftRoad, SoftRightRoad, HardLeftRoad, HardRightRoad };

        if (IsBossActivated)
        {
            _roads = new GameObject[] { BossModeRoad };
        }else if (IsNextLevelTime)
        {
            _roads = new GameObject[] { NextLevelRoad };
            IsNextLevelTime = false;
            _createdBossRoad = 0;
        }
        else
        {
            if (blockStepLeft > 0)
            {
                _roads = new GameObject[] { StraightRoad, SoftRightRoad, HardRightRoad };
                blockStepLeft -= 1;
            }
            else if (blockStepRight > 0)
            {
                _roads = new GameObject[] { StraightRoad, SoftLeftRoad, HardLeftRoad };
                blockStepRight -= 1;
            }
        }

        int _randomNumber = Random.Range(0, _roads.Length);

        GameObject selectedRoad = _roads[_randomNumber];
        return selectedRoad;
    }

    private void ActivateTheBoss()
    {
        IsBossActivated = true;
    }
}
