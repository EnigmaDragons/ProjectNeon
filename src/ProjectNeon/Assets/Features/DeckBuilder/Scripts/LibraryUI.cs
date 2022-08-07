using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LibraryUI : OnMessage<DeckBuilderCurrentDeckChanged, DeckBuilderStateUpdated, SetSuperFocusDeckBuilderControl>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private PartyCardCollection partyCards;
    [SerializeField] private DeckBuilderState state;

    private Hero _selectedHero;
    private bool _superFocusEnabledOnCardsInLibrary;
    
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => GenerateLibrary();
    protected override void Execute(DeckBuilderStateUpdated msg) => GenerateLibrary();

    protected override void Execute(SetSuperFocusDeckBuilderControl msg)
    {
        if (msg.Name == DeckBuilderControls.CardInLibrary)
            _superFocusEnabledOnCardsInLibrary = msg.Enabled;
    }

    private void GenerateLibrary()
    {
        Log.Info("Generate Library");
        var heroChanged = state.SelectedHeroesDeck.Hero != _selectedHero;
        _selectedHero = state.SelectedHeroesDeck.Hero;
        var cardsForHero = partyCards.AllCards
            .Where(cardWithCount => 
                cardWithCount.Key.Archetypes.All(archetype => _selectedHero.Character.Archetypes.Contains(archetype)) 
                && cardWithCount.Key.Id != _selectedHero.BasicCard.Id)
            .OrderBy(c => c.Key.Archetypes.None() ? 999 : 0)
            .ThenByDescending(c => (int)c.Key.Rarity)
            .ToList();

        cardsForHero.Insert(0, new KeyValuePair<CardTypeData, int>(_selectedHero.BasicCard, 0));

        var cardUsage = cardsForHero.SafeToDictionary(c => c.Key,
            c => new Tuple<int, int>(c.Value, c.Value - state.HeroesDecks.Sum(deck => deck.Deck.Count(card => card.Id == c.Key.Id))));

        var heroMember = state.SelectedHeroesDeck.Hero.AsMember(-1);  
        var cardActions = !state.ShowFormulas 
            ? cardUsage.ToDictionary(c => new Card(-1, heroMember, c.Key), c => c.Value)
                .Select(x => InitCardInLibraryButton(x.Key, x.Value.Item1, x.Value.Item2))
                .ToList()
            : cardUsage
                .Select(x => InitCardInLibraryButton(x.Key, x.Value.Item1, x.Value.Item2))
                .ToList();
        
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            cardActions,
            x => {},
            !heroChanged);
    }

    private Action<GameObject> InitCardInLibraryButton(CardTypeData card, int numTotal, int numAvailable)
    {
        void Init(GameObject gameObj)
        {
            var button = gameObj.GetComponent<CardInLibraryButton>();
                if (card.Id.Equals(_selectedHero.BasicCard.Id))
                    button.InitBasic(card);
                else
                    button.Init(card, numTotal, numAvailable, _superFocusEnabledOnCardsInLibrary);
        }

        return Init;
    }
    
    private Action<GameObject> InitCardInLibraryButton(Card card, int numTotal, int numAvailable)
    {
        void Init(GameObject gameObj)
        {
            var button = gameObj.GetComponent<CardInLibraryButton>();
            if (card.Id.Equals(_selectedHero.BasicCard.Id))
                button.InitBasic(card);
            else
                button.Init(card, numTotal, numAvailable, _superFocusEnabledOnCardsInLibrary);
        }

        return Init;
    }
}
