using System;
using System.Linq;
using UnityEngine;

public class DeckSelectionUI : MonoBehaviour
{
    [SerializeField] private GameEvent heroSelected;
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private AddDeckButton addDeckButton;
    [SerializeField] private SelectDeckButton selectDeckButton;
    [SerializeField] private GameEvent decksChanged;
    [SerializeField] private DeckStorage storage;
    [SerializeField] private DeckBuilderState state;

    private void OnEnable()
    {
        heroSelected.Subscribe(GenerateDeckSelection, this);
        decksChanged.Subscribe(GenerateDeckSelection, this);
    }

    private void OnDisable()
    {
        heroSelected.Unsubscribe(this);
        decksChanged.Unsubscribe(this);
    }

    private void GenerateDeckSelection()
    {
        pageViewer.Init(selectDeckButton.gameObject, addDeckButton.gameObject, storage.GetDecks()
            .Where(x => x.ClassName != null && !string.IsNullOrWhiteSpace(x.Name) && x.ClassName.Value == state.SelectedHero.ClassName.Value)
            .Select(InitSelectDeckButton)
            .ToList(), x => {}, true);
    }

    private Action<GameObject> InitSelectDeckButton(Deck deck)
    {
        Action<GameObject> init = gameObj => gameObj.GetComponent<SelectDeckButton>().Init(deck);
        return init;
    }
}
