using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchHandler : MonoBehaviour
{
    public bool isOn;

    public GameObject SBPoint;
    private RectTransform SBPointTransform;
    private float SBPointSize;

    public RectTransform SwitchHead;
    public float onPosX;
    public float offPosX;
    public float SBPointOffset;

    public GameObject SBHandler;
    public Color onColor;
    public Color offColor;
    private RawImage SBHandlerRI;

    public float moveSpeed;
    private bool switching = false;
    private float t = 0.0f;

    private void Awake()
    {
        SBPointTransform = SBPoint.GetComponent<RectTransform>();

        SBPointSize = SBPointTransform.sizeDelta.x;
        float SwitchHeadSizeX = SwitchHead.sizeDelta.x;
        onPosX = (SBPointSize / 2) - (SwitchHeadSizeX / 2) - SBPointOffset;
        offPosX = onPosX * -1;

        SBHandlerRI = SBHandler.GetComponent<RawImage>();
    }

    void Start()
    {
        if (isOn)
        {
            SBPoint.transform.localPosition = new Vector3(onPosX, 0, 0);
            SBHandlerRI.color = onColor;
        }
        else
        {
            SBPoint.transform.localPosition = new Vector3(offPosX, 0, 0);
            SBHandlerRI.color = offColor;
        }
    }

    void Update()
    {
        if (switching)
        {
            StartToggleing(isOn);
        }
    }

    private void StartToggleing(bool tStatus)
    {
        if (tStatus)
        {
            SBHandlerRI.color = SmoothlyChangeColor(onColor, offColor);
            SBPoint.transform.localPosition = SmoothlyMove(onPosX, offPosX);
        }
        else
        {
            SBHandlerRI.color = SmoothlyChangeColor(offColor, onColor);
            SBPoint.transform.localPosition = SmoothlyMove(offPosX, onPosX);
        }
    }

    private Color SmoothlyChangeColor(Color startColor, Color endColor)
    {
        Color colorValue = Color.Lerp(startColor, endColor, t += moveSpeed * Time.deltaTime);
        return colorValue;
    }

    private Vector3 SmoothlyMove(float startPosX, float endPosX)
    {
        Vector3 position = new Vector3(Mathf.Lerp(startPosX, endPosX, t += moveSpeed * Time.deltaTime), 0, 0);
        StopSwitching();
        return position;
    }

    void StopSwitching()
    {
        if (t > 1.0f)
        {
            switching = false;
            t = 0.0f;
            switch (isOn)
            {
                case true: isOn = false;
                    break;
                case false: isOn = true;
                    break;
            }
        }
    }

    public void Switch()
    {
        switching = true;
    }
}
