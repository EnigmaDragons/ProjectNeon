using UnityEngine;
using UnityEngine.UI;

public class DeleteDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Button button;
    [SerializeField] private GameEvent decksChanged;
    [SerializeField] private DeckStorage storage;

    public void Delete()
    {
        state.DeckIsSelected = false;
        storage.DeleteDeck(state.SelectedDeck);
        decksChanged.Publish();
    }

    private void Update() => button.interactable = state.DeckIsSelected && !state.SelectedDeck.IsImmutable;
}
