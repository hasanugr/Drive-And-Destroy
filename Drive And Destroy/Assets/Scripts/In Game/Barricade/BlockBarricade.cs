using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBarricade : MonoBehaviour
{
    public GameObject CrashEffect;
    public float health = 100;

    public void takeDamage (float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(this.transform.parent.gameObject);
            CrashEffectApply();
        }
    }

    public float getHealth ()
    {
        return health;
    }

    private void CrashEffectApply()
    {
        if (CrashEffect != null)
        {
            Instantiate(CrashEffect, transform.position, transform.rotation);
        }
    }
}
