using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public sealed class OnEnableExecuteDelayed : MonoBehaviour
{
    [SerializeField] private UnityEvent action;
    [SerializeField] private float delay;

    private void OnEnable() => StartCoroutine(Execute());
    
    private IEnumerator Execute()
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
