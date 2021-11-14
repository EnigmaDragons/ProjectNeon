using UnityEngine;

public class FModLifecycleSounds : MonoBehaviour
{
    [FMODUnity.EventRef] public string onEnable;
    [FMODUnity.EventRef] public string onDisable;
    
    public void OnEnable() => PlayOneShot(onEnable);
    public void OnDisable() => PlayOneShot(onDisable);

    private void PlayOneShot(string s)
    {
        if (!string.IsNullOrWhiteSpace(s))
            FMODUnity.RuntimeManager.PlayOneShotAttached(s, gameObject);
    }
}
