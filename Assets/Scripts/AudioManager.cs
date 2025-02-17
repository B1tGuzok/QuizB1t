using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Toggle toggleMusic;
    public Toggle toggleSound;
    public Slider sliderVolumeMusic;
    public Slider sliderVolumeSound;
    public AudioSource audioSrc;
    public AudioSource soundSrc;
    public AudioClip sound;
    public float volumeMusic = 0.5f;
    public float volumeSound = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Load();
        ValueMusic();
        ValueSound();
    }

    public void SliderSound()
    {
        volumeSound = sliderVolumeSound.value;
        ValueSound();
    }

    public void ToggleSound()
    {
        if (toggleSound.isOn == true)
        {
            volumeSound = 0;
        }
        else
        {
            volumeSound = 1;
        }
        ValueSound();
    }

    private void ValueSound()
    {
        soundSrc.volume = volumeSound;
        sliderVolumeSound.value = volumeSound;
        if (volumeSound == 0)
            toggleSound.isOn = true;
        else
            toggleSound.isOn = false;
    }

    public void PlaySound()
    {
        soundSrc.Play();
    }

    // MUSIC

    public void ToggleMusic()
    {
        if (toggleMusic.isOn == false)
        {
            volumeMusic = 0;
        }
        else
        {
            volumeMusic = 1;
        }
        ValueMusic();
    }

    public void SliderMusic()
    {
        volumeMusic = sliderVolumeMusic.value;
        ValueMusic();
    }

    private void ValueMusic()
    {
        audioSrc.volume = volumeMusic;
        sliderVolumeMusic.value = volumeMusic;
        if (volumeMusic == 0)
            toggleMusic.isOn = false;
        else
            toggleMusic.isOn = true;
    }


    public void Save()
    {
        PlayerPrefs.SetFloat("volumeMusic", volumeMusic);
        PlayerPrefs.SetFloat("volumeSound", volumeSound);
    }

    private void Load()
    {
        volumeMusic = PlayerPrefs.GetFloat("volumeMusic", volumeMusic);
        volumeSound = PlayerPrefs.GetFloat("volumeSound", volumeSound);
    }
}
