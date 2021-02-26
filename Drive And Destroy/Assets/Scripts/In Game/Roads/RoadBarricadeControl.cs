using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoadBarricadeControl : MonoBehaviour
{
    [Header("Enivronment")]
    public MeshRenderer RoadMeshRenderer;
    public GameObject[] Environments;
    public Material[] RoadMaterials;

    private Material[] _tempMaterials;

    [Header("Barricades")]
    public GameObject[] BarricadePoints;
    public GameObject[] Barricades;
    public Transform BarricadesHolderTransform;

    public GameObject GlassBaricade;
    public GameObject MovingBarricade;
    public GameObject UnbreakableSupriseBarricade;
    public GameObject DoubleGateBarricade;
    public GameObject VirusEffect;
    public Color BarricadeColor1;
    public Color BarricadeColor2;
    public Color BarricadeColor3;
    public Color BarricadeColor4;

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
            BarricadePoints[0].transform.Translate(BarricadePoints[0].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
            Instantiate(VirusEffect, BarricadePoints[0].transform.position, BarricadePoints[0].transform.rotation, BarricadesHolderTransform);
        }
    }

    private void AddUnbreakableSupriseBarricadeToExit(int oddsPercent)
    {
        if (IsShouldWork(oddsPercent))
        {
            BarricadePoints[4].transform.Translate(BarricadePoints[4].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
            Instantiate(UnbreakableSupriseBarricade, BarricadePoints[4].transform.position, BarricadePoints[4].transform.rotation, BarricadesHolderTransform);
        }
    }

    private void AddBasicToQuarters()
    {
        BarricadePoints[1].transform.Translate(BarricadePoints[1].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
        BarricadePoints[3].transform.Translate(BarricadePoints[3].transform.right * UnityEngine.Random.Range(-6, 7), Space.World);
        GameObject firstQuarterBarricade = Instantiate(GlassBaricade, BarricadePoints[1].transform.position, BarricadePoints[1].transform.rotation, BarricadesHolderTransform);
        GameObject lastQuarterBarricade = Instantiate(GlassBaricade, BarricadePoints[3].transform.position, BarricadePoints[3].transform.rotation, BarricadesHolderTransform);

        AddBarricadeLevelStats(firstQuarterBarricade);
        AddBarricadeLevelStats(lastQuarterBarricade);
    }

    private void AddMovingBasicToQuarters()
    {
        GameObject firstQuarterBarricade = Instantiate(MovingBarricade, BarricadePoints[1].transform.position, BarricadePoints[1].transform.rotation, BarricadesHolderTransform);
        GameObject lastQuarterBarricade = Instantiate(MovingBarricade, BarricadePoints[3].transform.position, BarricadePoints[3].transform.rotation, BarricadesHolderTransform);

        AddBarricadeLevelStats(firstQuarterBarricade);
        AddBarricadeLevelStats(lastQuarterBarricade);
    }

    private void AddBasicSeriesToMiddle()
    {
        GameObject firstQuarterBarricade = Instantiate(GlassBaricade, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation, BarricadesHolderTransform);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.forward * 3, Space.World);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.right * 6, Space.World);
        GameObject secondQuarterBarricade = Instantiate(GlassBaricade, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation, BarricadesHolderTransform);
        BarricadePoints[2].transform.Translate(BarricadePoints[2].transform.right * -12, Space.World);
        GameObject thirthQuarterBarricade = Instantiate(GlassBaricade, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation, BarricadesHolderTransform);

        AddBarricadeLevelStats(firstQuarterBarricade);
        AddBarricadeLevelStats(secondQuarterBarricade);
        AddBarricadeLevelStats(thirthQuarterBarricade);
    }

    private void AddDoubleGateToMiddle()
    {
        GameObject middleBarricade = Instantiate(DoubleGateBarricade, BarricadePoints[2].transform.position, BarricadePoints[2].transform.rotation, BarricadesHolderTransform);

        GameObject middleBreackableDoor = middleBarricade.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        AddBarricadeLevelStats(middleBreackableDoor);
        
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

    private void AddBarricadeLevelStats(GameObject barricadeParent)
    {
        Transform barricade = barricadeParent.transform.GetChild(0);
        Material barricadeMaterial = barricade.GetComponent<Renderer>().material;
        BlockBarricade barricadeBlockBarricade = barricade.GetComponent<BlockBarricade>();

        switch (_reachedLevel)
        {
            case 1:
                barricadeMaterial.color = BarricadeColor1;
                barricadeBlockBarricade.BarricadeLevel = _reachedLevel;
                barricadeBlockBarricade.FullHealth = 20;
                break;
            case 2:
                barricadeMaterial.color = BarricadeColor2;
                barricadeBlockBarricade.BarricadeLevel = _reachedLevel;
                barricadeBlockBarricade.FullHealth = 40;
                break;
            case 3:
                barricadeMaterial.color = BarricadeColor3;
                barricadeBlockBarricade.BarricadeLevel = _reachedLevel;
                barricadeBlockBarricade.FullHealth = 60;
                break;
            case 4:
                barricadeMaterial.color = BarricadeColor4;
                barricadeBlockBarricade.BarricadeLevel = _reachedLevel;
                barricadeBlockBarricade.FullHealth = 80;
                break;
            default:
                barricadeMaterial.color = BarricadeColor4;
                barricadeBlockBarricade.BarricadeLevel = _reachedLevel;
                barricadeBlockBarricade.FullHealth = 80;
                break;
        }
    }
}
