using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BossBomb : MonoBehaviour
{
    public float BombDamage = 30.0f;

    private bool _bombActive = true;

    private GameObject ship;
    private VehicleMovement vehicleMovement;
    private AudioManager _audioManager;

    BossAI _bossAI;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.FindWithTag("Player");
        vehicleMovement = ship.GetComponent<VehicleMovement>();
        _audioManager = FindObjectOfType<AudioManager>();
        _bossAI = FindObjectOfType<BossAI>();
    }

    private void OnEnable()
    {
        _bombActive = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            Vector3 pos = this.gameObject.transform.position;
            Quaternion rot = this.gameObject.transform.rotation;

            // Take passive the bullet when touch any collision
            gameObject.SetActive(false);

            _audioManager.PlayOneShot("BossBombExp");
            // Get the contact point and create bullet hole at contact point
            
            if (_bombActive)
            {
                print("BossBomb Triggered!");
                _bombActive = false;
                _bossAI.BombTriggerEffect(pos, rot);
                CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
                vehicleMovement.TakeDamage(BombDamage);
            }

        }
    }
}
