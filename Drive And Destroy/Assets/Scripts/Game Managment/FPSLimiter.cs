using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;  // VSync must be disabled
        Application.targetFrameRate = 300;
    }
}
