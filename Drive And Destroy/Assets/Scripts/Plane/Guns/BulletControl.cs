using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public GameObject bulletHolePrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet when touch any collision
        Destroy(this.gameObject);

        // Get the contact point and create bullet hole at contact point
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        GameObject bulletHole = Instantiate(bulletHolePrefab, pos, rot);

        // Destroy the bullet hole on surface
        Destroy(bulletHole, 2f);
    }
}
