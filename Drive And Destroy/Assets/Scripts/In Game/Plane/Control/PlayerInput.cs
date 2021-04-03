//This script handles reading inputs from the player and passing it on to the vehicle. We 
//separate the input code from the behaviour code so that we can easily swap controls 
//schemes or even implement and AI "controller". Works together with the VehicleMovement script

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/*
 * For Mobile --> CrossPlatformInputManager
 * For Desktop --> Input
 */
public class PlayerInput : MonoBehaviour
{
	public string verticalAxisName = "Vertical";        //The name of the thruster axis
	public string horizontalAxisName = "Horizontal";    //The name of the rudder axis

	//We hide these in the inspector because we want 
	//them public but we don't want people trying to change them
	[HideInInspector] public float thruster;						//The current thruster value
	[HideInInspector] public float rudder;							//The current rudder value
	[HideInInspector] public bool isReverseRotate;                  //The current reverse rotate effect status
	[HideInInspector] public bool isAccelerationControlActive;      //The current Acceleration rotate status

	
	private bool _isVehicleControlTypeChecked;
	private float _xAtStart;

    private void Start()
    {
		CheckPlayerControlType();
	}

    void Update()
	{
		if (!_isVehicleControlTypeChecked)
        {
			CheckPlayerControlType();
		}

		//Get the values of the thruster, rudder, and brake from the input class
		thruster = CrossPlatformInputManager.GetAxis(verticalAxisName);
		rudder = isAccelerationControlActive ? Mathf.Clamp(Input.acceleration.x - _xAtStart, -1f, 1f) : CrossPlatformInputManager.GetAxis(horizontalAxisName);
		if (isAccelerationControlActive)
        {
			rudder = rudder < 0.05f && rudder > -0.05f ? 0 : rudder * 4;
        }
		rudder = isReverseRotate ? rudder * (-1) : rudder;
	}

	public void ResetAccelerationStartPoint()
    {
		_xAtStart = Input.acceleration.x;
	}

	private void CheckPlayerControlType()
    {
		int vehicleControlTypeId = GameManager.instance.pd.vehicleControlType;
		if (vehicleControlTypeId == 1)
		{
			isAccelerationControlActive = true;
			ResetAccelerationStartPoint();
		}
		_isVehicleControlTypeChecked = true;
	}
}
