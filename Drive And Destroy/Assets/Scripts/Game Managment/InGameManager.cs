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
    public int MaxRoadInScene = 6;
    public GameObject StraightRoad;
    public GameObject SoftLeftRoad;
    public GameObject SoftRightRoad;
    public GameObject HardLeftRoad;
    public GameObject HardRightRoad;
    public GameObject BossModeRoad;
    public GameObject NextLevelRoad;
    public List<GameObject> activeRoads;
    public int StepToAddCountdownReset = 3;

    [SerializeField]
    private List<string> _activeRoadsNames;
    private Vector3 insPosition;
    private Quaternion insRotation;
    
    private int blockStepLeft = 0;
    private int blockStepRight = 0;

    private ObjectPooler _straightRoadPool;
    private ObjectPooler _softLeftRoadPool;
    private ObjectPooler _softRightRoadPool;
    private ObjectPooler _hardLeftRoadPool;
    private ObjectPooler _hardRightRoadPool;
    private ObjectPooler _bossRoadPool;
    private ObjectPooler _nextLevelRoadPool;

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

        CreatePoolObjects();
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
        _gm.PlayedGameCountIncrease();
        GameIsStarted = true;
        cameraHolder.GetComponent<CameraFollow>().positionSmoothTime = 0.01f;
        yield return new WaitForSeconds(1f);
        TimerUIObj.transform.parent.gameObject.SetActive(false);
    }

    private void CreatePoolObjects()
    {
        _straightRoadPool = new ObjectPooler(StraightRoad);
        _straightRoadPool.FillThePool(6);
        _softLeftRoadPool = new ObjectPooler(SoftLeftRoad);
        _softLeftRoadPool.FillThePool(3);
        _softRightRoadPool = new ObjectPooler(SoftRightRoad);
        _softRightRoadPool.FillThePool(3);
        _hardLeftRoadPool = new ObjectPooler(HardLeftRoad);
        _hardLeftRoadPool.FillThePool(3);
        _hardRightRoadPool = new ObjectPooler(HardRightRoad);
        _hardRightRoadPool.FillThePool(3);
        _bossRoadPool = new ObjectPooler(BossModeRoad);
        _bossRoadPool.FillThePool(6);
        _nextLevelRoadPool = new ObjectPooler(NextLevelRoad);
        _nextLevelRoadPool.FillThePool(1);

        // Create 3 Road too before start
        RoadChangeTrigger();
        RoadChangeTrigger();
        RoadChangeTrigger();
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
        if (activeRoads.Count >= MaxRoadInScene)
        {
            DestroyLastRoad();
        }
        CreateNewRoad();
    }

    public Vector3 GetActiveTerrainPosition()
    {
        return Terrains[_activeTerrain].transform.position;
    }

    public void ChangeTerrainPosition(string fixWay)
    {
        Transform activeTerrainTransform = Terrains[_activeTerrain].transform;
        Vector3 tempPos = activeTerrainTransform.position;
        switch (fixWay)
        {
            case "Vertical":
                tempPos.z = _player.transform.position.z - 1000;
                break;
            case "Horizontal":
                tempPos.x = _player.transform.position.x - 1000;
                break;
        }
        activeTerrainTransform.position = tempPos;
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
        string roadName = _activeRoadsNames[0];
        switch (roadName)
        {
            case "Straight":
                _straightRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "SoftLeft":
                _softLeftRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "SoftRight":
                _softRightRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "HardLeft":
                _hardLeftRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "HardRight":
                _hardRightRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "BossMode":
                _bossRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            case "NextLevel":
                _nextLevelRoadPool.SendObjectToPool(activeRoads[0]);
                break;
            default:
                print(roadName + " object is couldn't send to pool.!");
                break;
        }
        activeRoads.RemoveAt(0);
        _activeRoadsNames.RemoveAt(0);
    }

    private void CreateNewRoad()
    {
        if (activeRoads.Count <= 0)
        {
            insPosition = transform.position;
            insRotation = transform.rotation;
        }
        else
        {
            Transform LastObjectChildTransform = activeRoads[activeRoads.Count - 1].transform.GetChild(1).transform;
            insPosition = LastObjectChildTransform.position;
            insRotation = LastObjectChildTransform.rotation;
        }

        string newRoadName;
        do
        {
            newRoadName = GetNewRoad();
        } while (!IsFreeForNewRoad(newRoadName));

        GameObject _lastRoad;
        switch (newRoadName)
        {
            case "Straight":
                _lastRoad = _straightRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "SoftLeft":
                _lastRoad = _softLeftRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "SoftRight":
                _lastRoad = _softRightRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "HardLeft":
                _lastRoad = _hardLeftRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "HardRight":
                _lastRoad = _hardRightRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "BossMode":
                _lastRoad = _bossRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            case "NextLevel":
                _lastRoad = _nextLevelRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
            default:
                _lastRoad = _straightRoadPool.GetObjectFromPoolAtPosition(insPosition, insRotation);
                break;
        }
        activeRoads.Add(_lastRoad);
        _activeRoadsNames.Add(newRoadName);

        // The traps won't add to road when boss activated.
        if (IsBossActivated)
        {
            _createdBossRoad++;
            _lastRoad.GetComponent<BossRoadControl>().StartProccess(_reachedLevel, _createdBossRoad);
        }
    }

    private bool IsFreeForNewRoad(string selectedNewRoadName)
    {
        bool _isFree = true;

        if ((selectedNewRoadName == "SoftLeft" || selectedNewRoadName == "HardLeft"))
        {
            blockStepLeft = 2;
            blockStepRight = 0;
        }
        else if ((selectedNewRoadName == "SoftRight" || selectedNewRoadName == "HardRight"))
        {
            blockStepRight = 2;
            blockStepLeft = 0;
        }

        return _isFree;
    }

    private string GetNewRoad()
    {
        string[] _roads = { "Straight", "SoftLeft", "SoftRight", "HardLeft", "HardRight" };

        if (IsBossActivated)
        {
            _roads = new string[] { "BossMode" };
        }else if (IsNextLevelTime)
        {
            _roads = new string[] { "NextLevel" };
            IsNextLevelTime = false;
            _createdBossRoad = 0;
        }
        else
        {
            if (activeRoads.Count < 3)
            {
                _roads = new string[] { "Straight" };
            }
            else
            {
                if (blockStepLeft > 0)
                {
                    _roads = new string[] { "Straight", "SoftRight", "HardRight" };
                    blockStepLeft -= 1;
                }
                else if (blockStepRight > 0)
                {
                    _roads = new string[] { "Straight", "SoftLeft", "HardLeft" };
                    blockStepRight -= 1;
                }
            }
        }

        int _randomNumber = Random.Range(0, _roads.Length);

        string selectedRoad = _roads[_randomNumber];
        return selectedRoad;
    }

    private void ActivateTheBoss()
    {
        IsBossActivated = true;
    }
}
