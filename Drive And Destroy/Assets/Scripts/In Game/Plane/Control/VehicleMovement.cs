//This script handles all of the physics behaviors for the player's ship. The primary functions
//are handling the hovering and thrust calculations. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using EZCameraShake;

public class VehicleMovement : MonoBehaviour
{
	[Header("Ship Status")]
	public float FullHealth = 100.0f;           //The health of ship, if that value be 0 the ship die
	public float DamageDecreasePercent = 1.0f;  //Decrease the taken damage from obstacles.
	public GameObject DeathEffect;
	public GameObject DeathBody;

	private float _currentSpeed;					//The current forward speed of the ship
	private float _health;
	private float _currentTurbo = 100;

	[Header("Drive Settings")]
	public float driveForce = 17f;					//The force that the engine generates
	public float rotationForce = 1.3f;				//The force that the rotation
	//public float slowingVelFactor = .99f;			//The percentage of velocity the ship maintains when not thrusting (e.g., a value of .99 means the ship loses 1% velocity when not thrusting)
	//public float brakingVelFactor = .95f;			//The percentage of velocty the ship maintains when braking
	public float angleOfRoll = 30f;					//The angle that the ship "banks" into a turn
	public float RotationDecraseDueSpeed = 1.0f;    //Rotation speed reduction due to speed

	private float _lerpedRudder;

	[Header("Hover Settings")]
	public float hoverHeight = 1.5f;        //The height the ship maintains when hovering
	public float maxGroundDist = 5f;        //The distance the ship can be above the ground before it is "falling"
	public float hoverForce = 300f;			//The force of the ship's hovering
	public LayerMask whatIsGround;			//A layer mask to determine what layer the ground is on
	public PIDController hoverPID;			//A PID controller to smooth the ship's hovering

	[Header("Physics Settings")]
	public Transform shipBody;				//A reference to the ship's body, this is for cosmetics
	public float terminalVelocity = 100f;   //The max speed the ship can go
	public float hoverGravity = 20f;        //The gravity applied to the ship while it is on the ground
	public float fallGravity = 80f;         //The gravity applied to the ship while it is falling

	[Header("Boost Effects")]
	public GameObject HealBoostEffect;
	public GameObject TurboBoostEffect;

	Rigidbody rigidBody;					//A reference to the ship's rigidbody
	PlayerInput input;						//A reference to the player's input					
	float drag;								//The air resistance the ship recieves in the forward direction
	bool isOnGround;                        //A flag determining if the ship is currently on the ground

	private bool _playerAlive = true;
	private float _oldDriveForce;
	private float _oldTerminalVelocity;
	//Our saved shake instance.
	private bool _isTurboActive = false;
	private Vector3 _terrainPoisiton;


	private CameraShakeInstance _shakeInstance;
	private InGameManager _igm;
	private AudioManager _audioManager;

	void Start()
	{
		//Get references to the Rigidbody and PlayerInput components
		rigidBody = GetComponent<Rigidbody>();
		input = GetComponent<PlayerInput>();
		_igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
		_audioManager = FindObjectOfType<AudioManager>();

		//Calculate the ship's drag value
		drag = driveForce / terminalVelocity;

		//Get first values of drive force and terminal velocity
		_oldDriveForce = driveForce;
		_oldTerminalVelocity = terminalVelocity;

		//We make a single shake instance that we will fade in and fade out when the player enters and leaves the trigger area.
		_shakeInstance = CameraShaker.Instance.StartShake(0.1f, 10.0f, 0.1f);
		//Immediately make the shake inactive.  
		_shakeInstance.StartFadeOut(0);
		//We don't want our shake to delete itself once it stops shaking.
		_shakeInstance.DeleteOnInactive = false;

		_health = FullHealth;

		_terrainPoisiton = _igm.GetActiveTerrainPosition();

		// Fix Turbo button if it's stuck
		CrossPlatformInputManager.SetButtonUp("Turbo");
	}

	void FixedUpdate()
	{
		if (_playerAlive)
        {
			//Calculate the current speed by using the dot product. This tells us
			//how much of the ship's velocity is in the "forward" direction 
			_currentSpeed = Vector3.Dot(rigidBody.velocity, transform.forward);

			//Calculate the forces to be applied to the ship
			CalculatHover();
			CalculatePropulsion();
			TerrainPositionControl();

			if (CrossPlatformInputManager.GetButton("Turbo"))
			{
				if (!_isTurboActive && _currentTurbo > 0)
                {
					TurboActivate();
					_currentTurbo -= Time.deltaTime * 20;
                }else if (_isTurboActive && _currentTurbo > 0)
                {
					_currentTurbo -= Time.deltaTime * 20;
                }else if (_isTurboActive && _currentTurbo <= 0)
                {
					TurboDeactivate();
					_currentTurbo = 0;
				}
			}
			if (!CrossPlatformInputManager.GetButton("Turbo") && _isTurboActive)
			{
				TurboDeactivate();
			}
		}
	}

	void CalculatHover()
	{
		//This variable will hold the "normal" of the ground. Think of it as a line
		//the points "up" from the surface of the ground
		Vector3 groundNormal;

		//Calculate a ray that points straight down from the ship
		Ray ray = new Ray(transform.position, -transform.up);

		//Declare a variable that will hold the result of a raycast
		//Determine if the ship is on the ground by Raycasting down and seeing if it hits 
		//any collider on the whatIsGround layer
		isOnGround = Physics.Raycast(ray, out RaycastHit hitInfo, maxGroundDist, whatIsGround);

		//If the ship is on the ground...
		if (isOnGround)
		{
			//...determine how high off the ground it is...
			float height = hitInfo.distance;
			//...save the normal of the ground...
			groundNormal = hitInfo.normal.normalized;
			//...use the PID controller to determine the amount of hover force needed...
			float forcePercent = hoverPID.Seek(hoverHeight, height);
			
			//...calulcate the total amount of hover force based on normal (or "up") of the ground...
			Vector3 force = groundNormal * hoverForce * forcePercent;
			//...calculate the force and direction of gravity to adhere the ship to the 
			//track (which is not always straight down in the world)...
			Vector3 gravity = -groundNormal * hoverGravity * height;

			//...and finally apply the hover and gravity forces
			rigidBody.AddForce(force, ForceMode.Acceleration);
			rigidBody.AddForce(gravity, ForceMode.Acceleration);
		}
		//...Otherwise...
		else
		{
			//...use Up to represent the "ground normal". This will cause our ship to
			//self-right itself in a case where it flips over
			groundNormal = Vector3.up;

			//Calculate and apply the stronger falling gravity straight down on the ship
			Vector3 gravity = -groundNormal * fallGravity;
			rigidBody.AddForce(gravity, ForceMode.Acceleration);
		}

		//Calculate the amount of pitch and roll the ship needs to match its orientation
		//with that of the ground. This is done by creating a projection and then calculating
		//the rotation needed to face that projection
		Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
		Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

		//Move the ship over time to match the desired rotation to match the ground. This is 
		//done smoothly (using Lerp) to make it feel more realistic
		rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, rotation, Time.deltaTime * 10f));

		//Calculate the angle we want the ship's body to bank into a turn based on the current rudder.
		//It is worth noting that these next few steps are completetly optional and are cosmetic.
		//It just feels so darn cool
		float angle = angleOfRoll * -input.rudder;

		//Calculate the rotation needed for this new angle
		Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);
		//Finally, apply this angle to the ship's body
		shipBody.rotation = Quaternion.Lerp(shipBody.rotation, bodyRotation, Time.deltaTime * 8f);
	}

	void CalculatePropulsion()
	{
		// Decrease rotationForce while player speed incresing
		// Max Rotation Force is base Rotation Force
		// Min Rotation Force is half of the base Rotation Force
		// If Turbo is activated then decrase Rotation Force extra %25
		float speedPercantage = GetSpeedPercentage();
		float decraseValue = speedPercantage >= 0 ? (speedPercantage * RotationDecraseDueSpeed) + 1 : 1;
		float calculatedRotationForce = rotationForce / decraseValue;
		//_calculatedRotationForce = _isTurboActive ? _calculatedRotationForce * 0.75f : _calculatedRotationForce;
		//Debug.Log("GSP: " + GetSpeedPercentage() + " --- DV: " + _decraseValue + " --- CRF: " + _calculatedRotationForce);

		//Calculate the rudder for the soft rotate (Exclude Tilt vehicle control type)
		_lerpedRudder = input.isAccelerationControlActive ? input.rudder : Mathf.Lerp(_lerpedRudder, input.rudder, Time.deltaTime * 8f);
		//Calculate the yaw torque based on the rudder and current angular velocity
		//float rotationTorque = input.rudder - rigidBody.angularVelocity.y;
		float rotationTorque = _currentSpeed > 0 && input.thruster < 0
			? _lerpedRudder * (calculatedRotationForce * 2) // Breaking rotate faster
			: _lerpedRudder * calculatedRotationForce; // No breake rotate due of algorithm
		//Apply the torque to the ship's Y axis
		rigidBody.AddRelativeTorque(0f, (_currentSpeed < 1 && input.thruster < 0 ? -rotationTorque : rotationTorque), 0f, ForceMode.VelocityChange);

		//Calculate the current sideways speed by using the dot product. This tells us
		//how much of the ship's velocity is in the "right" or "left" direction
		float sidewaysSpeed = Vector3.Dot(rigidBody.velocity, transform.right);

		//Calculate the desired amount of friction to apply to the side of the vehicle. This
		//is what keeps the ship from drifting into the walls during turns. If you want to add
		//drifting to the game, divide Time.fixedDeltaTime by some amount
		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime); 

		//Finally, apply the sideways friction
		rigidBody.AddForce(sideFriction, ForceMode.Acceleration);

		//If not propelling the ship, slow the ships velocity
		/*if (input.thruster <= 0f)
			rigidBody.velocity *= slowingVelFactor;*/

		//Braking or driving requires being on the ground, so if the ship
		//isn't on the ground, exit this method
		if (!isOnGround)
			return;

		//If the ship is braking, apply the braking velocty reduction
		/*if (input.isBraking)
			rigidBody.velocity *= brakingVelFactor;*/

		//Calculate and apply the amount of propulsion force by multiplying the drive force
		//by the amount of applied thruster and subtracting the drag amount
		/*float propulsion = _currentSpeed > terminalVelocity && !_isTurboActive
			? driveForce * (input.thruster / 2) - drag * Mathf.Clamp(_currentSpeed, 0f, terminalVelocity) 
			: driveForce * input.thruster - drag * Mathf.Clamp(_currentSpeed, 0f, terminalVelocity);*/
		float propulsion = _currentSpeed > terminalVelocity
			? driveForce * (input.thruster / 2) - drag * Mathf.Clamp(_currentSpeed, 0f, terminalVelocity)
			: _currentSpeed < 0
				? driveForce * input.thruster - drag * Mathf.Clamp(_currentSpeed, -terminalVelocity, 0f) * 5
				: driveForce * input.thruster - drag * Mathf.Clamp(_currentSpeed, 0f, terminalVelocity);
		/*Debug.Log("TM: " + terminalVelocity);
		Debug.Log("MC: " + Mathf.Clamp(speed, 0f, terminalVelocity));
		Debug.Log("PR: " + propulsion);
		Debug.Log("TF: " + transform.forward);*/

		rigidBody.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
	}

	public void TurboActivate()
    {
		//Debug.Log("Turbo Activated!!");
		_isTurboActive = true;

		terminalVelocity = _oldTerminalVelocity * 1.3f;
		driveForce = _oldDriveForce * 3.0f;

		// Calculate the ship's drag value
		drag = driveForce / terminalVelocity;

		// Activate the camera shake
		_shakeInstance.StartFadeIn(1f);

		//float timeOut = 5.0f;
		//StartCoroutine(TurboDeactivate(oldTerminalVelocity, oldDriveForce, timeOut));
		_currentTurbo -= Time.deltaTime * 20;
		
	}

	public void TurboDeactivate()
	{
		//Debug.Log("Turbo Deactivated!!..");
		_isTurboActive = false;
		terminalVelocity = _oldTerminalVelocity;
		driveForce = _oldDriveForce;

		//Calculate the ship's drag value
		drag = driveForce / terminalVelocity;

		// Deactivate the camera shake
		_shakeInstance.StartFadeOut(1f);
	}
	
	/*IEnumerator TurboDeactivate(float oldTerminalVelocity, float oldDriveForce, float timeOut)
	{
		// Wait for X second
		yield return new WaitForSeconds(timeOut);

		Debug.Log("Turbo Deactivated!!..");

		isTurboActive = false;
		terminalVelocity = oldTerminalVelocity;
		driveForce = oldDriveForce;

		//Calculate the ship's drag value
		drag = driveForce / terminalVelocity;

		// Deactivate the camera shake
		_shakeInstance.StartFadeOut(3f);
	}*/

	private void OnCollisionEnter(Collision collision)
    {
		if (_currentSpeed > 5)
		{
			float customCrashSoundVolume = 0.4f;
			if (_currentSpeed < 25)
			{
				customCrashSoundVolume = 0.05f;
			}
			else if (_currentSpeed < 50)
			{
				customCrashSoundVolume = 0.1f;
			}
			else if (_currentSpeed < 75)
			{
				customCrashSoundVolume = 0.2f;
			}
			_audioManager.Play("CarCrash", customCrashSoundVolume);
		}

		//If the ship has collided with an object on the Barricade layer...
		if (collision.gameObject.layer == LayerMask.NameToLayer("Barricade"))
		{
			//...calculate how much upward impulse is generated and then push the vehicle down by that amount 
			//to keep it stuck on the track (instead up popping up over the barricade)
			Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
			rigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);

			BlockBarricade barricade = collision.gameObject.GetComponent<BlockBarricade>();
			if (barricade.IsBreakable)
            {
				if (_currentSpeed > 25)
				{
					float barricadeFullHealth = barricade.FullHealth;
					float barricadeHealth = barricade.GetHealth();
					int barricadeLevel = barricade.BarricadeLevel;
					int fullDamageFromBarricade = 30;
					switch (barricadeLevel)
                    {
						case 1:
							fullDamageFromBarricade = 30;
							break;
						case 2:
							fullDamageFromBarricade = 50;
							break;
						case 3:
							fullDamageFromBarricade = 70;
							break;
						default:
							fullDamageFromBarricade = 70;
							break;
					}

					if (_currentSpeed < 50)
                    {
						barricade.TakeDamage(barricadeFullHealth * 0.5f);
					}
					else if (_currentSpeed < 70)
                    {
						barricade.TakeDamage(barricadeFullHealth * 0.75f);
					}
					else
                    {
						barricade.TakeDamage(barricadeFullHealth);
					}

					TakeDamage(fullDamageFromBarricade * (barricadeHealth / barricadeFullHealth)); // Ship take less damage if barricade health is low.
				}
				else
				{
					// Barricade doesn't take damage because the ship is so slow.
					TakeDamage(_currentSpeed);
				}
			} else
            {
				if (barricade.BarricadeLevel == 4)
				{
					TakeDamage(1000);
				}else
                {
					if (_currentSpeed > 25 && _currentSpeed < 50)
					{
						TakeDamage(30);
					}
					else if (_currentSpeed > 25 && _currentSpeed < 70)
					{
						TakeDamage(60);
					}
					else if (_currentSpeed > 25)
					{
						TakeDamage(1000);
					}
				}
			}
		}

		//If the ship has collided with an object on the Wall layer...
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			Vector3 sideForceFromCollision = Vector3.Dot(collision.impulse, transform.right) * transform.right;
			rigidBody.AddForce(sideForceFromCollision * 1.2f, ForceMode.Impulse);
		}
	}

    void OnCollisionStay(Collision collision)
	{
		//If the ship has collided with an object on the Wall layer...
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			//...calculate how much upward impulse is generated and then push the vehicle down by that amount 
			//to keep it stuck on the track (instead up popping up over the wall)
			Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
			rigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);

			Vector3 sideForceFromCollision = Vector3.Dot(collision.impulse, transform.right) * transform.right;
			rigidBody.AddForce(sideForceFromCollision * 1.2f, ForceMode.Impulse);

			if (_currentSpeed > 5) { 
				float rotationTorque = 3f;
				rigidBody.AddRelativeTorque(0f, (collision.gameObject.name == "Left Wall" ? rotationTorque : -rotationTorque), 0f, ForceMode.VelocityChange);
			}
		}
	}

    public void TakeDamage(float damage)
    {
		if (_health > 0)
		{
			damage *= DamageDecreasePercent;
			_health -= damage;
			if (_health <= 0)
			{
				DeathEffectApply();
			}
		}
    }

	public float GetSpeedPercentage()
	{
		//Returns the total percentage of speed the ship is traveling
		//return rigidBody.velocity.magnitude / terminalVelocity;
		return _currentSpeed / _oldTerminalVelocity;
	}
	public bool GetTurboStatus()
	{
		//Returns the Turbo activate status
		return _isTurboActive;
	}
	public float GetTurbo()
	{
		//Returns the Turbo value
		return _currentTurbo;
	}
	public void AddTurboPoint(int addedTurboValue)
	{
		//Add Turbo value
		float tempTurboValue = _currentTurbo + addedTurboValue;
		_currentTurbo = Mathf.Clamp(tempTurboValue, 0, 100);
		TurboBoostEffect.SetActive(true);
	}
	public float GetSpeed()
	{
		//Returns the fixed Current Speed value. (Speed always be absolute)
		float fixedCurrentSpeed = _currentSpeed < 0.1f && _currentSpeed > -0.1f ? 0 : Mathf.Abs(_currentSpeed);
		return fixedCurrentSpeed;
	}
	public float GetHealth()
	{
		//Returns the Health value
		float fixedHealth = Mathf.Clamp(_health, 0, 999);
		return fixedHealth;
	}
	public void AddHealth(int addedHealthValue)
	{
		//Add Health value
		float tempHealthValue = _health + addedHealthValue;
		_health = Mathf.Clamp(tempHealthValue, 0, 100);
		HealBoostEffect.SetActive(true);
	}

	private void DeathEffectApply()
	{
		_playerAlive = false;
		_currentSpeed = 0;

		if (_isTurboActive)
        {
			TurboDeactivate();
        }

		if (DeathEffect != null)
		{
			GameObject crashEffectPS = Instantiate(DeathEffect, transform.position, transform.rotation);
			Destroy(crashEffectPS, 1);
			CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1.5f);

			shipBody.gameObject.SetActive(false);
			DeathBody.SetActive(true);
		}

		_igm.GameOver();
	}

	private void TerrainPositionControl()
    {
		Vector3 playerPos = this.gameObject.transform.position;
		if (playerPos.x < _terrainPoisiton.x + 500 || playerPos.x > _terrainPoisiton.x + 4500)
        {
			_terrainPoisiton.x = playerPos.x - 2500;
			_igm.ChangeTerrainPosition(_terrainPoisiton);
        }

		if (playerPos.z < _terrainPoisiton.z + 500 || playerPos.z > _terrainPoisiton.z + 4500)
        {
			_terrainPoisiton.z = playerPos.z - 2500;
			_igm.ChangeTerrainPosition(_terrainPoisiton);
		}
    }
}
