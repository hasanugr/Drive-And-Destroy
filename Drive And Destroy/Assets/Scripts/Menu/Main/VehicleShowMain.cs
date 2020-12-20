using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShowMain : MonoBehaviour
{
    public GameObject[] showVehicles;
    public GameObject spawnPoint;

    private int showingVehicleId;
    private GameObject showingVehicle;

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
        Destroy(showingVehicle);
        Vector3 spawnPosition = spawnPoint.transform.position;
        Quaternion spawnRotation = spawnPoint.transform.rotation;

        showingVehicle = Instantiate(showVehicles[showingVehicleId - 1], spawnPosition, spawnRotation);
        showingVehicle.transform.parent = transform;

        float extraPosition = 0.0f;
        switch (showingVehicleId)
        {
            case 2:
                {
                    extraPosition = 0.5f;
                    break;
                }
        }
        showingVehicle.transform.localPosition += new Vector3(0, 0, extraPosition);
    }
}
