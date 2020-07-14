using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundAtRandomTimes : MonoBehaviour
{
    [SerializeField] private float minTimeBetween = 3f;
    [SerializeField] private float maxTimeBetween = 8f;

    private void OnEnable() => StartCoroutine(PlayForever(GetComponent<AudioSource>()));

    private IEnumerator PlayForever(AudioSource src)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetween, maxTimeBetween));
            src.Play();
        }
    }
}
