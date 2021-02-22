using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelRoadControl : MonoBehaviour
{
    InGameManager _igm;
    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("PlayerShip"))
        {
            _igm.ChangeActiveTerrain();
            gameObject.SetActive(false);
        }
    }
}
