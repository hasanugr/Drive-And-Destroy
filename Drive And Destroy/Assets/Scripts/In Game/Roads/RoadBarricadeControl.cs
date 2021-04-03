using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoadBarricadeControl : MonoBehaviour
{
    public GameObject BoostsHolder;

    [Header("Enivronment")]
    public MeshRenderer RoadMeshRenderer;
    public GameObject[] Environments;
    public Material[] RoadMaterials;

    private Material[] _tempMaterials;

    [Header("Barricades")]
    public GameObject BackBlock;
    public GameObject DeleteRoadTrigger;
    public GameObject EntryVirus;
    public GameObject FirstQuarterFixed;
    public GameObject FirstQuarterMoving;
    public GameObject[] HalfTripleFixed;
    public GameObject HalfDoubleGate;
    public GameObject LastQuarterFixed;
    public GameObject LastQuarterMoving;
    public GameObject ExitSupriseUnbreakable;
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
        if (_igm.transform.position != this.transform.position)
        {
            // Will added baricades if this road not the first road.
            AddBarricades();
            BoostsHolder.SetActive(true);
            BackBlock.SetActive(false);
        }else
        {
            BackBlock.SetActive(true);
        }
        DeleteRoadTrigger.SetActive(true);
    }

    private void OnEnable()
    {
        if (_igm != null)
        {
            _reachedLevel = _igm.GetReachedLevel();

            EnvironmentAdd();
            RoadMaterialAdd();
            if (_igm.transform.position != this.transform.position)
            {
                // Will added baricades if this road not the first road.
                AddBarricades();
                BoostsHolder.SetActive(true);
                BackBlock.SetActive(false);
            }
            else
            {
                BackBlock.SetActive(true);
            }
            DeleteRoadTrigger.SetActive(true);
        }
    }

    public void AddBarricades()
    {
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
            EntryVirus.transform.Translate(EntryVirus.transform.right * UnityEngine.Random.Range(-6, 7));
            EntryVirus.SetActive(true);
        }
    }

    private void AddUnbreakableSupriseBarricadeToExit(int oddsPercent)
    {
        if (IsShouldWork(oddsPercent))
        {
            ExitSupriseUnbreakable.transform.Translate(ExitSupriseUnbreakable.transform.right * UnityEngine.Random.Range(-6, 7));
            ExitSupriseUnbreakable.SetActive(true);
        }
    }

    private void AddBasicToQuarters()
    {
        FirstQuarterFixed.transform.Translate(FirstQuarterFixed.transform.right * UnityEngine.Random.Range(-6, 7));
        LastQuarterFixed.transform.Translate(FirstQuarterFixed.transform.right * UnityEngine.Random.Range(-6, 7));
        
        AddBarricadeLevelStats(FirstQuarterFixed);
        AddBarricadeLevelStats(LastQuarterFixed);

        FirstQuarterFixed.SetActive(true);
        LastQuarterFixed.SetActive(true);
    }

    private void AddMovingBasicToQuarters()
    {
        AddBarricadeLevelStats(FirstQuarterMoving);
        AddBarricadeLevelStats(LastQuarterMoving);

        FirstQuarterMoving.SetActive(true);
        LastQuarterMoving.SetActive(true);
    }

    private void AddBasicSeriesToMiddle()
    {
        for (int i = 0; i < HalfTripleFixed.Length; i++)
        {
            AddBarricadeLevelStats(HalfTripleFixed[i]);
            HalfTripleFixed[i].SetActive(true);
        }
    }

    private void AddDoubleGateToMiddle()
    {
        HalfDoubleGate.SetActive(true);

        GameObject middleBreackableDoor = HalfDoubleGate.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;

        AddBarricadeLevelStats(middleBreackableDoor);
        middleBreackableDoor.SetActive(true);
        
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

    private void OnDisable()
    {
        GameObject[] resetPositionObjects = new GameObject[] { EntryVirus, FirstQuarterFixed, LastQuarterFixed, ExitSupriseUnbreakable };
        for (var i = 0; i < resetPositionObjects.Length; i++)
        {
            Vector3 objPos = resetPositionObjects[i].transform.position;
            objPos.x = 0;
            resetPositionObjects[i].transform.position = objPos;
        }

        EntryVirus.SetActive(false);
        FirstQuarterFixed.SetActive(false);
        FirstQuarterMoving.SetActive(false);
        HalfDoubleGate.SetActive(false);
        LastQuarterFixed.SetActive(false);
        LastQuarterMoving.SetActive(false);
        ExitSupriseUnbreakable.SetActive(false);

        for (int i = 0; i < HalfTripleFixed.Length; i++)
        {
            HalfTripleFixed[i].SetActive(false);
        }

        for (int i = 0; i < Environments.Length; i++)
        {
            Environments[i].SetActive(false);
        }
    }
}
