using System;

public static class Async
{
    public static void ExecuteAfterDelay(float delay, Action action) =>
        Message.Publish(new ExecuteAfterDelayRequested(delay, action));
}
