using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    GameManager _gm;
    AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void ButtonSound()
    {
        _audioManager.Play("Button");
    }

    public void SavePlayer()
    {
        _gm.SavePlayerData();
    }
}
