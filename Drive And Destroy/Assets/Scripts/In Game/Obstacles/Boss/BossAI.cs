using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BossAI : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject BossBody;
    public int BossFullHealth = 500;
    public int BossLevel = 1;
    public GameObject CrashEffect;
    public bool IsSpecialColor = false;
    public Color SpecialColor;

    [SerializeField]
    private int _health = 500;
    private GameObject _targetObject;
    private Rigidbody _bossRigidbody;
    private InGameManager _igm;
    private AudioManager _audioManager;

    [Header("Drive Settings")]
    public float SpeedAcceleration = 1.0f;
    public float DeviationSpeed = 3;
    public float MaxSpeed = 150;
    public float MinSpeed = 30;
    public float rotateAcceleration = 2.0f;

    // Speed Controls
    private float _currentSpeed = 0;
    private float _targetSpeed = 50;

    // Rotation Controls
    private int _targetDistanceFromLeftWall = 10;
    private int _rotateSpeedMod = 0;
    private float _rotateTimeLapsForMode0 = 5.0f;
    private float _rotateTimeLapsForMode1 = 4.0f;
    private float _rotateTimeLapsForMode2 = 3.0f;
    private float _rotateTimeLapsForMode3 = 1.5f;

    [Header("Weapon Settings")]
    public GameObject ThrowPoint;
    public GameObject ThrowingObject;
    public GameObject LaserWarning;
    public GameObject LaserFire;
    public float LaserCameraShakeMagnitude = 1.0f;
    public float LaserCameraShakeRoughness = 12.0f;

    //Our saved shake instance.
    private CameraShakeInstance _shakeInstance;


    // Start is called before the first frame update
    void Start()
    {
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _audioManager = FindObjectOfType<AudioManager>();
        _targetObject = GameObject.FindGameObjectWithTag("Player");
        _bossRigidbody = this.GetComponent<Rigidbody>();
        _health = BossFullHealth;
        _igm.IsBossAlive = true;
        _igm.BossFullHealth = BossFullHealth;
        _igm.BossCurrentHealth = _health;

        InvokeRepeating("RotatePositionChange", 0.5f, _rotateTimeLapsForMode0);

        //We make a single shake instance that we will fade in and fade out when the player enters and leaves the trigger area.
        _shakeInstance = CameraShaker.Instance.StartShake(LaserCameraShakeMagnitude, LaserCameraShakeRoughness, 2);
        //Immediately make the shake inactive.  
        _shakeInstance.StartFadeOut(0);
        //We don't want our shake to delete itself once it stops shaking.
        _shakeInstance.DeleteOnInactive = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SpeedControl();
        RotateControl();
    }

    public void TakeDamage(float damage)
    {
        if (_health > 0)
        {

            _health -= Mathf.FloorToInt(damage);
            _igm.BossCurrentHealth = _health;

            if (_health <= 0)
            {
                _igm.IsBossAlive = false;
                _igm.IsBossActivated = false;
                _igm.IsNextLevelTime = true;
                _igm.IncreaseReachedLevel();

                // Destroy the Laser Gun Effect if laser gun active when Boss die.
                LaserWarning.SetActive(false);
                LaserFire.SetActive(false);
                // Deactivate the camera shake
                _shakeInstance.StartFadeOut(0.1f);

                // Destroy all boss object
                BossBody.SetActive(false);
                Destroy(this.gameObject, 0.5f);
                CrashEffectApply();

                switch (BossLevel)
                {
                    case 1:
                        _igm.AddPlayerPoint(150);
                        break;
                    case 2:
                        _igm.AddPlayerPoint(200);
                        break;
                    case 3:
                        _igm.AddPlayerPoint(300);
                        break;
                    default:
                        _igm.AddPlayerPoint(300);
                        break;
                }
            }else
            {
                // Default value is 0
                float onePercentOfFullHeal = BossFullHealth / 100;
                if (_health < onePercentOfFullHeal * 10)
                {
                    _rotateSpeedMod = 3;
                }
                else if (_health < onePercentOfFullHeal * 30)
                {
                    _rotateSpeedMod = 2;
                }
                else if (_health < onePercentOfFullHeal * 60)
                {
                    _rotateSpeedMod = 1;
                }
            }
        }
    }

    public float GetHealth()
    {
        return _health;
    }

    private void CrashEffectApply()
    {
        if (CrashEffect != null)
        {
            GameObject crashEffectPS = Instantiate(CrashEffect, transform.position, transform.rotation);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1.5f);
            if (IsSpecialColor)
            {
                crashEffectPS.GetComponent<Renderer>().material.color = SpecialColor;
            }
        }
    }

    private void SpeedControl()
    {
        float dist = Vector3.Distance(this.transform.position, _targetObject.transform.position);
        //print("Distance --> " + dist);
        //Debug.DrawLine(this.transform.position, _targetObject.transform.position, Color.red);
        
        _currentSpeed = Vector3.Dot(_bossRigidbody.velocity, transform.forward);
        //print("Speed --> " + _currentSpeed);

        if (dist < 30)
        {
            _targetSpeed = MaxSpeed;
        }else if(dist > 40 && dist < 80)
        {
            _targetSpeed = MaxSpeed / 2;
        }else if(dist > 150 && dist < 200)
        {
            _targetSpeed = MaxSpeed / 3;
        }else if (dist > 200)
        {
            _targetSpeed = MinSpeed;
        }

        if (_currentSpeed < _targetSpeed - DeviationSpeed)
        {
            _bossRigidbody.velocity += this.transform.forward * SpeedAcceleration;
        }
        else if (_currentSpeed > _targetSpeed + DeviationSpeed)
        {
            _bossRigidbody.velocity -= this.transform.forward * SpeedAcceleration;
        }
    }

    private void RotateControl()
    {
        Physics.Raycast(transform.position, -transform.right, out RaycastHit hitInfo, 25.0f, 1 << 9);
        Debug.DrawRay(transform.position, -transform.right * hitInfo.distance, Color.red);

        float currentRotateSpeed = Vector3.Dot(_bossRigidbody.velocity, transform.right);
        float expectedRotateSpeed = (_targetDistanceFromLeftWall - hitInfo.distance) / 0.5f;
        //print(hitInfo.distance + " - " + currentRotateSpeed + " - " + expectedRotateSpeed);

        if (expectedRotateSpeed > -1 && expectedRotateSpeed < 1)
        {
            _bossRigidbody.velocity -= this.transform.right * currentRotateSpeed;
        }else
        {
            if (currentRotateSpeed < expectedRotateSpeed)
            {
                _bossRigidbody.velocity += this.transform.right * rotateAcceleration;
            }
            else if (currentRotateSpeed > expectedRotateSpeed)
            {
                _bossRigidbody.velocity -= this.transform.right * rotateAcceleration;
            }
        }

        //Calculate the angle we want the ship's body to bank into a turn based on the current rudder.
        //It is worth noting that these next few steps are completetly optional and are cosmetic.
        //It just feels so darn cool
        float angleOfRoll = 30;
        float slidingSpeed = currentRotateSpeed != 0 ? currentRotateSpeed / 6 : 0;
        slidingSpeed = Mathf.Clamp(slidingSpeed, -1, 1);
        float angle = angleOfRoll * -slidingSpeed;

        //Calculate the rotation needed for this new angle
        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);
        //Finally, apply this angle to the ship's body
        BossBody.transform.rotation = Quaternion.Lerp(BossBody.transform.rotation, bodyRotation, Time.deltaTime * 6f);

        //print(hitInfo.collider.gameObject.layer + " -> " + hitInfo.distance + " --- " + sideSpeed);
        //print(_bossRigidbody.velocity.x + " - " + _bossRigidbody.velocity.y + " - " + _bossRigidbody.velocity.z);
    }

    private void RotatePositionChange()
    {
        
        int rotateDistanceMinLimit = 3;

        int randomWallDistance = Random.Range(2, 19);
        if (Mathf.Abs(_targetDistanceFromLeftWall - randomWallDistance) < rotateDistanceMinLimit)
        {
            if (_targetDistanceFromLeftWall < 5)
            {
                randomWallDistance = _targetDistanceFromLeftWall + 3;
            }else if (_targetDistanceFromLeftWall > 15)
            {
                randomWallDistance = _targetDistanceFromLeftWall - 3;
            }else
            {
                if (randomWallDistance < _targetDistanceFromLeftWall)
                {
                    randomWallDistance -= Mathf.Abs(_targetDistanceFromLeftWall - randomWallDistance);
                }else if (randomWallDistance > _targetDistanceFromLeftWall)
                {
                    randomWallDistance += Mathf.Abs(_targetDistanceFromLeftWall - randomWallDistance);
                } else
                {
                    randomWallDistance += rotateDistanceMinLimit;
                }
            }
        }
        randomWallDistance = Mathf.Clamp(randomWallDistance, 2, 18);
        // print("RotatePositionChange --> " + randomWallDistance);
        _targetDistanceFromLeftWall = randomWallDistance;

        AttackToPlayer();

        if (_rotateSpeedMod == 1)
        {
            CancelInvoke("RotatePositionChange");
            InvokeRepeating("RotatePositionChange", _rotateTimeLapsForMode1, _rotateTimeLapsForMode1);
        }
        if (_rotateSpeedMod == 2)
        {
            CancelInvoke("RotatePositionChange");
            InvokeRepeating("RotatePositionChange", _rotateTimeLapsForMode2, _rotateTimeLapsForMode2);
        }
        if (_rotateSpeedMod == 3)
        {
            CancelInvoke("RotatePositionChange");
            InvokeRepeating("RotatePositionChange", _rotateTimeLapsForMode3, _rotateTimeLapsForMode3);
        }
    }

    private void AttackToPlayer()
    {
        int random = UnityEngine.Random.Range(0, 101);
        if (random < 25)
        {
            StartCoroutine(FireTheLaser(1f));
        }else
        {
            ThrowTheBarrell();
        }
    }

    private void ThrowTheBarrell()
    {
        Vector3 throwPoint = ThrowPoint.transform.position;
        Quaternion throwRotation = ThrowPoint.transform.rotation;
        GameObject obj = Instantiate(ThrowingObject, throwPoint, throwRotation);
        obj.GetComponent<Rigidbody>().velocity = _bossRigidbody.velocity - (transform.forward * 10);

        Destroy(obj, 10);
    }

    IEnumerator FireTheLaser(float timeOut)
    {
        LaserWarning.SetActive(true);
        _audioManager.Play("BossLaserWarmup");
        // Wait for X second
        yield return new WaitForSeconds(timeOut);

        if (_health > 0)
        {
            LaserFire.SetActive(true);
            _audioManager.Play("BossLaserShoot");
            // Activate the camera shake
            _shakeInstance.StartFadeIn(0.1f);

            StartCoroutine(StopTheLaser(3f));
        }

    }
    IEnumerator StopTheLaser(float timeOut)
    {
        // Wait for X second
        yield return new WaitForSeconds(timeOut);

        _audioManager.Stop("BossLaserShoot");
        LaserWarning.SetActive(false);
        LaserFire.SetActive(false);
        // Deactivate the camera shake
        _shakeInstance.StartFadeOut(0.5f);
    }
}
