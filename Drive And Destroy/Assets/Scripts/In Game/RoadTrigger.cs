using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrigger : MonoBehaviour
{
    private InGameManager igm;

    // Start is called before the first frame update
    void Start()
    {
        igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Suprise Barricade Animation. (Fall to road)
        if (collider.tag == "PlayerShip")
        {
            igm.RoadChangeTrigger();   
        }
    }
}
