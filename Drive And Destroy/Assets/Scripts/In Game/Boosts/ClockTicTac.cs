using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTicTac : MonoBehaviour
{
    [SerializeField]
    private GameObject smallStick;
    [SerializeField]
    private GameObject bigStick;
    [SerializeField]
    private bool isTweenClock = true;
    [SerializeField]
    private bool isTweeningClockway = true;

    // Start is called before the first frame update
    void Start()
    {
        if (isTweenClock)
        {
            LeanTween.rotateAroundLocal(gameObject, Vector3.forward, (isTweeningClockway ? -360f : 360f), 6f).setRepeat(-1);
        }
        LeanTween.rotateAroundLocal(smallStick, Vector3.up, -360f, 12f).setRepeat(-1);
        LeanTween.rotateAroundLocal(bigStick, Vector3.up, -360f, 1f).setRepeat(-1);
    }

}
