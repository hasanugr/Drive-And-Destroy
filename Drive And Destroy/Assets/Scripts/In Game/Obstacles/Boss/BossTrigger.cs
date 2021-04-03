using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject BossRoad;

    private BossRoadControl _bossRoadControl;
    private bool _isBossSpawned;

    private void Start()
    {
        _bossRoadControl = BossRoad.GetComponent<BossRoadControl>();
    }

    private void OnEnable()
    {
        _isBossSpawned = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip") && !_isBossSpawned)
        {
            _bossRoadControl.BossSpawn();
            _isBossSpawned = true;
        }
    }
}
