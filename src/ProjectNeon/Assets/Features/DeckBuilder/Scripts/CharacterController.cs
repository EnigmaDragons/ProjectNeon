using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] DeckBuilderState deckBuilderState;

    public static CharacterController instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void SelectCharacter(CharactersEnum characterValue)
    {
        deckBuilderState.currentCharacter = characterValue;
        if (deckBuilderState.OnCurrentDeckChanged != null)
            deckBuilderState.OnCurrentDeckChanged.Publish();
    }
}
