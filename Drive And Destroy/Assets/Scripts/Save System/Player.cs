using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Player : MonoBehaviour
{
    /*public int[] levels;
    public int gold;
    public int selectedVehicle;
    public ArrayList[] vehicles;*/


    public PlayerData pd;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveLoadManager.Save(pd);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            pd = SaveLoadManager.Load();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            pd.selectedVehicleIndex = pd.selectedVehicleIndex + 1;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            pd.selectedVehicleIndex = pd.selectedVehicleIndex - 1;
        }
    }

    [ContextMenu("Print Data")]
    void PrintData()
    {
        Debug.Log(SaveLoadManager.GetFullPath());
        Array.Resize(ref pd.levels, pd.levels.Length + 1);
    }

}
