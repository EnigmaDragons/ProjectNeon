using UnityEngine;

public abstract class OnBattleEvent<T> : MonoBehaviour
{
    private void OnEnable() => BattleEvent.Subscribe<T>(Execute, this);
    private void OnDisable() => BattleEvent.Unsubscribe(this);

    protected abstract void Execute(T e);
}

public abstract class OnBattleEvent<T1, T2> : MonoBehaviour
{
    private void OnEnable()
    {
        BattleEvent.Subscribe<T1>(Execute, this);
        BattleEvent.Subscribe<T2>(Execute, this);
    }

    private void OnDisable() => BattleEvent.Unsubscribe(this);

    protected abstract void Execute(T1 e);
    protected abstract void Execute(T2 e);
}
