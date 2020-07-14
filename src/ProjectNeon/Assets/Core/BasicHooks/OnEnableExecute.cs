using UnityEngine;
using UnityEngine.Events;

public sealed class OnEnableExecute : MonoBehaviour
{
    [SerializeField] private UnityEvent action;

    private void OnEnable() => action.Invoke();
}
