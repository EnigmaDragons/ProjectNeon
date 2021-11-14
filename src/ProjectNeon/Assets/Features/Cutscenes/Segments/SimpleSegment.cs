using System;

public class SimpleSegment : CutsceneSegment
{
    private readonly Action _startAction;
    private readonly Action _fastForwardAction;

    public SimpleSegment(Action startAction)
        : this(startAction, () => {}) {}
    
    public SimpleSegment(Action startAction, Action fastForwardAction)
    {
        _startAction = startAction;
        _fastForwardAction = fastForwardAction;
    }

    public void Start() => _startAction();
    public void FastForwardToFinishInstantly() => _fastForwardAction();
}
