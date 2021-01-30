using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBarricade : MonoBehaviour
{
    public GameObject CrashEffect;
    public bool IsSpecialColor = false;
    public Color SpecialColor;
    public float FullHealth = 100;
    public bool IsBreakable = true;
    public int BarricadeLevel = 0;

    private float _health;
    private InGameManager _igm;

    private void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _health = FullHealth;
    }

    public void TakeDamage (float damage)
    {
        if (IsBreakable)
        {
            _health -= damage;

            if (_health <= 0)
            {
                Destroy(this.transform.parent.gameObject);
                CrashEffectApply();

                switch (BarricadeLevel)
                {
                    case 1:
                        _igm.AddPlayerPoint(20);
                        break;
                    case 2:
                        _igm.AddPlayerPoint(30);
                        break;
                    case 3:
                        _igm.AddPlayerPoint(50);
                        break;
                    default:
                        _igm.AddPlayerPoint(50);
                        break;
                }
            }
        }
    }

    public float GetHealth ()
    {
        return _health;
    }

    private void CrashEffectApply()
    {
        if (CrashEffect != null)
        {
            GameObject crashEffectPS = Instantiate(CrashEffect, transform.position, transform.rotation);
            if (IsSpecialColor)
            {
                crashEffectPS.GetComponent<Renderer>().material.color = SpecialColor;
            }
        }
    }
}
