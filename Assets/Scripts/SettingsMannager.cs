using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMannagers : MonoBehaviour
{
    [Header("Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Gameplay")]
    public Slider sensitivitySlider;
    public Slider brightnessSlider;

    [Header("Graphics")]
    public Toggle vsyncToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
}
