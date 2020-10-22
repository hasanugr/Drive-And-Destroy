using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int[] levels;
    public int gold;
    public int selectedVehicleIndex;
    public int vehicleNumber = 3;

   /* public int[] vehicleStatus1;
    public int[] vehicleColors1;

    public int[] vehicleStatus2;
    public int[] vehicleColors2;

    public int[] vehicleStatus3;
    public int[] vehicleColors3;*/

    //public List<int[]> vehicleStatus;
    //public List<List<int[]>> vehicleList;


    public void AddDefaultValues()
    {
        // Create levels array with 1 size. There is only variable value is will be 0.
        levels = new int[1];
        // Starting vehicle index.
        selectedVehicleIndex = 0;
        // Starting value of gold.
        gold = 999;


        List<List<int[]>> vehicleList = new List<List<int[]>>();
        for (int i = 0; i < vehicleNumber; i++)
        {
            vehicleList.Add(new List<int[]>(AddPassiveVehicleValues(i)));
        }
/*
        // Vehicle status and color status of vehicle
        vehicleStatus1 = new int[3];
        vehicleColors1 = new int[3];
        AddPassiveVehicleValues(vehicleStatus1, vehicleColors1, true);

        // Vehicle status and color status of vehicle
        vehicleStatus2 = new int[3];
        vehicleColors2 = new int[3];
        AddPassiveVehicleValues(vehicleStatus2, vehicleColors2, false);

        // Vehicle status and color status of vehicle
        vehicleStatus3 = new int[3];
        vehicleColors3 = new int[3];
        AddPassiveVehicleValues(vehicleStatus3, vehicleColors3, false);*/

        Array.Resize(ref levels, 5);
    }

    private List<int[]> AddPassiveVehicleValues(int loopCount)
    {
        List<int[]> tempList = new List<int[]>();

        int[] vehiclePrefs = new int[2];
        vehiclePrefs[0] = loopCount == 0 ? 1 : 0; // isActive
        vehiclePrefs[1] = 0; // selectedColorIndex

        int[] vehicleColors = new int[3];
        vehicleColors[0] = 1; // First Color
        vehicleColors[1] = 0; // Second Color
        vehicleColors[2] = 0; // Thirth Color

        tempList.Add(vehiclePrefs.ToArray<int>());
        tempList.Add(vehicleColors.ToArray<int>());

        return tempList;
    }

  /*  private List<int[]> AddPassiveVehicleValues(int[] vehicleStatus, int[] vehicleColors, bool isFirstVehicle)
    {
        List<int[]> tempList = new List<int[]>();

        vehicleStatus[0] = isFirstVehicle ? 1 : 0; // isActive
        vehicleStatus[1] = isFirstVehicle ? 1 : 0; // isSelected
        vehicleStatus[2] = 0; // selectedColorIndex

        vehicleColors[0] = 1; // First Color
        vehicleColors[1] = 0; // Second Color
        vehicleColors[2] = 0; // Thirth Color

        tempList.Add(vehicleStatus.ToArray<int>());
        tempList.Add(vehicleColors.ToArray<int>());

        return tempList;
    }*/
}
