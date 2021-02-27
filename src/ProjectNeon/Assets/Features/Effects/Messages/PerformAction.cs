using System;

public class PerformAction
{
    public Action Action { get; }

    public PerformAction(Action a) => Action = a;
}
