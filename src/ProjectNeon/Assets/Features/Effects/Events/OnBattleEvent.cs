using UnityEngine;

public abstract class OnBattleEvent<T> : MonoBehaviour
{
    private void OnEnable() => BattleEvent.Subscribe<T>(Execute, this);
    private void OnDisable() => BattleEvent.Unsubscribe(this);

    protected abstract void Execute(T e);
}
