using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
   /* [SerializeField] private FmodGameAudioManager Fmanager;
    [SerializeField] private string valueName = "MusicVolume";
    [SerializeField] private FloatReference reductionDb = new FloatReference(0f);
    [Header("Debug - For Initial Volume Preview Sound")]
    [SerializeField] private AudioSource player;
    [SerializeField] private AudioClip demoSound;
    [SerializeField] private bool loggingEnabled = false;

    private void Start()
    {
        var volume = PlayerPrefs.GetFloat(valueName, 0.5f);
        var mixerVolume = (Mathf.Log10(volume) * 20) - reductionDb;
        if (loggingEnabled)
            Log.Info($"Init - Set Audio Level for {valueName} to {volume} ({mixerVolume}db)", gameObject);
        Fmanager.SetFloat(valueName, mixerVolume);
        if (player != null && demoSound != null)
            player.PlayOneShot(demoSound, 1f);
    } */
}
