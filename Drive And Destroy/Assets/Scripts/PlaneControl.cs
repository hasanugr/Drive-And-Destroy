using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour {
    private Rigidbody rb;
    public GameObject planeModelObject;
    private Rigidbody planeModelRB;
    private float thrust = 15f;
    private float maxSpeed = 100f;
    private float rotateLimit = 30;
    // private float posChangeSpeed = 25f;
    // private float minSpeed = 20f;


    // Start is called before the first frame update
    void Start() {
        planeModelRB = planeModelObject.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        float _xMov = Input.GetAxisRaw("Horizontal"); // Sağ sol
        float _zMov = Input.GetAxisRaw("Vertical"); // İleri geri

        this.SpeedControl(_zMov);
        this.RotationControl(_xMov);

        if (_xMov == 0 && rb.velocity.x != 0) {
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        }
    }

    private void SpeedControl (float zMov) {
        // Debug.Log(rb.velocity.z);
        // Debug.Log(rb.transform.forward);
        if (zMov > 0 && rb.velocity.z < maxSpeed) {
            float speedVal = zMov * thrust;
            Debug.Log(rb.transform.forward * speedVal);
            rb.AddForce(transform.forward * speedVal);}
        // }else if (rb.velocity.z > 1 && (zMov == 0 || rb.velocity.z > maxSpeed)) {
        //     rb.AddForce(transform.forward * thrust);
        // }else if (rb.velocity.z < 1) {
        //     rb.velocity = new Vector3(0,0,0);
        // }
    }

    private void RotationControl (float xMov) {
        if (xMov != 0) {
            float rotater = xMov * 50;
            rb.transform.Rotate(Vector3.up * rotater * Time.deltaTime, Space.Self);
            // rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, rb.transform.rotation.y + rotater, 0), rotateSpeed * Time.deltaTime);
        }

        float vectorZ = xMov * -rotateLimit;

        // Transform planeTrans = planeModelObject.transform;
        // Debug.Log("Plane Trans. Rot. --> " + planeTrans.rotation);
        // Debug.Log("Queternion Euler --> " + Quaternion.Euler(0, 0, vectorZ));
        // targetRotation = Quaternion.FromToRotation(Vector3.up, rcHit.normal);
        // planeModelObject.transform.rotation = Quaternion.RotateTowards(planeTrans.rotation, Quaternion.Euler(0, 0, vectorZ), rotateSpeed * Time.deltaTime);

        // Quaternion target = Quaternion.Euler(0, 0, vectorZ);
        // var step = rotateSpeed * Time.deltaTime;
        // planeModelObject.transform.rotation = Quaternion.RotateTowards(planeModelObject.transform.rotation, target, step);
        // Debug.Log("Final TR --> " + planeModelObject.transform.rotation);
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
            rb.AddForce(new Vector3(-1, 0, -0.5f) * 8f, ForceMode.Impulse);
        }else if(force.x > 0) {
            rb.AddForce(new Vector3(1, 0, -0.5f) * 8f, ForceMode.Impulse);
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
