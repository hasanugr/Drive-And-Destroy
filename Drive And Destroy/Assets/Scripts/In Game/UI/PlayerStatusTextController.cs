using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusTextController : MonoBehaviour
{
    [Header("Health Status")]
    public Image HealthBar;
    public Color LowHealColor;
    public Color HighHealColor;
    public Image DamagedEffectImage;

    private float _fullHealth;
    private float _currentHealth;
    private float _healthBarValue;
    private float _lastCurrentHealth;

    [Header("Speed Status")]
    public TextMeshProUGUI SpeedText;
    public Image SpeedBar;
    public int TopSpeedOnBar = 150;
    public Color LowSpeedColor;
    public Color HighSpeedColor;

    private int _currentSpeed;
    private float _speedBarValue;

    [Header("Turbo Status")]
    public Image[] TurboBars;
    public int TopTurboLimitOnBar = 100;
    public Color LowTurboColor;
    public Color HighTurboColor;

    private Image _activeTurboBar;
    private int _currentTurbo;
    private float _turboBarValue;

    [Header("Boss Status")]
    public GameObject BossBarHolder;
    public Image BossBar;

    private int _bossFullHealth;
    private int _bossCurrentHealth;
    private float _bossBarValue;

    [Header("Player Point Status")]
    public TextMeshProUGUI PointUI;

    private int _playerPoint;

    [Header("Player Gold Status")]
    public TextMeshProUGUI GoldUI;
    public GameObject GoldIcon;

    private int _playerGold;

    [Header("Countdown Virus")]
    public GameObject VirusHolder;
    public Image VirusBar;
    public TextMeshProUGUI VirusUI;
    public GameObject MyopicVirus;
    public GameObject EarthquakeVirus;
    public GameObject ReverseMovementVirus;

    private float _virusCountdown;
    private float _virusCountdownMax;
    private float _virusCountdownBarValue;
    private bool _isVirusActive;


    [Header("Countdown Timer")]
    public GameObject TimerHolder;
    public float DamagePerSecond = 20;
    public float TimerMaxValue = 30;
    public TextMeshProUGUI TimerUI;
    public GameObject TimerRotatingIconOutside;
    public GameObject TimerRotatingIconInside;

    private float _timerCountdown;
    private float _timerDamageCount;
    private int _timerLastValue;


    private GameObject _ship;
    private VehicleMovement _vehicleMovement;
    private InGameManager _igm;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _ship = GameObject.FindWithTag("Player");
        _vehicleMovement = _ship.GetComponent<VehicleMovement>();
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
        _audioManager = FindObjectOfType<AudioManager>();

        ResetTheCounter();
        CountdownTimerRotateMoveStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HealthBarProccess();
        SpeedBarProccess();
        TurboBarProccess();
        BossBarProccess();
        PlayerPointProccess();
        PlayerGoldProccess();
        CountdownVirusProccess();
        CountdownTimerProccess();
    }

    public void ResetTheCounter()
    {
        LeanTween.scale(TimerHolder, new Vector3(1.5f, 1.5f, 1f), 1f).setEasePunch();
        _timerCountdown = TimerMaxValue;
        TimerUI.text = _timerCountdown.ToString();
    }

    public void ActivateTheVirusCounter(string virusType, float virusDuration)
    {
        switch (virusType)
        {
            case "Myopic":
                MyopicVirus.SetActive(true);
                break;
            case "Earthquake":
                EarthquakeVirus.SetActive(true);
                break;
            case "ReverseMovement":
                ReverseMovementVirus.SetActive(true);
                break;
        }
        _virusCountdown = virusDuration;
        _virusCountdownMax = _virusCountdown;
        VirusUI.text = _virusCountdown.ToString();
        VirusHolder.SetActive(true);
        _isVirusActive = true;
        LeanTween.scale(VirusHolder, new Vector3(1.5f, 1.5f, 1f), 1f).setEasePunch();
    }



    private void HealthBarProccess()
    {
        if (_fullHealth <= 0)
        {
            _fullHealth = _vehicleMovement.FullHealth;
        }

        _currentHealth = Mathf.FloorToInt(_vehicleMovement.GetHealth());
        if (_currentHealth != _lastCurrentHealth)
        {
            bool isGetDamaged = _currentHealth < _lastCurrentHealth;
            _lastCurrentHealth = _currentHealth;
            _healthBarValue = (float)_currentHealth / _fullHealth;

            HealthBar.fillAmount = _healthBarValue;
            Color lerpedColor = Color.Lerp(LowHealColor, HighHealColor, _healthBarValue);
            HealthBar.color = lerpedColor;
            if (isGetDamaged)
            {
                // Get Damaged Effect
                Color tempColor = DamagedEffectImage.color;
                tempColor.a = 0.3f;
                DamagedEffectImage.color = tempColor;
                LeanTween.value(gameObject, 0.3f, 0, 0.5f).setOnUpdate((float val) =>
                {
                    tempColor.a = val;
                    DamagedEffectImage.color = tempColor;
                });
            }
        }
    }

    private void SpeedBarProccess()
    {
        _currentSpeed = Mathf.FloorToInt(_vehicleMovement.GetSpeed());
        _speedBarValue = (float)_currentSpeed / TopSpeedOnBar;

        SpeedText.text = (_currentSpeed * 2).ToString();
        SpeedBar.fillAmount = _speedBarValue;

        Color lerpedColor = Color.Lerp(LowSpeedColor, HighSpeedColor, _speedBarValue);
        SpeedBar.color = lerpedColor;
    }

    private void TurboBarProccess()
    {
        if (!_activeTurboBar)
        {
            for (int i = 0; i < TurboBars.Length; i++)
            {
                if (TurboBars[i].gameObject.transform.parent.parent.gameObject.activeSelf)
                {
                    _activeTurboBar = TurboBars[i];
                }
            }
        }


        int newTurboValue = Mathf.FloorToInt(_vehicleMovement.GetTurbo());
        if (newTurboValue != _currentTurbo)
        {
            _currentTurbo = newTurboValue;
            _turboBarValue = (float)_currentTurbo / TopTurboLimitOnBar;

            _activeTurboBar.fillAmount = _turboBarValue;

            Color lerpedColor = Color.Lerp(LowTurboColor, HighTurboColor, _turboBarValue);
            _activeTurboBar.color = lerpedColor;
        }
    }

    private void BossBarProccess()
    {
        if (_igm.IsBossActivated && _igm.IsBossAlive)
        {
            if (!BossBarHolder.activeSelf && _bossCurrentHealth > 0)
            {
                BossBarHolder.SetActive(true);
            }

            if (_bossFullHealth <= 0)
            {
                _bossFullHealth = _igm.BossFullHealth;
            }
            _bossCurrentHealth = _igm.BossCurrentHealth;
            _bossBarValue = (float)_bossCurrentHealth / _bossFullHealth;

            BossBar.fillAmount = _bossBarValue;
        }
        else if (BossBarHolder.activeSelf)
        {
            BossBarHolder.SetActive(false);
            _bossFullHealth = 0;
        }
    }

    private void PlayerPointProccess()
    {
        int newPointValue = _igm.GetPlayerPoint();
        if (newPointValue > _playerPoint)
        {
            _playerPoint = newPointValue;
            PointUI.text = _playerPoint.ToString();
        }
    }

    private void PlayerGoldProccess()
    {
        int newGoldValue = _igm.GetCollectedGold();
        if (newGoldValue > _playerGold)
        {
            LeanTween.scale(GoldIcon, new Vector3(1.5f, 1.5f, 1), 0.5f).setEasePunch();
            _playerGold = newGoldValue;
            GoldUI.text = _playerGold.ToString();
        }
    }
    
    private void CountdownVirusProccess()
    {
        if (_igm.GameIsStarted && _isVirusActive)
        {
            if (_virusCountdown > 0)
            {
                _virusCountdown -= Time.deltaTime;
                _virusCountdown = Mathf.Clamp(_virusCountdown, 0, _virusCountdownMax);
                VirusUI.text = Mathf.CeilToInt(_virusCountdown).ToString();

                _virusCountdownBarValue = _virusCountdown / _virusCountdownMax;
                VirusBar.fillAmount = _virusCountdownBarValue;
            }
            else
            {
                _isVirusActive = false;
                MyopicVirus.SetActive(false);
                EarthquakeVirus.SetActive(false);
                ReverseMovementVirus.SetActive(false);
                VirusHolder.SetActive(false);
            }
        }
    }
    
    private void CountdownTimerProccess()
    {
        if (_igm.GameIsStarted)
        {
            if (_timerCountdown > 0)
            {
                _timerCountdown -= Time.deltaTime;
                _timerCountdown = Mathf.Clamp(_timerCountdown, 0, TimerMaxValue);
                int timerInt = Mathf.CeilToInt(_timerCountdown);
                if (_timerLastValue != timerInt)
                {
                    TimerUI.text = timerInt.ToString();
                    float difTimerValues = _timerLastValue - timerInt;
                    if (difTimerValues == 1 && _timerCountdown <= 3)
                    {
                        _audioManager.PlayOneShot("TimesUpBeep");
                    }
                    _timerLastValue = timerInt;
                }
            }
            else
            {
                _timerCountdown -= Time.deltaTime;
                if ((_timerDamageCount - _timerCountdown) >= 1)
                {
                    _timerDamageCount = _timerCountdown;
                    _audioManager.PlayOneShot("TimesUpDamage");
                    _vehicleMovement.TakeDamage(DamagePerSecond);
                }
            }
        }
    }

    private void CountdownTimerRotateMoveStart()
    {
        LeanTween.rotateAroundLocal(TimerRotatingIconInside, Vector3.forward, 360f, 7f).setRepeat(-1);
        LeanTween.rotateAroundLocal(TimerRotatingIconOutside, Vector3.forward, -360f, 6f).setRepeat(-1);
    }
}
