using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    private GameObject MainMenu;
    [SerializeField]
    private GameObject ShipSelectMenu;
    [SerializeField]
    private GameObject SettingsMenu;
    [Header("3D Canvas")]
    [SerializeField]
    private GameObject Main3D;
    [SerializeField]
    private GameObject ShipSelect3D;


    GameManager _gm;
    AudioManager _audioManager;
    PopupController _popupController;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        _audioManager = FindObjectOfType<AudioManager>();
        _popupController = GameObject.Find("Popup Controller").GetComponent<PopupController>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackButtonProccess();
            }
        }
    }

    public void ButtonSound()
    {
        _audioManager.Play("Button");
    }

    public void SavePlayer()
    {
        _gm.SavePlayerData();
    }

    private void BackButtonProccess()
    {
        _audioManager.Play("Button");

        if (MainMenu.activeSelf)
        {
            _popupController.OpenPopup("ExitGame");
        }
        else
        {
            SettingsMenu.SetActive(false);
            ShipSelectMenu.SetActive(false);
            ShipSelect3D.SetActive(false);

            MainMenu.SetActive(true);
            Main3D.SetActive(true);
            SavePlayer();
        }
    }
}
