using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBarricade : MonoBehaviour
{
    public GameObject[] CrashEffects;
    public float FullHealth = 100;
    public bool IsBreakable = true;
    public int BarricadeLevel = 0;

    private float _health;
    private InGameManager _igm;

    private void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _health = FullHealth;

        /*gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;*/
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
                        _igm.AddPlayerPoint(25);
                        break;
                    case 3:
                        _igm.AddPlayerPoint(30);
                        break;
                    case 4:
                        _igm.AddPlayerPoint(35);
                        break;
                    default:
                        _igm.AddPlayerPoint(35);
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
        if (CrashEffects.Length > 0)
        {
            /*gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;*/
            switch (BarricadeLevel)
            {
                case 1:
                    Instantiate(CrashEffects[0], transform.position, transform.rotation);
                    //CrashEffects[0].SetActive(true);
                    break;
                case 2:
                    Instantiate(CrashEffects[1], transform.position, transform.rotation);
                    //CrashEffects[1].SetActive(true);
                    break;
                case 3:
                    Instantiate(CrashEffects[2], transform.position, transform.rotation);
                    //CrashEffects[2].SetActive(true);
                    break;
                case 4:
                    Instantiate(CrashEffects[3], transform.position, transform.rotation);
                    //CrashEffects[3].SetActive(true);
                    break;
                default:
                    Instantiate(CrashEffects[3], transform.position, transform.rotation);
                    //CrashEffects[3].SetActive(true);
                    break;
            }
        }

    }
}
