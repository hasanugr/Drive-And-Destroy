using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    public enum BoostTypes
    {
        Turbo,
        Heal,
        Gold,
        Timer
    }

    public BoostTypes BoostType;
    public GameObject ChildBodyObject;

    public int ValueToAdd = 20;
    public float DisappearDuration = 0.2f;
    public float RotateAroundTime = 3f;
    public float EasePunchSize = 1.2f;

    private Transform _defaultParent;
    private Vector3 _defaultPos;

    private InGameManager _igm;
    private Vector3 _childBodyObjectScale;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _audioManager = FindObjectOfType<AudioManager>();

        if (ChildBodyObject != null)
        {
            _childBodyObjectScale = ChildBodyObject.transform.localScale;
            LeanTween.rotateAround(this.gameObject, transform.up, 360, RotateAroundTime).setLoopClamp();
            LeanTween.scale(ChildBodyObject, _childBodyObjectScale * EasePunchSize, 1).setEasePunch().setLoopPingPong();
        }

        _defaultParent = this.gameObject.transform.parent;
        _defaultPos = this.gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Turbo Boost
        if (collider.gameObject.CompareTag("PlayerShip"))
        {
            bool isDestroy = false;
            VehicleMovement vehicleMovement = collider.GetComponentInParent<VehicleMovement>();

            if (BoostType == BoostTypes.Turbo)
            {
                if (vehicleMovement.GetTurbo() < 100)
                {
                    _audioManager.Play("BoostSound");
                    vehicleMovement.AddTurboPoint(ValueToAdd);
                    isDestroy = true;
                }
            }
            else if (BoostType == BoostTypes.Heal)
            {
                if (vehicleMovement.GetHealth() < 100)
                {
                    _audioManager.Play("BoostSound");
                    vehicleMovement.AddHealth(ValueToAdd);
                    isDestroy = true;
                }
            }else if (BoostType == BoostTypes.Gold)
            {
                _audioManager.Play("CoinPickup");
                _igm.AddGold(ValueToAdd);
                isDestroy = true;
            }else if (BoostType == BoostTypes.Timer)
            {
                _audioManager.Play("BoostSound");
                _igm.ResetCountdownTimer();
                gameObject.SetActive(false);
            }

            if (isDestroy)
            {
                this.gameObject.transform.parent = collider.gameObject.transform;
                LeanTween.scale(this.gameObject, new Vector3(0, 0, 0), DisappearDuration);

                StartCoroutine(SetPassiveBoost(DisappearDuration));
            }
        }

    }

    IEnumerator SetPassiveBoost(float time)
    {
        // Wait for X second
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        this.gameObject.transform.parent = _defaultParent;
        this.gameObject.transform.position = _defaultPos;
        this.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
}
