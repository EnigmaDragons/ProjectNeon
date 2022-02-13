using UnityEngine;

[CreateAssetMenu(menuName = "ModalState")]
public class StaticModalState : ScriptableObject
{
    [SerializeField] private string current;

    public Maybe<string> Current => string.IsNullOrWhiteSpace(current) ? Maybe<string>.Missing() : Maybe<string>.Present(current);
    public void Set(string newModal) => current = newModal;
    public void Clear() => current = null;
    public void Clear(string modal)
    {
        if (current == modal)
            Clear();
    }
}
