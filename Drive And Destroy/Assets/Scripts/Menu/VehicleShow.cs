using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShow : MonoBehaviour
{
    public GameObject[] showVehicles;
    public PlayerData pd;
    public int showingVehicleIndex;

    // Update is called once per frame
    void Update()
    {
        Debug.Log("sv --> " + showingVehicleIndex);
        Debug.Log("pd.sv --> " + pd.selectedVehicleIndex);
        if (showingVehicleIndex != pd.selectedVehicleIndex)
        {
            showingVehicleIndex = pd.selectedVehicleIndex;
            ChangeShowingVehicle();
        }
    }

    private void ChangeShowingVehicle()
    {
        for (int i = 0; i < showVehicles.Length; i++)
        {
            showVehicles[i].SetActive(i == showingVehicleIndex);
        }
    }
}
