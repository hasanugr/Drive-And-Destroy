using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BossBomb : MonoBehaviour
{
    public GameObject BombTriggerEffect;
    public float BombDamage = 30.0f;

    private GameObject ship;
    private VehicleMovement vehicleMovement;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Player");
        vehicleMovement = ship.GetComponent<VehicleMovement>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            Vector3 pos = this.gameObject.transform.position;
            Quaternion rot = this.gameObject.transform.rotation;

            // Destroy the bullet when touch any collision
            Destroy(this.gameObject);

            _audioManager.PlayOneShot("BossBombExp");
            // Get the contact point and create bullet hole at contact point
            if (BombTriggerEffect)
            {
                GameObject bombEffect = Instantiate(BombTriggerEffect, pos, rot);
                // Destroy the bullet hole on surface
                Destroy(bombEffect, 3f);

                CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            }

            vehicleMovement.TakeDamage(BombDamage);

        }
    }
}
