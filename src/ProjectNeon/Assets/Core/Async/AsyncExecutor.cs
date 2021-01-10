using System;
using System.Collections;
using UnityEngine;

public class AsyncExecutor : OnMessage<ExecuteAfterDelayRequested>
{
    protected override void Execute(ExecuteAfterDelayRequested msg) 
        => StartCoroutine(ExecuteAfterDelay(msg.DelaySeconds, msg.Action));

    private IEnumerator ExecuteAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
