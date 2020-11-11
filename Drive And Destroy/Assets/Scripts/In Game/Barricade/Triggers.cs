using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Triggers : MonoBehaviour
{
    public GameObject barricade;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = barricade.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Suprise Barricade Animation. (Fall to road)
        if (collider.tag == "PlayerShip" && this.name == "Barricade Trigger")
        {
            animator.SetTrigger("Suprise");
        }

        // Shake the camera when Suprise Barricade fall down.
        if (collider.name == "Barricade" && this.name == "Barricade Falled Trigger")
        {
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
        }
    }
}
