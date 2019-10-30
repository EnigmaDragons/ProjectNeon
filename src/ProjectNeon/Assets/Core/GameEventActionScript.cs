using UnityEngine;

public abstract class GameEventActionScript : MonoBehaviour
{
    [SerializeField] private GameEvent trigger;
    [SerializeField] private GameEvent onFinished;

    private void OnEnable() => trigger.Subscribe(() => { Execute(); onFinished.Publish(); }, this);
    private void OnDisable() => trigger.Unsubscribe(this);

    protected abstract void Execute();
}
