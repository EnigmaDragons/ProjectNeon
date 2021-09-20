using UnityEngine;
using UnityEngine.Audio;

public sealed class InitAudioVolumeLevel : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string valueName = "MusicVolume";
    [SerializeField] private FloatReference reductionDb = new FloatReference(0f);
    [Header("Debug - For Initial Volume Preview Sound")]
    [SerializeField] private AudioSource player;
    [SerializeField] private AudioClip demoSound;
    [SerializeField] private bool loggingEnabled = false; 
    
    private void Start()
    {
        var volume = PlayerAudioPrefs.GetVolumeLevel(valueName);
        var mixerVolume = (Mathf.Log10(volume) * 20) - reductionDb;
        if (loggingEnabled)
            Log.Info($"Init - Set Audio Level for {valueName} to {volume} ({mixerVolume}db)", gameObject);
        mixer.SetFloat(valueName, volume <= 0 ? -80 : mixerVolume);
        if (player != null && demoSound != null)
            player.PlayOneShot(demoSound, 1f);
    }
}
