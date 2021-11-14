using System;

public class SimpleSegment : CutsceneSegment
{
    private readonly Action _action;

    public SimpleSegment(Action action) => _action = action;

    public void Start() => _action();
}
