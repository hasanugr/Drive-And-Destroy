using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float RayLength;
    public float RayDeflection;

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray myRay = new Ray(transform.position, transform.forward + (transform.right * RayDeflection));
        Physics.Raycast(myRay, out hit, RayLength);
        if (hit.collider)
        {
            print("Hit: " + hit.collider.gameObject.name);
        }else
        {
            print("Hit: -----");
        }
        //Debug.DrawRay(transform.position, transform.forward * 50.0f, Color.red);
        Debug.DrawRay(myRay.origin, myRay.direction * RayLength, Color.red);
    }
}
