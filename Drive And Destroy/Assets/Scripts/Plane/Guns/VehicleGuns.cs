using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGuns : MonoBehaviour
{
    [Header("Basic Bullet")]
    public Transform bulletPosition;
    public GameObject bulletPrefab;
    public float bulletSpeed = 100.0f;
    public float bulletRate = 0.5f;
    [Range(0.1f, 10.0f)]

    private bool allowFire = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0) && allowFire)
        {
            // Trigger the fire function
            StartCoroutine(Fire());

        }
    }
        
    IEnumerator Fire()
    {
        allowFire = false;

        GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, bulletPosition.rotation);
        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        Destroy(bullet, 2f);

        // Wait for X second
        yield return new WaitForSeconds(bulletRate);

        allowFire = true;
    }
}
