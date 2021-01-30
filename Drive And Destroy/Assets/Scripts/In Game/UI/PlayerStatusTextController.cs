using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusTextController : MonoBehaviour
{
    [Header("Health Status")]
    public GameObject HealthBarDotsHolder;
    public Color LowHealColor;
    public Color HighHealColor;

    [SerializeField]
    private GameObject[] _healthBarDots = new GameObject[49];
    private float _fullHealth;
    private float _currentHealth;
    private float _healthBarValue;

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


    private GameObject _ship;
    private VehicleMovement _vehicleMovement;
    private InGameManager _igm;

    // Start is called before the first frame update
    void Start()
    {
        _ship = GameObject.FindWithTag("Player");
        _vehicleMovement = _ship.GetComponent<VehicleMovement>();
        _igm = GameObject.Find("In Game Manager").GetComponent<InGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarProccess();
        SpeedBarProccess();
        TurboBarProccess();
        BossBarProccess();
        PlayerPointProccess();
    }

    private void HealthBarProccess()
    {
        if (_healthBarDots[0] == null)
        {
            int dotsCount = HealthBarDotsHolder.transform.childCount;
            for (int i = 0; i < dotsCount; i++)
            {
                _healthBarDots[i] = HealthBarDotsHolder.transform.GetChild(i).gameObject;
            }
        }

        if (_fullHealth <= 0)
        {
            _fullHealth = _vehicleMovement.FullHealth;
        }
        _currentHealth = Mathf.FloorToInt(_vehicleMovement.GetHealth());
        _healthBarValue = Mathf.CeilToInt((_currentHealth / _fullHealth) * _healthBarDots.Length);

        for (int i = 0; i < _healthBarDots.Length; i++)
        {
            if (i < _healthBarValue)
            {
                _healthBarDots[i].SetActive(true);
            }else
            {
                _healthBarDots[i].SetActive(false);
            }
        }

        Color lerpedColor = Color.Lerp(LowHealColor, HighHealColor, (_currentHealth / _fullHealth));
        _healthBarDots[0].GetComponent<Image>().material.color = lerpedColor;
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


        _currentTurbo = Mathf.FloorToInt(_vehicleMovement.GetTurbo());
        _turboBarValue = (float)_currentTurbo / TopTurboLimitOnBar;

        _activeTurboBar.fillAmount = _turboBarValue;

        Color lerpedColor = Color.Lerp(LowTurboColor, HighTurboColor, _turboBarValue);
        _activeTurboBar.color = lerpedColor;
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
        }
    }

    private void PlayerPointProccess()
    {
        _playerPoint = _igm.GetPlayerPoint();
        PointUI.text = _playerPoint.ToString();
    }
}
