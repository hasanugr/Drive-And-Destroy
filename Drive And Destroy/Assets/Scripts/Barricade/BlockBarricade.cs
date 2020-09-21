using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBarricade : MonoBehaviour
{
    public float health = 100;

    public void takeDamage (float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(this.transform.parent.gameObject);
        }
    }

    public float getHealth ()
    {
        return health;
    }
}
