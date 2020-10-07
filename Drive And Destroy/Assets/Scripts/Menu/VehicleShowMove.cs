using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleShowMove : MonoBehaviour
{
    public float heoverHeight = 0.03f;

    private bool goingUp = true;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.localPosition.y);
        transform.Rotate(0, 0.1f, 0 * Time.deltaTime); //rotates 50 degrees per second around z axis
        if (transform.localPosition.y > heoverHeight)
        {
            goingUp = false;
        }else if (transform.localPosition.y < 0)
        {
            goingUp = true;
        }

        if (goingUp)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 0.03f, 0), Time.deltaTime / 10);
        }else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - new Vector3(0, 0.03f, 0), Time.deltaTime / 10);
        }

    }
}
