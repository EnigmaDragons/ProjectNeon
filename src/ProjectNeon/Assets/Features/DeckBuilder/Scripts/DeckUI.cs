using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DeckUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged, SetSuperFocusDeckBuilderControl>
{
    [SerializeField] private NonDestructivePageViewer pageViewer;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private CardInDeckButton cardInDeckButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private Button clearDeckButton;
    
    private bool _cardInDeckSuperFocusEnabled;
    
    private void Awake()
    {
        if (clearDeckButton != null)
            clearDeckButton.onClick.AddListener(ClearDeck);
    }

    protected override void Execute(DeckBuilderHeroSelected msg) => GenerateDeck();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => OnDeckChanged();
    
    protected override void Execute(SetSuperFocusDeckBuilderControl msg)
    {
        if (msg.Name == DeckBuilderControls.CardInDeck)
            _cardInDeckSuperFocusEnabled = msg.Enabled;
    }

    private void OnDeckChanged() => GenerateDeck();

    public void Init(BaseHero character)
    {
        var deck = new RuntimeDeck {Cards = character.Deck.CardTypes};
        state.SelectedHeroesDeck = new HeroesDeck { Hero = new Hero(character, deck), Deck = deck.Cards };
        GenerateDeck();
    }

    public void GenerateDeck()
    {
        if (state.SelectedHeroesDeck == null)
            return;
        
        var hero = state.SelectedHeroesDeck.Hero;
        var baseHero = hero.Character;
        var heroAsMemberForLibrary = baseHero.AsMemberForLibrary(hero.Stats);
        Func<CardType, Card> createCardDelegate = cardType => new Card(-1, heroAsMemberForLibrary, cardType, cardType.NonBattleTint(baseHero), cardType.NonBattleBust(baseHero));
        pageViewer.Init(state.SelectedHeroesDeck.Deck
            .Select(createCardDelegate)
            .GroupBy(ByName)
            .OrderBy(ByCost)
            .ThenBy(ByRarity)
            .ThenBy(ByKeyAlloc)
            .Select(InitCardInDeckAlloc)
            .ToList(), InitEmptyAlloc,
            false);
    }
    
    private string ByName(Card card) => card.Name;
    private int ByCost(IGrouping<string, Card> grouping) => grouping.First().Cost.CostSortOrder();
    private Rarity ByRarity(IGrouping<string, Card> grouping) => grouping.First().Rarity;
    private string ByKeyAlloc(IGrouping<string, Card> grouping) => grouping.Key; 
    private void InitEmptyAlloc(GameObject obj) => obj.GetComponent<CardInDeckButton>().InitEmpty();
    private Action<GameObject> InitCardInDeckAlloc(IGrouping<string, Card> grouping) => InitCardInDeckButton(grouping.First());

    private Action<GameObject> InitCardInDeckButton(Card card)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardInDeckButton = gameObj.GetComponent<CardInDeckButton>();
            cardInDeckButton.Init(card, _cardInDeckSuperFocusEnabled);
        };
        return init;
    }

    private void ClearDeck() => state.ClearCurrentDeck();
}
