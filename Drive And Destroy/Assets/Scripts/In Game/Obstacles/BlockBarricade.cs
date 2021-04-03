using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBarricade : MonoBehaviour
{
    public GameObject[] CrashEffects;
    public float FullHealth = 100;
    public bool IsBreakable = true;
    public int BarricadeLevel = 0;

    private GameObject _barricadeHolder;
    private BoxCollider _barricadeBoxCollider;
    private MeshRenderer _barricadeMeshRenderer;
    private int[] _destroyPoints = { 20, 25, 30, 35 };
    private int _barricadeLevelToIndex;
    private float _health;
    private InGameManager _igm;

    private void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _health = FullHealth;

        _barricadeHolder = gameObject.transform.parent.gameObject;
        _barricadeBoxCollider = gameObject.GetComponent<BoxCollider>();
        _barricadeMeshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        _health = FullHealth;
        _barricadeLevelToIndex = Mathf.Clamp(BarricadeLevel, 1, 4) - 1;
    }

    public void TakeDamage (float damage)
    {
        if (IsBreakable)
        {
            _health -= damage;

            if (_health <= 0)
            {
                _igm.AddPlayerPoint(_destroyPoints[_barricadeLevelToIndex]);
                CrashEffectApply();
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
            _barricadeBoxCollider.enabled = false;
            _barricadeMeshRenderer.enabled = false;

            CrashEffects[_barricadeLevelToIndex].SetActive(true);
            StartCoroutine(DisableBarricade(2.2f));
        }

    }

    IEnumerator DisableBarricade(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        _barricadeHolder.SetActive(false);
        _barricadeBoxCollider.enabled = true;
        _barricadeMeshRenderer.enabled = true;
    }
}
