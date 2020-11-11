using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGuns : MonoBehaviour
{
    [Header("Basic Bullet")]
    public GameObject[] Guns;
    public GameObject bulletPrefab;
    public float bulletSpeed = 100.0f;
    public float bulletRate = 0.5f;
    [Range(0.1f, 10.0f)]

    private Transform bulletPosition1;
    private Transform bulletPosition2;
    private bool allowFire = true;
    private bool isDoubleWeapon = false;
    public bool isShootingSameTime = true;
    private bool useFirstWeapon = true;

    private void Start()
    {
        bulletPosition1 = Guns[0].transform;
        if (Guns.Length > 1)
        {
            isDoubleWeapon = true;
            bulletPosition2 = Guns[1].transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0) && allowFire)
        {
            // Trigger the fire function
            if (isDoubleWeapon)
            {
                if (isShootingSameTime)
                {
                    StartCoroutine(FireDoubleWeapon());
                }else
                {
                    StartCoroutine(FireDoubleWeaponWithOrder());
                }
            }else
            {
                StartCoroutine(FireSingleWeapon());
            }
        }
    }
        
    IEnumerator FireSingleWeapon()
    {
        allowFire = false; 
        GameObject bullet = Instantiate(bulletPrefab, bulletPosition1.position, bulletPosition1.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        Destroy(bullet, 2f);

        // Wait for X second
        yield return new WaitForSeconds(bulletRate);

        allowFire = true;
    }
        
    IEnumerator FireDoubleWeapon()
    {
        allowFire = false; 
        GameObject bullet = Instantiate(bulletPrefab, bulletPosition1.position, bulletPosition1.rotation);
        GameObject bullet2 = Instantiate(bulletPrefab, bulletPosition2.position, bulletPosition2.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        bullet2.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        Destroy(bullet, 2f);
        Destroy(bullet2, 2f);

        // Wait for X second
        yield return new WaitForSeconds(bulletRate);

        allowFire = true;
    }
        
    IEnumerator FireDoubleWeaponWithOrder()
    {
        allowFire = false; 

        GameObject bullet;
        if (useFirstWeapon)
        {
            bullet = Instantiate(bulletPrefab, bulletPosition1.position, bulletPosition1.rotation);
        }else
        {
            bullet = Instantiate(bulletPrefab, bulletPosition2.position, bulletPosition2.rotation);
        }

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        useFirstWeapon = !useFirstWeapon;
        Destroy(bullet, 2f);

        // Wait for X second
        yield return new WaitForSeconds(bulletRate / 2);

        allowFire = true;
    }
}
