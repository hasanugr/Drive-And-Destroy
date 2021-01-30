using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    public float LaserDamagePerSecond = 30.0f;

    private GameObject _ship;
    private VehicleMovement _vehicleMovement;
    private float _t = 0;
    private bool _isPlayerInLaser = false;

    // Start is called before the first frame update
    void Start()
    {
        _ship = GameObject.FindWithTag("Player");
        _vehicleMovement = _ship.GetComponent<VehicleMovement>();
    }

    private void OnEnable()
    {
        _isPlayerInLaser = false;
    }

    // Update is called once per frame
    void Update()
    {
        float dur = 1f / 10f;
        _t += Time.deltaTime;
        int maxLimitCounter = 3;
        while (_t > dur && maxLimitCounter > 0)
        {
            _t -= dur;
            maxLimitCounter--;
            this.HitDamageConstantly();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            _isPlayerInLaser = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            _isPlayerInLaser = false;
        }
    }

    private void HitDamageConstantly()
    {
        if (_isPlayerInLaser)
        {
            _vehicleMovement.TakeDamage(LaserDamagePerSecond/10);
        }
    }
}
