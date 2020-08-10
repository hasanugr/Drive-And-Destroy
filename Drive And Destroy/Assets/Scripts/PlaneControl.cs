using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour
{
    public GameObject planeModelObject;
    private float rotateSpeed = 100f;
    private float rotateLimit = 30;
    private float posChangeSpeed = 25f;
    private float thrust = 80f;
    private Rigidbody rb;
    private Rigidbody planeModelRB;
    private float minSpeed = 20f;

    // Start is called before the first frame update
    void Start() {
        planeModelRB = planeModelObject.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        // Vector3 _movHorizontal = rb.transform.right * _xMov;
        Vector3 _movVertical = rb.transform.forward * _zMov;

        //Final movement vector
        Vector3 _velocity = (_movVertical).normalized * posChangeSpeed;

        rb.MovePosition(rb.position + _velocity * Time.deltaTime);

        if (_xMov != 0) {
            float rotater = _xMov * 50;
            rb.transform.Rotate(Vector3.up * rotater * Time.deltaTime);
            // rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, rb.transform.rotation.y + rotater, 0), rotateSpeed * Time.deltaTime);
        }
        this.RotationControl(_xMov);

        // if (_xMov == 0 && _zMov == 0 && (rb.velocity.x != 0 || rb.velocity.y != 0)) {
        //     rb.velocity = new Vector3(0f, 0f, rb.velocity.z);
        // }
        // Debug.Log("X_Mov: " + _xMov);
        // Debug.Log("Y_Mov: " + _yMov);
        // Debug.Log(rb.velocity);

    }

    private void ApplyReboundForce (Collision col) {
        Debug.Log("ApplyReboundForce");
        Debug.Log(rb.transform.position);
        Debug.Log(col.transform.position);
        Vector3 force = rb.transform.position - col.transform.position;
        Debug.Log("Force X --> " + force.x);
        Debug.Log(force);
        force.Normalize ();
        // Debug.Log(force);
        // force.x negatif değer ise sağ duvar pozitif değer ise sol duvar ile çarpışma olmuştur.
        if (force.x < 0) {
            rb.AddForce(new Vector3(-1, 0, -0.3f) * 8f, ForceMode.Impulse);
        }else if(force.x > 0) {
            rb.AddForce(new Vector3(1, 0, -0.3f) * 8f, ForceMode.Impulse);
        }
        
        // rb.AddForce (force * 50f, ForceMode.Impulse);

     }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Çarpışma oldu.");
        if (col.gameObject.tag == "SideWall") {
            Debug.Log("Duvara Çarptı.!!!");
            //  appliedSpeed = speed * 0.5f;
            //  elapsedEngineStartWaiting = 0f;
             ApplyReboundForce (col);
         }

    }

    private void OnCollisionStay(Collision col) {
        // Debug.Log("Touching..");
        if (col.gameObject.tag == "PipeWall") {
            this.posChangeSpeed = 5f;
            // Debug.Log("Duvara Dokunuyor.!!");
            if (rb.velocity.z > minSpeed) {
                rb.AddForce(-transform.forward * thrust);
            }
           
        }
    }

    private void OnCollisionExit(Collision col) {
        if (col.gameObject.tag == "PipeWall"){
            this.posChangeSpeed = 25f;
        }
    }

    protected void LateUpdate() {
        // rb.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void RotationControl (float xMov) {
		// Debug.Log("X-Mov: "+ xMov);
		// Debug.Log("Y-Mov: "+ yMov);
        float vectorZ = xMov * -rotateLimit;

        // Transform planeTrans = planeModelObject.transform;
        // Debug.Log("Plane Trans. Rot. --> " + planeTrans.rotation);
        // Debug.Log("Queternion Euler --> " + Quaternion.Euler(0, 0, vectorZ));
        // targetRotation = Quaternion.FromToRotation(Vector3.up, rcHit.normal);
        // planeModelObject.transform.rotation = Quaternion.RotateTowards(planeTrans.rotation, Quaternion.Euler(0, 0, vectorZ), rotateSpeed * Time.deltaTime);

        Quaternion target = Quaternion.Euler(0, 0, vectorZ);
        var step = rotateSpeed * Time.deltaTime;
        planeModelObject.transform.rotation = Quaternion.RotateTowards(planeModelObject.transform.rotation, target, step);
        Debug.Log("Final TR --> " + planeModelObject.transform.rotation);
	}
}
