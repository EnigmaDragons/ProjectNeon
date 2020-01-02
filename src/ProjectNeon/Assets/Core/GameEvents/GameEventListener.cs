using UnityEngine;
using UnityEngine.Events;

public sealed class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    [SerializeField, DTValidator.Optional] private UnityEvent response;

    private void OnEnable() => gameEvent.Subscribe(this);
    private void OnDisable() => gameEvent.Unsubscribe(this);
    public void OnEventRaised() => response.Invoke();
}
