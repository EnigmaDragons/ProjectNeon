using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public sealed class OnVideoFinished : MonoBehaviour
{
    [SerializeField] private UnityEvent onFinished;
    [SerializeField] private FloatReference delay;

    private void Awake() => GetComponent<VideoPlayer>().loopPointReached += _ => StartCoroutine(ExecuteAfterDelay());

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        onFinished.Invoke();
    }
}
  
