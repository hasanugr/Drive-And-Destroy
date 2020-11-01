using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShow : MonoBehaviour
{
    public GameObject[] showVehicles;
    //public PlayerData pd;
    public int showingVehicleId;

    GameManager gm;
    private void Start()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (showingVehicleId != gm.pd.selectedVehicleId)
        {
            showingVehicleId = gm.pd.selectedVehicleId;
            ChangeShowingVehicle();
        }
    }

    private void ChangeShowingVehicle()
    {
        for (int i = 0; i < showVehicles.Length; i++)
        {
            if (showVehicles[i].name == showingVehicleId.ToString())
            {
                showVehicles[i].SetActive(true);
            }else
            {
                showVehicles[i].SetActive(false);
            }
        }
    }
}
