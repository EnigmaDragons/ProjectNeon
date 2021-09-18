using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public sealed class UnityMixerVolumeSlider : MonoBehaviour 
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;
    [SerializeField] private string valueName = "MusicVolume";
    [SerializeField] private FloatReference reductionDb = new FloatReference(0f);

    void Start()
    {
        slider.value = PlayerAudioPrefs.GetVolumeLevel(valueName);
        slider.onValueChanged.AddListener(SetLevel);
    }
    
    public void SetLevel(float sliderValue)
    {
        var mixerVolume = (Mathf.Log10(sliderValue) * 20) - reductionDb;
        Log.Info($"Slider - Set Audio Level for {valueName} to {sliderValue} ({mixerVolume}db)");
        mixer.SetFloat(valueName, mixerVolume);
        PlayerAudioPrefs.SetVolumeLevel(valueName, sliderValue);
        Message.Publish(new MixerVolumeChanged(valueName, sliderValue));
    }
}
