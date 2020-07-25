using UnityEngine;

[CreateAssetMenu]
public class DirectionInputBinding : ScriptableObject, IDirectionControllable
{
    private IDirectionControllable _inner;

    public void Bind(IDirectionControllable target) => _inner = target;
    public void Clear() => _inner = null;
    
    public void MoveNext() => _inner?.MoveNext();

    public void MovePrevious() => _inner?.MovePrevious();
}
