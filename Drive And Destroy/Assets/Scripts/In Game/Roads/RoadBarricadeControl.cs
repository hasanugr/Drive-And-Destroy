using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoadBarricadeControl : MonoBehaviour
{
    public GameObject[] BarricadePoints;
    public GameObject[] Barricades;
    public Transform BarricadesHolderTransform;

    private int _playerScore;

    /*
     * BasicToQuarters -> Adding basic barricades to first and last quarters of road.
     * BasicSeriesToMiddle -> Adding 5 basic barricades to middle of road as series.
     * DoubleGateToMiddle -> Adding double gate barricade to middle of road.
     * PoisonToMiddle -> Adding poison portal to middle of road. This poison effected randomly as half blind view, reverse control, lost of speed, etc..
     */
    private enum BarricadeTypes
    {
        BasicToQuarters,
        MovingBasicToQuarters,
        BasicSeriesToMiddle,
        DoubleGateToMiddle,
    }

    public void AddBarricades(int playerScore)
    {
        _playerScore = playerScore;
        BarricadeTypes barricadeType = GetRandomBarricade();
       switch (barricadeType)
        {
            case BarricadeTypes.BasicToQuarters:
                AddBasicToQuarters();
                AddVirusEffectToEntry(25);
                AddUnbreakableSupriseBarricadeToExit(25);
                break;
            case BarricadeTypes.MovingBasicToQuarters:
                AddMovingBasicToQuarters();
                AddVirusEffectToEntry(25);
                AddUnbreakableSupriseBarricadeToExit(25);
                break;
            case BarricadeTypes.BasicSeriesToMiddle:
                AddBasicSeriesToMiddle();
                AddVirusEffectToEntry(25);
                AddUnbreakableSupriseBarricadeToExit(25);
                break;
            case BarricadeTypes.DoubleGateToMiddle:
                AddDoubleGateToMiddle();
                AddVirusEffectToEntry(25);
                AddUnbreakableSupriseBarricadeToExit(25);
                break;
            default:
                Debug.LogWarning("Barricade Type can not selected.!!");
                break;
        }
    }

    private BarricadeTypes GetRandomBarricade()
    {
        var values = Enum.GetValues(typeof(BarricadeTypes));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (BarricadeTypes)values.GetValue(random);
    }

    private bool IsShouldWork(int percent)
    {
        int random = UnityEngine.Random.Range(0, 101);
        bool isIt = random <= percent;
        return isIt;
    }

    private void AddVirusEffectToEntry(int oddsPercent)
    {
        if (IsShouldWork(oddsPercent))
        {
            GameObject virusEffect = Barricades[8];

            BarricadePoints[0].transform.Translate(BarricadePoints[0].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
            GameObject exitBarricade = Instantiate(virusEffect, BarricadePoints[0].transform.position, BarricadePoints[0].transform.rotation);
            exitBarricade.transform.parent = BarricadesHolderTransform;
        }
    }

    private void AddUnbreakableSupriseBarricadeToExit(int oddsPercent)
    {
        if (IsShouldWork(oddsPercent))
        {
            GameObject unbreakableBarricade = Barricades[6];
            
            BarricadePoints[4].transform.Translate(BarricadePoints[4].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
            GameObject exitBarricade = Instantiate(unbreakableBarricade, BarricadePoints[4].transform.position, BarricadePoints[4].transform.rotation);
            exitBarricade.transform.parent = BarricadesHolderTransform;
        }
    }

    private void AddBasicToQuarters()
    {
        GameObject basicBarricade;

        if (_playerScore < VariableController.FirstLevelEndPoint)
        {
            basicBarricade = Barricades[0];
        }else if (_playerScore < VariableController.SecondLevelEndPoint)
        {
            basicBarricade = Barricades[2];
        }else
        {
            basicBarricade = Barricades[4];
        }

        BarricadePoints[1].transform.Translate(BarricadePoints[1].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
        BarricadePoints[3].transform.Translate(BarricadePoints[3].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
        GameObject firstQuarterBarricade = Instantiate(basicBarricade, BarricadePoints[1].transform.position, BarricadePoints[1].transform.rotation);
        GameObject lastQuarterBarricade = Instantiate(basicBarricade, BarricadePoints[3].transform.position, BarricadePoints[3].transform.rotation);

        firstQuarterBarricade.transform.parent = BarricadesHolderTransform;
        lastQuarterBarricade.transform.parent = BarricadesHolderTransform;
    }

    private void AddMovingBasicToQuarters()
    {
        GameObject movingBarricade;

        if (_playerScore < VariableController.FirstLevelEndPoint)
        {
            movingBarricade = Barricades[1];
        }
        else if (_playerScore < VariableController.SecondLevelEndPoint)
        {
            movingBarricade = Barricades[3];
        }
        else
        {
            movingBarricade = Barricades[5];
        }

        GameObject firstQuarterBarricade = Instantiate(movingBarricade, BarricadePoints[1].transform.position, BarricadePoints[1].transform.rotation);
        GameObject lastQuarterBarricade = Instantiate(movingBarricade, BarricadePoints[3].transform.position, BarricadePoints[3].transform.rotation);

        firstQuarterBarricade.transform.parent = BarricadesHolderTransform;
        lastQuarterBarricade.transform.parent = BarricadesHolderTransform;
    }

    private void AddBasicSeriesToMiddle()
    {
        GameObject basicBarricadeSeries;

        if (_playerScore < VariableController.FirstLevelEndPoint)
        {
            basicBarricadeSeries = Barricades[0];
        }
        else if (_playerScore < VariableController.SecondLevelEndPoint)
        {
            basicBarricadeSeries = Barricades[2];
        }
        else
        {
            basicBarricadeSeries = Barricades[4];
        }

        GameObject firstQuarterBarricade = Instantiate(basicBarricadeSeries, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.forward * 3, Space.World);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.right * 6, Space.World);
        GameObject secondQuarterBarricade = Instantiate(basicBarricadeSeries, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.right * -12, Space.World);
        GameObject thirthQuarterBarricade = Instantiate(basicBarricadeSeries, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation);

        firstQuarterBarricade.transform.parent = BarricadesHolderTransform;
        secondQuarterBarricade.transform.parent = BarricadesHolderTransform;
        thirthQuarterBarricade.transform.parent = BarricadesHolderTransform;
    }

    private void AddDoubleGateToMiddle()
    {
        GameObject doubleGateBarricade = Barricades[7];

        GameObject middleBarricade = Instantiate(doubleGateBarricade, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation);
        middleBarricade.transform.parent = BarricadesHolderTransform;

        GameObject middleBreackableDoor = middleBarricade.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        BlockBarricade middleBreackableDoorConf = middleBreackableDoor.GetComponent<BlockBarricade>();
        Material middleBreackableDoorMat = middleBreackableDoor.GetComponent<Renderer>().material;

        if (_playerScore < VariableController.FirstLevelEndPoint)
        {
            middleBreackableDoorMat.color = Color.green;
            middleBreackableDoorConf.SpecialColor = Color.green;
            middleBreackableDoorConf.FullHealth = 40;
        }
        else if (_playerScore < VariableController.SecondLevelEndPoint)
        {
            middleBreackableDoorMat.color = Color.yellow;
            middleBreackableDoorConf.SpecialColor = Color.yellow;
            middleBreackableDoorConf.FullHealth = 70;
        }
        else
        {
            middleBreackableDoorMat.color = Color.red;
            middleBreackableDoorConf.SpecialColor = Color.red;
            middleBreackableDoorConf.FullHealth = 100;
        }
    }

}
