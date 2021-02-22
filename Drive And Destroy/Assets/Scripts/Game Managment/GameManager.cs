using UnityEngine;

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
	public VehicleMovement vehicleMovement; //A reference to the ship's VehicleMovement script

	[Header("UI References")]
	public GameObject gameOverUI;			//A reference to the UI objects that appears when the game is complete


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

	public void SavePlayerData()
    {
		SaveLoadManager.Save(pd);
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

	public void AddGold(int val)
    {
		pd.gold += val;
    }
}
