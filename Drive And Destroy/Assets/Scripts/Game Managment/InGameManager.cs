using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject inGameUI;
    public GameObject lateLoadObject;
    public GameObject cameraHolder;

    public GameObject spawnPoint;

    private GameObject _player;

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


}
