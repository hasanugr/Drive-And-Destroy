using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    private GameObject ship;
    private VehicleMovement vehicleMovement;

    // Start is called before the first frame update
    void OnEnable()
    {
        ship = GameObject.FindWithTag("Player");
        vehicleMovement = ship.GetComponent<VehicleMovement>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Turbo Boost
        if (collider.gameObject.CompareTag("PlayerShip") && this.name == "BoostPad")
        {
            vehicleMovement.TurboActivate();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // Destroy the trigger game object
        if (collider.gameObject.CompareTag("PlayerShip") && this.name == "BoostPad")
        {
            Destroy(this.transform.parent.gameObject);
        }
    }
}
