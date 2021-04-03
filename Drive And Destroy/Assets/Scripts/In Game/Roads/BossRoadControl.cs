using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoadControl : MonoBehaviour
{
    public GameObject BackBlock;
    public GameObject DeleteRoadTrigger;

    [Header("Enivronment")]
    public MeshRenderer RoadMeshRenderer;
    public GameObject[] Environments;
    public Material[] RoadMaterials;

    private Material[] _tempMaterials;

    [Header("Boss")]
    public GameObject BossBody;
    public GameObject BossSpawnPoint;
    public GameObject BossTriggerObject;

    private int _reachedLevel = 0;

    public void StartProccess(int reachedLevel, int createdBossRoad)
    {
        _reachedLevel = reachedLevel;
        EnvironmentAdd();
        RoadMaterialAdd();

        if (createdBossRoad > 1)
        {
        BossTriggerObject.SetActive(false);
        }

        BackBlock.SetActive(false);
        DeleteRoadTrigger.SetActive(true);
    }

    public void BossSpawn()
    {
        Vector3 pos = BossSpawnPoint.transform.position;
        Quaternion rot = BossSpawnPoint.transform.rotation;

        Instantiate(BossBody, pos, rot);
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

    private void OnDisable()
    {
        for (int i = 0; i < Environments.Length; i++)
        {
            Environments[i].SetActive(false);
        }
        BossTriggerObject.SetActive(true);
    }

}
