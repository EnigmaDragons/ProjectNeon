using UnityEngine;
using UnityEngine.Events;

public class OnAwakeExecute : MonoBehaviour
{
    [SerializeField] private UnityEvent action;

    private void Awake() => action.Invoke();
}
