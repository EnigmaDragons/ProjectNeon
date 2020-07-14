using UnityEngine;
using System.Collections;
 
public static class AudioFadeOut 
{
    public static IEnumerator FadeOutAsync(this AudioSource audioSource, float duration) 
    {
        var startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) 
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
 
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
