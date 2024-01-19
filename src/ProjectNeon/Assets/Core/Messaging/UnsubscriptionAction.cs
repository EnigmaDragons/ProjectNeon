using System.Collections.Generic;

public class UnsubscriptionAction
{
    private readonly object _toRemove;
    private readonly List<object> _removeFrom;
    private bool _hasRemoved;

    public UnsubscriptionAction(object toRemove, List<object> removeFrom)
    {
        _toRemove = toRemove;
        _removeFrom = removeFrom;
        _hasRemoved = false;
    }

    public void Execute()
    {
        if (_hasRemoved)
            return;
        _removeFrom.Remove(_toRemove);
        _hasRemoved = true;
    } 
}