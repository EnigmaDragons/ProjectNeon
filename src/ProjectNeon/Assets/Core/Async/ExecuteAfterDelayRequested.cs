using System;

public class ExecuteAfterDelayRequested
{
    public float DelaySeconds { get; }
    public Action Action { get; }

    public ExecuteAfterDelayRequested(float delay, Action action)
    {
        DelaySeconds = delay;
        Action = action;
    }
}
