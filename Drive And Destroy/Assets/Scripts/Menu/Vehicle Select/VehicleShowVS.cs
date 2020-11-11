using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShowVS : MonoBehaviour
{
    public GameObject[] showVehicles;
    public GameObject spawnPoint;

    public GameObject NextButton;
    public GameObject PrevButton;

    public GameObject BuyButton;
    public GameObject SelectButton;

    public int showingVehicleId;
    private GameObject showingVehicle;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        showingVehicleId = gm.pd.selectedVehicleId;
        ChangeShowingVehicle();
        CheckActiveButtons();
        SelectBuyControlAndShowButtons();
    }

    public void NextVehicle()
    {
        showingVehicleId += 1;
        ChangeShowingVehicle();
        CheckActiveButtons();
        SelectBuyControlAndShowButtons();
    }

    public void PrevVehicle()
    {
        showingVehicleId -= 1;
        ChangeShowingVehicle();
        CheckActiveButtons();
        SelectBuyControlAndShowButtons();
    }

    public void SelectVehicle()
    {
        gm.pd.selectedVehicleId = showingVehicleId;
        SelectBuyControlAndShowButtons();
    }

    public void BuyVehicle()
    {
        gm.pd.ActivateVehicle(showingVehicleId);
        SelectVehicle();
    }

    public void ChangeColorVehicle()
    {
        // Change Color of Vehicle
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
            case 1:
                {
                    extraPosition = 0.3f;
                    break;
                }
            case 2:
                {
                    extraPosition = 0.7f;
                    break;
                }
        }
        showingVehicle.transform.localPosition += new Vector3(0, 0, extraPosition);
    }

    private void CheckActiveButtons()
    {
        if (showingVehicleId >= showVehicles.Length)
        {
            NextButton.SetActive(false);
            PrevButton.SetActive(true);
        }
        else if (showingVehicleId <= 1)
        {
            NextButton.SetActive(true);
            PrevButton.SetActive(false);
        }
        else
        {
            NextButton.SetActive(true);
            PrevButton.SetActive(true);
        }
    }

    private void SelectBuyControlAndShowButtons()
    {
        if (showingVehicleId == gm.pd.selectedVehicleId)
        {
            BuyButton.SetActive(false);
            SelectButton.SetActive(false);
        }
        else if (gm.pd.IsVehicleActive(showingVehicleId))
        {
            BuyButton.SetActive(false);
            SelectButton.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            SelectButton.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Destroy(showingVehicle);
    }
}
