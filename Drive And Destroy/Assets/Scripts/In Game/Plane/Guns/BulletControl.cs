using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public GameObject BulletTriggerEffect;
    public float BulletDamage = 30.0f;
    public TrailRenderer[] Trails;

    private string _bulletHitSoundName;

    AudioManager _audioManager;

    private void OnEnable()
    {
        if (Trails.Length > 0)
        {
            for (int i = 0; i < Trails.Length; i++)
            {
                Trails[i].Clear();
            }
        }
    }

    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        string selectedVehicleId = GameManager.instance.selectedVehicle.name;
        _bulletHitSoundName = "BulletHit" + selectedVehicleId;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet when touch any collision
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
        
        // Get the contact point and create bullet hole at contact point
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.LookRotation(contact.normal);
        Vector3 pos = contact.point + (contact.normal * 0.01f);
        GameObject bulletHole = Instantiate(BulletTriggerEffect, pos, rot);
        _audioManager.PlayOneShot(_bulletHitSoundName);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Barricade"))
        {
            BlockBarricade barricade = collision.gameObject.GetComponent<BlockBarricade>();
            barricade.TakeDamage(BulletDamage);
        }else if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            BossAI boss = collision.gameObject.GetComponent<BossAI>();
            boss.TakeDamage(BulletDamage);
        }

        // Destroy the bullet hole on surface
        Destroy(bulletHole, 3f);
    }
}
