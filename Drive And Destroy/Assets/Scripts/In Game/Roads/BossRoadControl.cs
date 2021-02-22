using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoadControl : MonoBehaviour
{
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
    }

    public void BossSpawn()
    {
        Vector3 pos = BossSpawnPoint.transform.position;
        Quaternion rot = BossSpawnPoint.transform.rotation;

        /*switch (_reachedLevel)
        {
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            default:
                
                break;
        }*/

        GameObject bossObj = Instantiate(BossBody, pos, rot);
        InGameManager igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        igm.IsBossAlive = true;
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
