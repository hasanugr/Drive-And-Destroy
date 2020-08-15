using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour {
    // Objects and Components
    private Rigidbody rb;
    public GameObject planeModelObject;
    private Rigidbody planeModelRB;
    // Speed things
    private float curSpeed = 0.0f;
    public float maxSpeed = 100.0f;
    public float acceleration = 25.0f;
    public float dragDeceleration = 15.0f;
    // Rotate Things
    private float rotateLimit = 30.0f;
    private float rotateSpeed = 50.0f;
    private float animateRotateSpeed = 100.0f;
    // Rebound Things
    public float reboundForce = 20.0f;
    private bool reboundAppliying = false;

    // Start is called before the first frame update
    void Start() {
        planeModelRB = planeModelObject.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        float _yMov = Input.GetAxisRaw("Horizontal"); // Sağ sol
        float _zMov = Input.GetAxisRaw("Vertical"); // İleri geri

        this.SpeedControl(_zMov);
        this.RotationControl(_yMov);
        // Debug.Log("Speed: " + curSpeed);
        rb.transform.Translate(Vector3.forward * curSpeed * Time.deltaTime);

        // if (_yMov == 0 && rb.velocity.x != 0) {
        //     rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        // }
    }

    private void SpeedControl (float zMov) {
        if (zMov > 0 && rb.velocity.z < maxSpeed) {
            curSpeed += acceleration * Time.deltaTime;
            if (curSpeed > maxSpeed) {
                curSpeed = maxSpeed;
            }
        }else if (zMov <= 0) {
            if (curSpeed > 0) {
                curSpeed -= dragDeceleration * Time.deltaTime;
            }else if (curSpeed < 0) {
                curSpeed = 0;
            }
        }
    }

    private void RotationControl (float yMov) {
        if (yMov != 0) {
            float rotater = yMov * rotateSpeed;
            rb.transform.Rotate(Vector3.up * rotater * Time.deltaTime, Space.Self);
        }

        float vectorZ = yMov * -rotateLimit;
        Quaternion target = Quaternion.Euler(0, 0, vectorZ);
        float step = animateRotateSpeed * Time.deltaTime;
        planeModelObject.transform.localRotation = Quaternion.RotateTowards(planeModelObject.transform.localRotation, target, step);
	}

    private void ReboundForce (Vector3 contactPos) {
        if (!reboundAppliying) {
            // Debug.Log("ReboundForce");
            Vector3 force = rb.transform.position - contactPos;
            force.Normalize ();
            force.y = 0;
            // Debug.Log(force);
            reboundAppliying = true;
            StartCoroutine(ApplyReboundForce(0, force));
            curSpeed = curSpeed / 4;
        }
     }

     IEnumerator ApplyReboundForce(int counter, Vector3 reboundWay)
    {
        rb.transform.Translate(reboundWay * reboundForce * Time.deltaTime, Space.World);

        // wait for 1 second
        yield return new WaitForSeconds(0.001f);
        counter++;
        if (counter < 10) {
            // Debug.Log("ApplyReboundForce");
            yield return StartCoroutine(ApplyReboundForce(counter, reboundWay));
        }else {
            reboundAppliying = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "SideWall") {
            // Debug.Log("Duvara ile temas.");
            ContactPoint contact = col.contacts[0];
            Vector3 contactPos = contact.point;
            ReboundForce(contactPos);
            // Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            
            // Debug.Log(contact);
            // Debug.Log(rot);
            // Debug.Log(pos);
        }

    }
    
}
