using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class SupriseBarricadeTrigger : MonoBehaviour
{
    public GameObject barricade;

    private bool _isFalled;

    // Start is called before the first frame update
    void Start()
    {
        SetBarricadePosition(1000);
        _isFalled = false;
    }

    private void OnEnable()
    {
        SetBarricadePosition(1000);
        _isFalled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Trigger the Suprise Barricade Animation. (Fall to road)
        if (collider.gameObject.CompareTag("PlayerShip") && this.name == "Barricade Trigger" && !_isFalled)
        {
            //animator.SetTrigger("Suprise");
            SetBarricadePosition(100);
            LeanTween.moveY(barricade, 2.75f, 0.1f);
            StartCoroutine(ShakeCameraAfterFall(0.1f));
            _isFalled = true;
        }
    }

    private void SetBarricadePosition(float yPos)
    {
        Vector3 objPos = barricade.transform.position;
        objPos.y = yPos;
        barricade.transform.position = objPos;
    }

    IEnumerator ShakeCameraAfterFall(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
    }
}
