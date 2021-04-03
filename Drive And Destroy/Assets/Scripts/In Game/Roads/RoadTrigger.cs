using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrigger : MonoBehaviour
{
    public bool IsBossRoad;
    public bool IsNextLevelRoad;
    public GameObject BackBlockObstacle;

    private bool _isActiveToTrigger = true;

    private InGameManager _igm;

    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
    }

    private void OnEnable()
    {
        _isActiveToTrigger = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Create New Road and add it to after last added road.
        if (collider.CompareTag("PlayerShip") && _isActiveToTrigger)
        {
            _isActiveToTrigger = false;
            _igm.RoadChangeTrigger();
            if (!IsBossRoad && !IsNextLevelRoad)
            {
                _igm.AddPlayerPoint(10);
            }
            if (BackBlockObstacle != null)
            {
                BackBlockObstacle.SetActive(true);
            }
            if (IsNextLevelRoad)
            {
                // Change Terrain to Next Level
                _igm.ChangeActiveTerrain();
            }
            gameObject.SetActive(false);
        }
    }
}
