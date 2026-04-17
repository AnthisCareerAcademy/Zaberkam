using UnityEngine;
using UnityEngine.UI;
//eventually will probably need to connect it to the player camera if it doesnt work 
public class GameplaySettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public static float mouseSensitivity = 1f;
    void Start()
    {
        float sens = PlayerPrefs.GetFloat("MouseSensitivity", 1f);

        sensitivitySlider.value = sens;
        mouseSensitivity = sens;

        sensitivitySlider.onValueChanged.AddListener(SetSenitivity);
    }

    public void SetSenitivity(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }
}