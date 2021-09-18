using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSliderV2 : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private string valueName = "MusicVolume";

    void Start()
    {
        slider.value = PlayerAudioPrefs.GetVolumeLevel(valueName);
        slider.onValueChanged.AddListener(SetLevel);
    }
    
    public void SetLevel(float sliderValue)
    {
        PlayerAudioPrefs.SetVolumeLevel(valueName, sliderValue);
    }
}
