using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    public enum BoostTypes
    {
        Turbo,
        Heal,
        Gold
    }

    public BoostTypes BoostType;
    public GameObject ChildBodyObject;

    public int ValueToAdd = 20;
    public float DisappearDuration = 0.2f;
    public float RotateAroundTime = 3f;
    public float EasePunchSize = 1.2f;

    private InGameManager _igm;
    private Vector3 _childBodyObjectScale;

    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _childBodyObjectScale = ChildBodyObject.transform.localScale;

        LeanTween.rotateAround(this.gameObject, transform.up, 360, RotateAroundTime).setLoopClamp();
        LeanTween.scale(ChildBodyObject, _childBodyObjectScale * EasePunchSize, 1).setEasePunch().setLoopPingPong();
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
                    vehicleMovement.AddTurboPoint(ValueToAdd);
                    isDestroy = true;
                }
            }
            else if (BoostType == BoostTypes.Heal)
            {
                print("Heal Taked.!!");
                if (vehicleMovement.GetHealth() < 100)
                {
                    print("Heal Boosted.!! " + vehicleMovement.GetHealth());
                    vehicleMovement.AddHealth(ValueToAdd);
                    isDestroy = true;
                }
            }else if (BoostType == BoostTypes.Gold)
            {
                _igm.AddGold(ValueToAdd);
                    isDestroy = true;
            }

            if (isDestroy)
            {
                this.gameObject.transform.parent = collider.gameObject.transform;
                LeanTween.scale(this.gameObject, new Vector3(0, 0, 0), DisappearDuration).setDestroyOnComplete(true);
            }
        }
    }
}
