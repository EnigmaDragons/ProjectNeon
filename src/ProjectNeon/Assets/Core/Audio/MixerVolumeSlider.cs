using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public sealed class MixerVolumeSlider : MonoBehaviour 
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;
    [SerializeField] private string valueName = "MusicVolume";
    [SerializeField] private FloatReference reductionDb = new FloatReference(0f);

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(valueName, 0.5f);
        slider.onValueChanged.AddListener(SetLevel);
    }
    
    public void SetLevel(float sliderValue)
    {
        var mixerVolume = (Mathf.Log10(sliderValue) * 20) - reductionDb;
        Debug.Log($"Slider - Set Audio Level for {valueName} to {sliderValue} ({mixerVolume}db)");
        mixer.SetFloat(valueName, mixerVolume);
        PlayerPrefs.SetFloat(valueName, sliderValue);
        Message.Publish(new MixerVolumeChanged(valueName));
    }
}
