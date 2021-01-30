using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform target; // Self explanatory
	public float distance = 1.5f; // Standard distance to follow object
	public float height = 1.0f; // The height of the camera

	public float lookAtHeight = 1.0f; // An offset of the target

	private Rigidbody parentRigidbody; // Used to determine how far the camera should zoom out when the car moves forward

	public float distanceSnapTime; // The time it takes to snap back to the original distance or the zoomed distance (depending on speed of parentRigidyBody)
	public float distanceMultiplier; // Make this around 0.1f for a small zoom out or 0.5f for a large zoom (depending on the speed of your rigidbody)

	private Vector3 lookAtVector;

	private float usedDistance;

	Vector3 wantedPosition;
	Vector3 currentPosition;

	private float zVelocity = 0.0F;
	private Vector3 xyzVelocity = new Vector3(0, 0, 0);

	VehicleMovement movement;               //A reference to the ship's VehicleMovement script

	void Start()
	{
		GameObject targetObject = GameObject.FindGameObjectWithTag("Player");
		target = targetObject.transform;
		parentRigidbody = targetObject.GetComponent<Rigidbody>();

		movement = target.GetComponent<VehicleMovement>();
	}

	void FixedUpdate()
	{
		// Camera Hight and Distance functions
		usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);
		wantedPosition = target.position + (target.up * height) + target.rotation * new Vector3(0, 0, -usedDistance);
		currentPosition = transform.position;

		transform.position = Vector3.SmoothDamp(currentPosition, wantedPosition, ref xyzVelocity, 0.01f);

		//Generate custom percentage of speed of ship with custom speed value to camera verticle rotation value
		float speedPercent = movement.GetSpeed() / 80.0f;

		// Look at height of ship related speed percent.
		float lookAtHeightRSP = speedPercent >= 1 ? lookAtHeight : lookAtHeight * speedPercent;
		lookAtVector = new Vector3(0, lookAtHeightRSP, 0);

		transform.LookAt(target.position + lookAtVector, target.up);
	}
}