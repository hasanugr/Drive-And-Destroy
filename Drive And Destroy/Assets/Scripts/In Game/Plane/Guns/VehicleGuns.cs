//using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class VehicleGuns : MonoBehaviour
{
    [Header("Basic Bullet")]
    public GameObject[] Guns;
    public GameObject bulletPrefab;
    public GameObject bulletShootEffect;
    public float bulletShootDelay = 0;
    public float bulletSpeed = 100.0f;
    public float bulletRate = 0.5f;

    private Transform bulletPosition1;
    private Transform bulletPosition2;
    private bool allowFire = true;
    private bool isDoubleWeapon = false;
    public bool isShootingSameTime = true;
    private bool useFirstWeapon = true;
    private string _gunFireSoundName;

    ObjectPooler _bulletPool;
    AudioManager _audioManager;

    private void Start()
    {
        // Fix Fire button if it's stuck
        CrossPlatformInputManager.SetButtonUp("Fire");
        _audioManager = FindObjectOfType<AudioManager>();
        string selectedVehicleId = GameManager.instance.selectedVehicle.name;
        _gunFireSoundName = "GunFire" + selectedVehicleId;

        bulletPosition1 = Guns[0].transform;
        if (Guns.Length > 1)
        {
            isDoubleWeapon = true;
            bulletPosition2 = Guns[1].transform;
        }

        _bulletPool = new ObjectPooler(bulletPrefab);
        _bulletPool.FillThePool(15);

        /*if (bulletShootEffect)
        {
            ObjectPooler bulletShootEffectPool = new ObjectPooler(bulletShootEffect);
            bulletShootEffectPool.FillThePool(5);
        }*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (Input.GetKey(KeyCode.Mouse0) && allowFire)
        if (CrossPlatformInputManager.GetButton("Fire") && allowFire)
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
            _audioManager.PlayOneShot(_gunFireSoundName);
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

        /*GameObject bullet = bulletPool.GetObjectFromPool();
        bullet.transform.position = bulletPosition1.position;
        bullet.transform.rotation = bulletPosition1.rotation;*/
        GameObject bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);

        // Destroy(bullet, 2f);
        StartCoroutine(SendToPoolObjectTimout(bullet, 2f));

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
        /*GameObject bullet = bulletPool.GetObjectFromPool();
        bullet.transform.position = bulletPosition1.position;
        bullet.transform.rotation = bulletPosition1.rotation;
        GameObject bullet2 = bulletPool.GetObjectFromPool();
        bullet2.transform.position = bulletPosition2.position;
        bullet2.transform.rotation = bulletPosition2.rotation;*/
        GameObject bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);
        GameObject bullet2 = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition2.position, bulletPosition2.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        bullet2.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);

        StartCoroutine(SendToPoolObjectTimout(bullet, 2f));
        StartCoroutine(SendToPoolObjectTimout(bullet2, 2f));

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
            /*bullet = bulletPool.GetObjectFromPool();
            bullet.transform.position = bulletPosition1.position;
            bullet.transform.rotation = bulletPosition1.rotation;*/
            bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);
        }
        else
        {
            // Shoot effect
            /*            StartCoroutine(FireEffectWeapon(bulletPosition2));
            */
            // Then bullet create
            /*bullet = bulletPool.GetObjectFromPool();
            bullet.transform.position = bulletPosition2.position;
            bullet.transform.rotation = bulletPosition2.rotation;*/
            bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition2.position, bulletPosition2.rotation);
        }

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        useFirstWeapon = !useFirstWeapon;

        StartCoroutine(SendToPoolObjectTimout(bullet, 2f));

        // Wait for X second
        yield return new WaitForSeconds(bulletRate / 2);

        allowFire = true;
    }

    private IEnumerator SendToPoolObjectTimout(GameObject bulletObject, float time)
    {
        // Wait for X second
        yield return new WaitForSeconds(time);

        _bulletPool.SendObjectToPool(bulletObject);
    }
}
