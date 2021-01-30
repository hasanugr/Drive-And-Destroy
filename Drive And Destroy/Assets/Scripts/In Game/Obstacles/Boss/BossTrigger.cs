using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject BossRoad;

    private BossRoadControl _bossRoadControl;

    private void Start()
    {
        _bossRoadControl = BossRoad.GetComponent<BossRoadControl>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            _bossRoadControl.BossSpawn();
        }
    }
}
