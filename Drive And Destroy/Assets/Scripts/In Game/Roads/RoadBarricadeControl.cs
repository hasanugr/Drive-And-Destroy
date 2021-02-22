using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoadBarricadeControl : MonoBehaviour
{
    public int RoadMaterialTest = 0;
    private int _activeRMT = 0;
    [Header("Enivronment")]
    public MeshRenderer RoadMeshRenderer;
    public GameObject[] Environments;
    public Material[] RoadMaterials;

    private Material[] _tempMaterials;

    [Header("Barricades")]
    public GameObject[] BarricadePoints;
    public GameObject[] Barricades;
    public Transform BarricadesHolderTransform;

    private int _reachedLevel;

    private InGameManager _igm;
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

    private void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _reachedLevel = _igm.GetReachedLevel();

        EnvironmentAdd();
        RoadMaterialAdd();
    }

    public void AddBarricades(int reachedLevel)
    {
       BarricadeTypes barricadeType = GetRandomBarricade();
       _reachedLevel = reachedLevel;

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
        switch (_reachedLevel)
        {
            case 1:
                basicBarricade = Barricades[0];
                break;
            case 2:
                basicBarricade = Barricades[2];
                break;
            case 3:
                basicBarricade = Barricades[4];
                break;
            default:
                basicBarricade = Barricades[4];
                break;
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

        switch (_reachedLevel)
        {
            case 1:
                movingBarricade = Barricades[1];
                break;
            case 2:
                movingBarricade = Barricades[3];
                break;
            case 3:
                movingBarricade = Barricades[5];
                break;
            default:
                movingBarricade = Barricades[5];
                break;
        }

        GameObject firstQuarterBarricade = Instantiate(movingBarricade, BarricadePoints[1].transform.position, BarricadePoints[1].transform.rotation);
        GameObject lastQuarterBarricade = Instantiate(movingBarricade, BarricadePoints[3].transform.position, BarricadePoints[3].transform.rotation);

        firstQuarterBarricade.transform.parent = BarricadesHolderTransform;
        lastQuarterBarricade.transform.parent = BarricadesHolderTransform;
    }

    private void AddBasicSeriesToMiddle()
    {
        GameObject basicBarricadeSeries;

        switch (_reachedLevel)
        {
            case 1:
                basicBarricadeSeries = Barricades[0];
                break;
            case 2:
                basicBarricadeSeries = Barricades[2];
                break;
            case 3:
                basicBarricadeSeries = Barricades[4];
                break;
            default:
                basicBarricadeSeries = Barricades[4];
                break;
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

        switch (_reachedLevel)
        {
            case 1:
                middleBreackableDoorMat.color = Color.green;
                middleBreackableDoorConf.SpecialColor = Color.green;
                middleBreackableDoorConf.FullHealth = 40;
                break;
            case 2:
                middleBreackableDoorMat.color = Color.yellow;
                middleBreackableDoorConf.SpecialColor = Color.yellow;
                middleBreackableDoorConf.FullHealth = 70;
                break;
            case 3:
                middleBreackableDoorMat.color = Color.red;
                middleBreackableDoorConf.SpecialColor = Color.red;
                middleBreackableDoorConf.FullHealth = 100;
                break;
            default:
                middleBreackableDoorMat.color = Color.red;
                middleBreackableDoorConf.SpecialColor = Color.red;
                middleBreackableDoorConf.FullHealth = 130;
                break;
        }
        
    }

    private void EnvironmentAdd()
    {
        switch (_reachedLevel)
        {
            case 1:
                Environments[0].SetActive(true);
                break;
            case 2:
                Environments[1].SetActive(true);
                break;
            case 3:
                Environments[2].SetActive(true);
                break;
            default:
                Environments[3].SetActive(true);
                break;
        }
    }

    private void RoadMaterialAdd()
    {
        _tempMaterials = RoadMeshRenderer.materials;
        switch (_reachedLevel)
        {
            case 1:
                _tempMaterials[0] = RoadMaterials[0];
                break;
            case 2:
                _tempMaterials[0] = RoadMaterials[1];
                break;
            case 3:
                _tempMaterials[0] = RoadMaterials[2];
                break;
            default:
                _tempMaterials[0] = RoadMaterials[3];
                break;
        }
        RoadMeshRenderer.materials = _tempMaterials;
    }
}
