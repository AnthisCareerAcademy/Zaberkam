using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.50f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.50f);

        musicSlider.value = music;
        sfxSlider.value = sfx;

        SetMusicVolume(music);
        SetSFXVolume(sfx);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
