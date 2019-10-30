using UnityEngine;
using UnityEngine.UI;

public class EditDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Button button;
    [SerializeField] private GameEvent deckBuildingStarted;

    public void Edit()
    {
        state.Operation = state.SelectedDeck.IsImmutable ? DeckBuilderOperation.View : DeckBuilderOperation.Edit;
        deckBuildingStarted.Publish();
    }

    private void Update() => button.interactable = state.DeckIsSelected;
}
