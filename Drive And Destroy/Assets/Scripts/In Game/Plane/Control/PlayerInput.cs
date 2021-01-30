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
	public string brakingKey = "Brake";                 //The name of the brake button

	//We hide these in the inspector because we want 
	//them public but we don't want people trying to change them
	[HideInInspector] public float thruster;						//The current thruster value
	[HideInInspector] public float rudder;							//The current rudder value
	[HideInInspector] public bool isBraking;						//The current brake value
	[HideInInspector] public bool isReverseRotate;					//The current reverse rotate effect status

	
	private bool _isVehicleControlTypeChecked;
	private bool _isAccelerationControlActive;      //The current Acceleration rotate status
	private float _xAtStart;

    private void Start()
    {
		CheckPlayerControlType();
	}

    void Update()
	{
		//If the player presses the Escape key and this is a build (not the editor), exit the game
		if (CrossPlatformInputManager.GetButtonDown("Cancel") && !Application.isEditor)
			Application.Quit();

		if (!_isVehicleControlTypeChecked)
        {
			CheckPlayerControlType();
		}

		//If a GameManager exists and the game is not active...
		if (GameManager.instance != null && !GameManager.instance.IsActiveGame())
		{
			//...set all inputs to neutral values and exit this method
			thruster = rudder = 0f;
			isBraking = false;
			return;
		}

		//Get the values of the thruster, rudder, and brake from the input class
		thruster = CrossPlatformInputManager.GetAxis(verticalAxisName);
		rudder = _isAccelerationControlActive ? Mathf.Clamp(Input.acceleration.x - _xAtStart, -1f, 1f) : CrossPlatformInputManager.GetAxis(horizontalAxisName);
		if (_isAccelerationControlActive)
        {
			rudder = rudder < 0.05f && rudder > -0.05f ? 0 : rudder * 3;
        }
		rudder = isReverseRotate ? rudder * (-1) : rudder;
		//isBraking = CrossPlatformInputManager.GetButton(brakingKey);
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
			_isAccelerationControlActive = true;
			ResetAccelerationStartPoint();
		}
		_isVehicleControlTypeChecked = true;
	}
}
