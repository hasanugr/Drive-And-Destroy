using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour {
    private Rigidbody rb;
    public GameObject planeModelObject;
    private Rigidbody planeModelRB;
    // private float thrust = 15.0f;
    public float maxSpeed = 100.0f;
    private float rotateLimit = 30.0f;
    private float rotateSpeed = 50.0f;
    private float animateRotateSpeed = 50.0f;
    // private float posChangeSpeed = 25f;
    // private float minSpeed = 20f;
    public float acceleration = 25.0f;
    public float dragDeceleration = 15.0f;
    private float curSpeed = 0.0f;
    public float reboundForce = 5.0f;


    // Start is called before the first frame update
    void Start() {
        planeModelRB = planeModelObject.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        float _yMov = Input.GetAxisRaw("Horizontal"); // Sağ sol
        float _zMov = Input.GetAxisRaw("Vertical"); // İleri geri

        // Debug.Log("Time --> " + Time.time);

        this.SpeedControl(_zMov);
        this.RotationControl(_yMov);
        // Debug.Log("Speed: " + curSpeed);
        rb.transform.Translate(Vector3.forward * curSpeed * Time.deltaTime);

        // if (_yMov == 0 && rb.velocity.x != 0) {
        //     rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        // }
    }

    private void SpeedControl (float zMov) {
        // Debug.Log(rb.velocity.z);
        // Debug.Log(rb.transform.forward);
        if (zMov > 0 && rb.velocity.z < maxSpeed) {
            // float speedVal = zMov * thrust;
            // Debug.Log(rb.transform.forward * speedVal);
            // rb.AddForce(transform.forward * speedVal);
            
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
        // }else if (rb.velocity.z > 1 && (zMov == 0 || rb.velocity.z > maxSpeed)) {
        //     rb.AddForce(transform.forward * thrust);
        // }else if (rb.velocity.z < 1) {
        //     rb.velocity = new Vector3(0,0,0);
        // }
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

    private void ApplyReboundForce (Collision col) {
        // Debug.Log("ApplyReboundForce");
        // Debug.Log(rb.transform.position);
        // Debug.Log(col.transform.position);
        Vector3 force = rb.transform.position - col.transform.position;
        // Debug.Log("Force X --> " + force.x);
        // Debug.Log(force);
        force.Normalize ();
        // Debug.Log(force);
        // force.x negatif değer ise sağ duvar pozitif değer ise sol duvar ile çarpışma olmuştur.
        if (force.x < 0) {
            // rb.AddForce(new Vector3(-1, 0, 0) * reboundForce, ForceMode.Impulse);
            int reboundCounter = 0;
            while(reboundCounter < 5) {
                rb.transform.Translate(Vector3.left * 25 * Time.deltaTime);
                reboundCounter++;
                // Debug.Log("Rebound Count: " + reboundCounter);
            } 
            curSpeed = curSpeed / 4;
        }else if(force.x > 0) {
            // rb.AddForce(new Vector3(1, 0, 0) * reboundForce, ForceMode.Impulse);
            int reboundCounter = 0;
            while(reboundCounter < 5) {
                rb.transform.Translate(Vector3.right * 25 * Time.deltaTime);
                reboundCounter++;
                // Debug.Log("Rebound Count: " + reboundCounter);
            }
            curSpeed = curSpeed / 4;
        }
        
        // rb.AddForce (force * 50f, ForceMode.Impulse);

     }

    void OnCollisionEnter(Collision col)
    {
        // Debug.Log("Çarpışma oldu.");
        if (col.gameObject.tag == "SideWall") {
            // Debug.Log("Duvara Çarptı.!!!");
            //  appliedSpeed = speed * 0.5f;
            //  elapsedEngineStartWaiting = 0f;
             ApplyReboundForce (col);
         }

    }

    // private void OnCollisionStay(Collision col) {
    //     // Debug.Log("Touching..");
    //     if (col.gameObject.tag == "PipeWall") {
    //         this.posChangeSpeed = 5f;
    //         // Debug.Log("Duvara Dokunuyor.!!");
    //         if (rb.velocity.z > minSpeed) {
    //             rb.AddForce(-transform.forward * thrust);
    //         }
           
    //     }
    // }

    // private void OnCollisionExit(Collision col) {
    //     if (col.gameObject.tag == "PipeWall"){
    //         this.posChangeSpeed = 25f;
    //     }
    // }

    // protected void LateUpdate() {
    //     // rb.transform.localEulerAngles = new Vector3(0, 0, 0);
    // }

    
}
