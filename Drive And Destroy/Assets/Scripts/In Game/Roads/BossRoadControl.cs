using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoadControl : MonoBehaviour
{
    public GameObject BossBody;
    public GameObject BossSpawnPoint;
    public GameObject BossTriggerObject;

    private int _reachedLevel = 0;

    public void StartProccess(int reachedLevel, int createdBossRoad)
    {
        _reachedLevel = reachedLevel;

        if (createdBossRoad > 1)
        {
            BossTriggerObject.SetActive(false);
        }
    }

    public void BossSpawn()
    {
        Vector3 pos = BossSpawnPoint.transform.position;
        Quaternion rot = BossSpawnPoint.transform.rotation;

        if (_reachedLevel <= 2) // Mean First Level
        {
            
        }
        else if (_reachedLevel <= 3) // Mean Second Level
        {

        }
        else // And the other levels
        {

        }

        GameObject bossObj = Instantiate(BossBody, pos, rot);
    }

}
