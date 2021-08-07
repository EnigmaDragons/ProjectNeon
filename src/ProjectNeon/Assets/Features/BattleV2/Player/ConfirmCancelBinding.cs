using UnityEngine;

[CreateAssetMenu(menuName = "UI/ConfirmCancelBinding")]
public class ConfirmCancelBinding : ScriptableObject, IConfirmCancellable
{
    private IConfirmCancellable _inner;

    public void Bind(IConfirmCancellable x) => _inner = x;
    
    public void Confirm() => _inner?.Confirm();
    public void Cancel() => _inner?.Cancel();
}
