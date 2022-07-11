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
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    } 

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
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

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
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}


public abstract class OnMessage<T1, T2, T3, T4, T5, T6> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
        Message.Subscribe<T5>(Execute, this);
        Message.Subscribe<T6>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    protected abstract void Execute(T6 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2, T3, T4, T5, T6, T7> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
        Message.Subscribe<T5>(Execute, this);
        Message.Subscribe<T6>(Execute, this);
        Message.Subscribe<T7>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    protected abstract void Execute(T6 msg);
    protected abstract void Execute(T7 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}

public abstract class OnMessage<T1, T2, T3, T4, T5, T6, T7, T8> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
        Message.Subscribe<T5>(Execute, this);
        Message.Subscribe<T6>(Execute, this);
        Message.Subscribe<T7>(Execute, this);
        Message.Subscribe<T8>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    protected abstract void Execute(T6 msg);
    protected abstract void Execute(T7 msg);
    protected abstract void Execute(T8 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}


public abstract class OnMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> : MonoBehaviour
{
    private void OnEnable()
    {
        Message.Subscribe<T1>(Execute, this);
        Message.Subscribe<T2>(Execute, this);
        Message.Subscribe<T3>(Execute, this);
        Message.Subscribe<T4>(Execute, this);
        Message.Subscribe<T5>(Execute, this);
        Message.Subscribe<T6>(Execute, this);
        Message.Subscribe<T7>(Execute, this);
        Message.Subscribe<T8>(Execute, this);
        Message.Subscribe<T9>(Execute, this);
        AfterEnable();
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        AfterDisable();
    }

    protected abstract void Execute(T1 msg);
    protected abstract void Execute(T2 msg);
    protected abstract void Execute(T3 msg);
    protected abstract void Execute(T4 msg);
    protected abstract void Execute(T5 msg);
    protected abstract void Execute(T6 msg);
    protected abstract void Execute(T7 msg);
    protected abstract void Execute(T8 msg);
    protected abstract void Execute(T9 msg);
    
    protected virtual void AfterEnable() {}
    protected virtual void AfterDisable() {}
}
