using UnityEngine;
using UnityEngine.UI;

public class DeckNameUI : MonoBehaviour
{
    [SerializeField] private InputField deckNameTextField;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckChosen;

    public void OnChange() => state.TemporaryDeck.Name = deckNameTextField.text;

    private void OnEnable() => deckChosen.Subscribe(() =>
        {
            deckNameTextField.text = state.TemporaryDeck.Name;
            deckNameTextField.interactable = !state.TemporaryDeck.IsImmutable;
        }, this);

    private void OnDisable() => deckChosen.Unsubscribe(this);
}
