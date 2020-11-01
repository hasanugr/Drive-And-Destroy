using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VCHandler : MonoBehaviour
{
    public int SelectedShemeIndex;
    private int SelectingShemeIndex;
    public GameObject[] ControlShemes;
    public Color onColor;
    public Color offColor;

    public float moveSpeed = 2.0f;
    private bool switching = false;
    private float t = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        ControlShemes[SelectedShemeIndex].GetComponent<RawImage>().color = onColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (switching)
        {
            StartSelecting();
            StopSelecting();
        }
    }

    private void StartSelecting()
    {
        ControlShemes[SelectedShemeIndex].GetComponent<RawImage>().color = SmoothlyChangeColor(onColor, offColor);
        ControlShemes[SelectingShemeIndex].GetComponent<RawImage>().color = SmoothlyChangeColor(offColor, onColor);
    }

    private Color SmoothlyChangeColor(Color startColor, Color endColor)
    {
        Color colorValue = Color.Lerp(startColor, endColor, t += moveSpeed * Time.deltaTime);
        return colorValue;
    }

    void StopSelecting()
    {
        if (t > 1.0f)
        {
            switching = false;
            t = 0.0f;
            SelectedShemeIndex = SelectingShemeIndex;
        }
    }

    public void SelectVehicleControlSheme(int index)
    {
        if (!switching)
        {
            SelectingShemeIndex = index;
            switching = true;
        }
    }
}
