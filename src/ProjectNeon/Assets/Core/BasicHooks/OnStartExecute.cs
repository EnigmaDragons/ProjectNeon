using UnityEngine;
using UnityEngine.Events;

public sealed class OnStartExecute : MonoBehaviour
{
    [SerializeField] private UnityEvent action;

    void Start() => action.Invoke();
}
