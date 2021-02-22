using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    GameManager _gm;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        _gm = GameObject.Find(VariableController.GAME_MANAGER).GetComponent<GameManager>();

        if (_gm.pd.music)
        {
            Play("Theme");
        }
    }

    public void Play(string name, float customVolume = -1)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s== null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (customVolume >= 0)
        {
            s.source.volume = customVolume;
        }

        if (_gm.pd.sound || name == "Theme")
        {
            s.source.Play();
        }
    }

    public void PlayOneShot(string name, float customVolume = -1)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s== null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (customVolume >= 0)
        {
            s.source.volume = customVolume;
        }

        if (_gm.pd.sound || name == "Theme")
        {
            s.source.PlayOneShot(s.clip);
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }
}
