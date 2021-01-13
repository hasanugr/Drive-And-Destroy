using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [Header("General")]
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject inGameUI;
    public GameObject lateLoadObject;
    public GameObject cameraHolder;

    public GameObject spawnPoint;
    
    private GameObject _player;

    [Header("Road Creator")]
    public GameObject StraightRoad;
    public GameObject SoftLeftRoad;
    public GameObject SoftRightRoad;
    public GameObject HardLeftRoad;
    public GameObject HardRightRoad;
    public List<GameObject> activeRoads;

    private Vector3 insPosition;
    private Quaternion insRotation;
    private Vector3 insForward;

    private int blockStepLeft = 0;
    private int blockStepRight = 0;


    GameManager gm;
    private void Start()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        SpawnTheVehicle();
        ProcessAfterLoad();
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
        inGameUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void RoadChangeTrigger()
    {
        if (activeRoads.Count > 6)
        {
            DestroyLastRoad();
        }
        CreateNewRoad();
    }

    private void SpawnTheVehicle()
    {
        Vector3 spawnPosition = spawnPoint.transform.position;
        Quaternion spawnRotation = spawnPoint.transform.rotation;

        _player = Instantiate(gm.selectedVehicle, spawnPosition, spawnRotation);
        _player.transform.parent = transform;
    }

    private void ProcessAfterLoad()
    {
        cameraHolder.GetComponent<CameraFollow>().enabled = true;
        lateLoadObject.SetActive(true);
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
        insForward = LastObjectChildTransform.forward;

        GameObject _newRoad;
        do
        {
            _newRoad = GetNewRoad();
        } while (!IsFreeForNewRoad(_newRoad.name));
        
        GameObject _lastRoad = Instantiate(_newRoad, insPosition, insRotation);
        activeRoads.Add(_lastRoad);
        _newRoad = null;
    }

    private bool IsFreeForNewRoad(string selectedNewRoadName)
    {
        bool _isFree = true;
        float[] _checkLengths = new float[3];
        float[] _checkDeflections = new float[3];

        switch (selectedNewRoadName)
        {
            case "Straight Road":
                _checkLengths = new float[] { 400f, 400f, 400f };
                _checkDeflections = new float[] { -0.1f, 0f, 0.1f };
                break;
            case "Soft Left Road":
                _checkLengths = new float[] { 600f, 450f, 350f };
                _checkDeflections = new float[] { -0.6f, -0.25f, 0f };
                break;
            case "Soft Right Road":
                _checkLengths = new float[] { 600f, 450f, 350f };
                _checkDeflections = new float[] { 0.6f, 0.25f, 0f };
                break;
            case "Hard Left Road":
                _checkLengths = new float[] { 400f, 350f, 300f };
                _checkDeflections = new float[] { -1f, -0.5f, 0f };
                break;
            case "Hard Right Road":
                _checkLengths = new float[] { 400f, 350f, 300f };
                _checkDeflections = new float[] { 1f, 0.5f, 0f };
                break;
        }

        for (int i = 0; i < _checkLengths.Length; i++)
        {
            RaycastHit _hit;
            Ray myRay = new Ray(insPosition, insForward + (transform.right * _checkDeflections[i]));
            if (Physics.Raycast(myRay, out _hit, _checkLengths[i]))
            {
                _isFree = false;
                break;
            }
        }

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

        int _randomNumber = Random.Range(0, _roads.Length);

        GameObject selectedRoad = _roads[_randomNumber];
        return selectedRoad;
    }

}
