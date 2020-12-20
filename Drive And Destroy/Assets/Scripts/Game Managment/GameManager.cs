//This script manages the timing and flow of the game. It is also responsible for telling
//the UI when and how to update

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	//The game manager holds a public static reference to itself. This is often referred to
	//as being a "singleton" and allows it to be access from all other objects in the scene.
	//This should be used carefully and is generally reserved for "manager" type objects
	public static GameManager instance;		

	[Header("Game Data")]
	public PlayerData pd;                   //The game data of player.
	public GameObject[] vehicles;
	public GameObject selectedVehicle;

	[Header("Race Settings")]
	public int heal = 100;                  //The heal of vehicle
	public int numberOfLaps = 1;			//The number of laps to complete
	public VehicleMovement vehicleMovement; //A reference to the ship's VehicleMovement script

	[Header("UI References")]
	public ShipUI shipUI;					//A reference to the ship's ShipUI script
	public LapTimeUI lapTimeUI;				//A reference to the LapTimeUI script in the scene
	public GameObject gameOverUI;			//A reference to the UI objects that appears when the game is complete
/*
	float[] lapTimes;						//An array containing the player's lap times
	int currentLap = 0;						//The current lap the player is on
	bool isGameOver;						//A flag to determine if the game is over
	bool raceHasBegun;  */                    //A flag to determine if the race has begun


	void Awake()
	{
		MakeSingleton();
		LoadPlayerData();
	}

	private void MakeSingleton()
    {
		if (instance != null)
        {
			Destroy(gameObject);
        }else
        {
			instance = this;
			DontDestroyOnLoad(gameObject);
        }
    }

	private void LoadPlayerData()
    {
		pd = SaveLoadManager.Load();
		ChangeActiveVehicle(pd.selectedVehicleId);
    }

	void Update()
	{
        if (Input.GetKeyDown(KeyCode.N))
        {
            //pd.selectedVehicleId += 1;
            pd.gold += 100;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //pd.selectedVehicleId -= 1;
            pd.gold -= 100;
        }
    }

	public void ChangeActiveVehicle(int vehicleId)
	{
		pd.selectedVehicleId = vehicleId;
		for (int i = 0; i < vehicles.Length; i++)
		{
			if (vehicles[i].name == vehicleId.ToString())
			{
				selectedVehicle = vehicles[i];
				break;
			}
		}
	}

	public bool IsActiveGame()
	{
		//If the race has begun and the game is not over, we have an active game
		return true; //raceHasBegun && !isGameOver;
	}

}
