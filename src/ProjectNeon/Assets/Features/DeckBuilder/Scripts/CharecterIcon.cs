using UnityEngine;
using UnityEngine.UI;

public class CharecterIcon : MonoBehaviour
{
    [SerializeField] DeckBuilderState deckBuilderState;
    [SerializeField, ReadOnly] bool selected;
    [SerializeField] CharactersEnum characterValue;
    [SerializeField] GameObject selectImage;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        deckBuilderState.OnCurrentDeckChanged.Subscribe(OnCharacterChanged, this);
        OnCharacterChanged();
    }

    private void OnCharacterChanged()
    {
        selected = deckBuilderState.currentCharacter == characterValue;
        selectImage.SetActive(selected);
    }

    public void OnClick()
    {
        CharacterController.instance.SelectCharacter(characterValue);
    }
    private void OnDisable()
    {
        deckBuilderState.OnCurrentDeckChanged.Unsubscribe(this);
    }
}
