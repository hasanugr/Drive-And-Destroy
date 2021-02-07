using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class VehicleShowVS : MonoBehaviour
{
    public GameObject[] showVehicles;
    public GameObject spawnPoint;

    public GameObject NextButton;
    public GameObject PrevButton;

    public GameObject BuyButton;
    public GameObject SelectButton;

    public GameObject Lights;

    public int showingVehicleId;
    private GameObject showingVehicle;
    private int[] vehiclePrices = { 0, 1000, 1500, 2000 };

    GameManager gm;
    PopupController _popupController;
    private void OnEnable()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        showingVehicleId = gm.pd.selectedVehicleId;

        _popupController = GameObject.Find("Popup Controller").GetComponent<PopupController>();

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
        gm.ChangeActiveVehicle(showingVehicleId);
        SelectBuyControlAndShowButtons();
    }

    public void BuyVehicle()
    {
        if (gm.pd.gold >= vehiclePrices[showingVehicleId - 1])
        {
            gm.pd.ActivateVehicle(showingVehicleId);
            SelectVehicle();

            gm.pd.gold -= vehiclePrices[showingVehicleId - 1];
        }
        else
        {
            _popupController.OpenPopup("EarnGold");
            Debug.LogError("Paranız Yetersiz.!!");
        }
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
        int showingVehicleChildCount = showingVehicle.transform.childCount;
        //GameObject showingVehicleVFX = showingVehicle.transform.GetChild(showingVehicleChildCount - 1).gameObject;

        if (showingVehicleId == gm.pd.selectedVehicleId)
        {
            BuyButton.SetActive(false);
            SelectButton.SetActive(false);
            Lights.SetActive(true);
            //showingVehicleVFX.SetActive(true);
        }
        else if (gm.pd.IsVehicleActive(showingVehicleId))
        {
            BuyButton.SetActive(false);
            SelectButton.SetActive(true);
            Lights.SetActive(true);
            //showingVehicleVFX.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            SelectButton.SetActive(false);
            Lights.SetActive(false);
            //showingVehicleVFX.SetActive(false);

            TextMeshProUGUI ButtonPriceText = BuyButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            ButtonPriceText.text = vehiclePrices[showingVehicleId - 1].ToString();
        }
    }

    private void OnDisable()
    {
        Destroy(showingVehicle);
        Lights.SetActive(true);
    }
}
