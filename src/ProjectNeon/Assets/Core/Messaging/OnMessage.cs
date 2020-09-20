using UnityEngine;

public abstract class OnMessage<T> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);

    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2, T3> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2, T3, T4> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2, T3, T4, T5> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
        Message.Subscribe<T5>(Execute, this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}
