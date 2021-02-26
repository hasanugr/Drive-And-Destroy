using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EZCameraShake;

public class VirusEffectTrigger : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject DestroyEffect;
    public GameObject VirusPartycleSystem;

    [Header("Camera Shake Settings")]
    public float CameraShakeMagnitude = 6.0f;
    public float CameraShakeRoughness = 12.0f;
    public float CameraShakeEffectDuration = 10.0f;

    [Header("Half Blind Settings")]
    public float HalfBlindDuration = 5.0f;
    public GameObject HalfBlindVolumeObject;

    [Header("Change Rotate Settings")]
    public float ChangeRotateDuration = 5.0f;

    // Our saved shake instance.
    private CameraShakeInstance _shakeInstance;
    // Player Input script
    private PlayerInput _playerInput;

    private BoxCollider _thisBoxCollider;

    InGameManager _igm;
    private enum VirusEffectTypes
    {
        ShakeTheCamera,
        HalfBlindView,
        ChangeRotateController
    }

    // Start is called before the first frame update
    void Start()
    {

        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();

        _thisBoxCollider = this.GetComponent<BoxCollider>();

        // Camera Shake Part //
        //We make a single shake instance that we will fade in and fade out when the player enters and leaves the trigger area.
        _shakeInstance = CameraShaker.Instance.StartShake(CameraShakeMagnitude, CameraShakeRoughness, 2);
        //Immediately make the shake inactive.  
        _shakeInstance.StartFadeOut(0);
        //We don't want our shake to delete itself once it stops shaking.
        _shakeInstance.DeleteOnInactive = false;

        // Change Rotate Part //
        GameObject targetObject = GameObject.FindGameObjectWithTag("Player");
        _playerInput = targetObject.GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter(Collider col)
    {
        // Trigger the Virus effect proccess.
        if (col.CompareTag("PlayerShip") && !_igm.IsVirusEffected)
        {
            StartVirusEffect();
            
            _igm.IsVirusEffected = true;
            _thisBoxCollider.enabled = false;
            VirusPartycleSystem.SetActive(false);
        }
    }

    public void StartVirusEffect()
    {
        VirusEffectTypes virusEffectType = VirusEffectTypes.ChangeRotateController; //GetRandomEffect();
        switch (virusEffectType)
        {
            case VirusEffectTypes.ShakeTheCamera:
                StartCoroutine(StartShakeTheCamera());
                break;
            case VirusEffectTypes.HalfBlindView:
                StartCoroutine(StartHalfBlindView());
                break;
            case VirusEffectTypes.ChangeRotateController:
                StartCoroutine(StartChangeRotateController());
                break;
            default:
                Debug.LogWarning("Virus Effect Type can not selected.!!");
                break;
        }
    }

    private VirusEffectTypes GetRandomEffect()
    {
        var values = Enum.GetValues(typeof(VirusEffectTypes));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (VirusEffectTypes)values.GetValue(random);
    }

    private void DestroyObject()
    {
        _igm.IsVirusEffected = false;
        Destroy(gameObject);
    }

    IEnumerator StartShakeTheCamera()
    {
        _igm.ActivateVirusEffectStatus("Earthquake", CameraShakeEffectDuration);
        _shakeInstance.StartFadeIn(0.1f);

        //yield on a new YieldInstruction that waits for X(CameraShakeEffectDuration) seconds.
        yield return new WaitForSeconds(CameraShakeEffectDuration);

        _shakeInstance.StartFadeOut(0.5f);
        DestroyObject();
    }

    IEnumerator StartHalfBlindView()
    {
        _igm.ActivateVirusEffectStatus("Myopic", HalfBlindDuration);
        GameObject halfBlindVolume = Instantiate(HalfBlindVolumeObject);

        //yield on a new YieldInstruction that waits for X(HalfBlindDuration) seconds.
        yield return new WaitForSeconds(HalfBlindDuration);

        Destroy(halfBlindVolume);
        DestroyObject();
    }

    IEnumerator StartChangeRotateController()
    {
        _igm.ActivateVirusEffectStatus("ReverseMovement", ChangeRotateDuration);
        _playerInput.isReverseRotate = true;

        //yield on a new YieldInstruction that waits for X(ChangeRotateDuration) seconds.
        yield return new WaitForSeconds(ChangeRotateDuration);

        _playerInput.isReverseRotate = false;
        DestroyObject();
    }
}
