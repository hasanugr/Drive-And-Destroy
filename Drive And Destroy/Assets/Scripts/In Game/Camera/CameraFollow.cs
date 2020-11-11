using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform target; // Self explanatory
	public float distance = 20.0f; // Standard distance to follow object
	public float height = 5.0f; // The height of the camera
	public float heightDamping = 2.0f; // Smooth out the height position

	public float lookAtHeight = 0.0f; // An offset of the target

	private Rigidbody parentRigidbody; // Used to determine how far the camera should zoom out when the car moves forward

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

	VehicleMovement movement;               //A reference to the ship's VehicleMovement script

	void Start()
	{
		GameObject targetObject = GameObject.FindGameObjectWithTag("Player");
		target = targetObject.transform;
		parentRigidbody = targetObject.GetComponent<Rigidbody>();

		// lookAtVector = new Vector3(0, lookAtHeight, 0);
		movement = target.GetComponent<VehicleMovement>();

	}

	void FixedUpdate()
	{
		//Generate custom percentage of speed of ship with custom speed value to camera verticle rotation value
		float speedPercent = movement.speed / 80.0f;
		// Debug.Log(speedPercent);

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

		// Look at height of ship related speed percent.
		float lookAtHeightRSP = speedPercent >= 1 ? lookAtHeight : lookAtHeight * speedPercent;
		lookAtVector = new Vector3(0, lookAtHeightRSP, 0);

        //Debug.Log("Speed Percent: " + speedPercent);
        //Debug.Log("LAH: " + lookAtHeightRSP);

        transform.LookAt(target.position + lookAtVector);
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotationAngle, target.eulerAngles.z);

	}
}