using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButtonV2: OnMessage<DeckBuilderHeroSelected>
{
    [SerializeField] private GameObject owner;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference deckSize;
    [SerializeField] private Button button;
    [SerializeField] private Image bust;
    [SerializeField] private Image selected;
    [SerializeField] private int heroIndex;
    [SerializeField] private Image toTint;

    private bool _selected;
    
    public void Init()
    {
        if (state.HeroesDecks.Count <= heroIndex)
        {
            owner.SetActive(false);
            return;   
        }

        bust.sprite = state.HeroesDecks[heroIndex].Hero.Character.Bust;
        _selected = heroIndex == 0;
        if (_selected)
            state.SelectedHeroesDeck = state.HeroesDecks[heroIndex];
        selected.gameObject.SetActive(_selected);
        button.onClick.AddListener(() =>
        {
            if (!_selected)
                state.SelectedHeroesDeck = state.HeroesDecks[heroIndex];
        });
    }
    
    protected override void Execute(DeckBuilderHeroSelected msg)
    {
        if (state.HeroesDecks.Count <= heroIndex)
            return;
        _selected = state.HeroesDecks[heroIndex] == state.SelectedHeroesDeck;
        selected.gameObject.SetActive(_selected);
        if (_selected)
            toTint.color = state.SelectedHeroesDeck.Deck.Count == deckSize.Value ? Color.white : Color.red;
    }
}