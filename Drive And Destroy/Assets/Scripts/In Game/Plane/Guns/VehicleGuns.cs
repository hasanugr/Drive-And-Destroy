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
    public GameObject bulletTriggerEffect;
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
    ObjectPooler _shootEffectPool;
    ObjectPooler _bulletTriggerEffectPool;
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
        _bulletPool.FillThePool(10);

        _bulletTriggerEffectPool = new ObjectPooler(bulletTriggerEffect);
        _bulletTriggerEffectPool.FillThePool(10);

        if (bulletShootEffect)
        {
            _shootEffectPool = new ObjectPooler(bulletShootEffect);
            _shootEffectPool.FillThePool(3);
        }
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

    public void BulletTriggerEffect(Vector3 pos, Quaternion rot)
    {
        GameObject bulletTriggerEffect = _bulletTriggerEffectPool.GetObjectFromPoolAtPosition(pos, rot);

        StartCoroutine(SendToPoolObjectTimout(bulletTriggerEffect, "BulletTriggerEffect", 2f));
    }

    private void FireEffectWeapon (Transform bulletPosition)
    {
        GameObject shootEffect = _shootEffectPool.GetObjectFromPool();

        shootEffect.transform.parent = bulletPosition;
        shootEffect.transform.localPosition = new Vector3(0, 0, 0);

        StartCoroutine(SendToPoolObjectTimout(shootEffect, "ShootEffect", 1f));
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

        GameObject bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);

        // Destroy(bullet, 2f);
        StartCoroutine(SendToPoolObjectTimout(bullet, "Bullet", 2f));

        // Wait for X second
        yield return new WaitForSeconds(bulletRate);
        
        allowFire = true;
    }
        
    IEnumerator FireDoubleWeapon()
    {
        allowFire = false;
        GameObject bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);
        GameObject bullet2 = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition2.position, bulletPosition2.rotation);

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        bullet2.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);

        StartCoroutine(SendToPoolObjectTimout(bullet, "Bullet", 2f));
        StartCoroutine(SendToPoolObjectTimout(bullet2, "Bullet", 2f));

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
            bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition1.position, bulletPosition1.rotation);
        }
        else
        {
            bullet = _bulletPool.GetObjectFromPoolAtPosition(bulletPosition2.position, bulletPosition2.rotation);
        }

        bullet.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity + (transform.forward * bulletSpeed);
        useFirstWeapon = !useFirstWeapon;

        StartCoroutine(SendToPoolObjectTimout(bullet, "Bullet", 2f));

        // Wait for X second
        yield return new WaitForSeconds(bulletRate / 2);

        allowFire = true;
    }

    private IEnumerator SendToPoolObjectTimout(GameObject bulletObject, string objectType, float time)
    {
        // Wait for X second
        yield return new WaitForSeconds(time);

        if (objectType == "Bullet")
        {
            _bulletPool.SendObjectToPool(bulletObject);
        }else if (objectType == "ShootEffect")
        {
            _shootEffectPool.SendObjectToPool(bulletObject);
        }else if (objectType == "BulletTriggerEffect")
        {
            _bulletTriggerEffectPool.SendObjectToPool(bulletObject);
        }
    }
}
