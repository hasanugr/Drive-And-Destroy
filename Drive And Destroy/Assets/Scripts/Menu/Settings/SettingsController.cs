using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject MusicButton;
    public GameObject SoundButton;
    public GameObject VehicleControl;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();

        MusicButton.GetComponent<SwitchHandler>().isOn = gm.pd.music;
        SoundButton.GetComponent<SwitchHandler>().isOn = gm.pd.sound;
        VehicleControl.GetComponent<VCHandler>().SelectedShemeIndex = gm.pd.vehicleControlType;
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicButton.GetComponent<SwitchHandler>().isOn != gm.pd.music)
        {
            gm.pd.music = MusicButton.GetComponent<SwitchHandler>().isOn;
        }
        
        if (SoundButton.GetComponent<SwitchHandler>().isOn != gm.pd.sound)
        {
            gm.pd.sound = SoundButton.GetComponent<SwitchHandler>().isOn;
        }
        
        if (VehicleControl.GetComponent<VCHandler>().SelectedShemeIndex != gm.pd.vehicleControlType)
        {
            gm.pd.vehicleControlType = VehicleControl.GetComponent<VCHandler>().SelectedShemeIndex;
        }
    }
}
