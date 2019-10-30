using UnityEngine;

public class DeckBuilderState : ScriptableObject
{
    [SerializeField] private GameEvent heroSelected;
    [SerializeField] private GameEvent deckSelected;
    [SerializeField] private GameEvent deckChosen;

    private Hero _selectedHero;
    private Deck _selectedDeck;
    private Deck _temporaryDeck;

    public DeckBuilderOperation Operation { get; set; }
    public bool DeckIsSelected { get; set; }

    public Hero SelectedHero
    {
        get => _selectedHero;
        set
        {
            _selectedHero = value;
            heroSelected.Publish();
        }
    }

    public Deck SelectedDeck
    {
        get => _selectedDeck;
        set
        {
            _selectedDeck = value;
            deckSelected.Publish();
        }
    }

    public Deck TemporaryDeck
    {
        get => _temporaryDeck;
        set
        {
            _temporaryDeck = value;
            deckChosen.Publish();
        }
    }
}
