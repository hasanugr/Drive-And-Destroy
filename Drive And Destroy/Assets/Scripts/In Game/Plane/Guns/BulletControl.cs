using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public GameObject bulletTriggerEffect;
    public float bulletDamage = 30.0f;
    

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet when touch any collision
        Destroy(this.gameObject);
        
        // Get the contact point and create bullet hole at contact point
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.LookRotation(contact.normal);
        Vector3 pos = contact.point + (contact.normal * 0.01f);
        GameObject bulletHole = Instantiate(bulletTriggerEffect, pos, rot);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Barricade"))
        {
            BlockBarricade barricade = collision.gameObject.GetComponent<BlockBarricade>();
            barricade.takeDamage(bulletDamage);
        }

        // Destroy the bullet hole on surface
        Destroy(bulletHole, 3f);
    }
}
