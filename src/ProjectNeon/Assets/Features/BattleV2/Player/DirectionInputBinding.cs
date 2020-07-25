using UnityEngine;

[CreateAssetMenu]
public class DirectionInputBinding : ScriptableObject, IDirectionControllable
{
    private IDirectionControllable _inner;

    public void Bind(IDirectionControllable target)
    {
        _inner?.LostFocus();
        _inner = target;
    }

    public void Clear()
    {
        _inner?.LostFocus();
        _inner = null;
    }

    public void MoveNext() => _inner?.MoveNext();
    public void MovePrevious() => _inner?.MovePrevious();
    public void LostFocus() => _inner?.LostFocus();
}
