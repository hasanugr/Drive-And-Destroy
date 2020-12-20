using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGuns : MonoBehaviour
{
    [Header("Basic Bullet")]
    public GameObject[] Guns;
    public GameObject bulletPrefab;
    public GameObject bulletShootEffect;
    public float bulletShootDelay = 0;
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

    private void FireEffectWeapon (Transform bulletPosition)
    {
        GameObject shootEffect = Instantiate(bulletShootEffect);

        shootEffect.transform.parent = bulletPosition;
        shootEffect.transform.localPosition = new Vector3(0, 0, 0);

        Destroy(shootEffect, 1f);
    }

    IEnumerator FireSingleWeapon()
    {
        allowFire = false;
        if (bulletShootEffect != null)
        {
            FireEffectWeapon(bulletPosition1);
            // Wait for X second
            yield return new WaitForSeconds(bulletShootDelay);
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletPosition1.position, bulletPosition1.rotation);
        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);

        Destroy(bullet, 2f);

        // Wait for X second
        yield return new WaitForSeconds(bulletRate);
        
        allowFire = true;
    }
        
    IEnumerator FireDoubleWeapon()
    {
        // Shoot effect
        /*StartCoroutine(FireEffectWeapon(bulletPosition1));
        StartCoroutine(FireEffectWeapon(bulletPosition2));*/

        // Then bullet create
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
            // Shoot effect
            /*StartCoroutine(FireEffectWeapon(bulletPosition1));*/

            // Then bullet create
            bullet = Instantiate(bulletPrefab, bulletPosition1.position, bulletPosition1.rotation);
        }else
        {
            // Shoot effect
/*            StartCoroutine(FireEffectWeapon(bulletPosition2));
*/
            // Then bullet create
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
