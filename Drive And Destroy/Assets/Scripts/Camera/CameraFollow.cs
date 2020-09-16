using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	//public Transform target;
	//private float sSpeed = 1000.0f;
	//private Vector3 dist = new Vector3(0, 2, -6);
	//private Vector3 moveLimit;

	//private void Update()
	//{
	//    moveLimit = new Vector3(PercentCalculator(transform.position.x, 20), PercentCalculator(transform.position.y, 20), 0);
	//    Vector3 dPos = target.position + dist;
	//    Vector3 sPos = Vector3.Lerp(transform.position, dPos, sSpeed * Time.deltaTime);
	//    transform.position = sPos;
	//    transform.LookAt(target.position);
	//}

	//private float PercentCalculator(float val, int percent)
	//{
	//    return val * percent / 100;
	//}

	public Transform target; // Self explanatory
	public float distance = 20.0f; // Standard distance to follow object
	public float height = 5.0f; // The height of the camera
	public float heightDamping = 2.0f; // Smooth out the height position

	public float lookAtHeight = 0.0f; // An offset of the target

	public Rigidbody parentRigidbody; // Used to determine how far the camera should zoom out when the car moves forward

	public float rotationSnapTime = 0.3F; // The time it takes to snap back to original rotation

	public float distanceSnapTime; // The time it takes to snap back to the original distance or the zoomed distance (depending on speed of parentRigidyBody)
	public float distanceMultiplier; // Make this around 0.1f for a small zoom out or 0.5f for a large zoom (depending on the speed of your rigidbody)

	private Vector3 lookAtVector;

	private float usedDistance;

	float wantedRotationAngle;
	float wantedHeight;
	float wantedRotationAngleZ;

	float currentRotationAngle;
	float currentHeight;
	float currentRotationAngleZ;

	Quaternion currentRotation;
	Vector3 wantedPosition;

	private float yVelocity = 0.0F;
	private float zVelocity = 0.0F;

	void Start()
	{

		lookAtVector = new Vector3(0, lookAtHeight, 0);

	}

	void FixedUpdate()
	{

		wantedHeight = target.position.y + height;
		currentHeight = transform.position.y;

		wantedRotationAngle = target.eulerAngles.y;
		currentRotationAngle = transform.eulerAngles.y;

		currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);

		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		wantedPosition = target.position;
		wantedPosition.y = currentHeight;

		usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);

		wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);

		transform.position = wantedPosition;

		transform.LookAt(target.position + lookAtVector);
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotationAngle, target.eulerAngles.z);

	}
}