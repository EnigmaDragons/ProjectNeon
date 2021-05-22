using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LibraryUI : OnMessage<DeckBuilderCurrentDeckChanged, DeckBuilderFiltersChanged>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private PartyCardCollection partyCards;
    [SerializeField] private DeckBuilderState state;

    private HeroCharacter _selectedHero;
    
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => GenerateLibrary();
    protected override void Execute(DeckBuilderFiltersChanged msg) => GenerateLibrary();

    private void GenerateLibrary()
    {
        var heroChanged = state.SelectedHeroesDeck.Hero.Character != _selectedHero;
        _selectedHero = state.SelectedHeroesDeck.Hero.Character;
        var cardsForHero = partyCards.AllCards
            .Where(cardWithCount => cardWithCount.Key.Archetypes.All(archetype => _selectedHero.Archetypes.Contains(archetype)) 
                && (state.ShowRarities.None() 
                    || state.ShowRarities.Contains(cardWithCount.Key.Rarity)) 
                && (state.ShowArchetypes.None() 
                    || (cardWithCount.Key.Archetypes.None() && state.ShowArchetypes.Contains("")) 
                    || cardWithCount.Key.Archetypes.Any(state.ShowArchetypes.Contains)))
            .ToList();
        if (state.ShowRarities.None() || state.ShowRarities.Contains(Rarity.Basic))
            cardsForHero.Insert(0, new KeyValuePair<CardType, int>(_selectedHero.ClassCard, 0));
        var cardUsage = cardsForHero.ToDictionary(c => c.Key,
            c => new Tuple<int, int>(c.Value, c.Value - state.HeroesDecks.Sum(deck => deck.Deck.Count(card => card == c.Key))));
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            cardUsage
                .Select(x => InitCardInLibraryButton(x.Key, x.Value.Item1, x.Value.Item2))
                .ToList(), 
            x => {},
            !heroChanged);
    }

    private Action<GameObject> InitCardInLibraryButton(CardType card, int numTotal, int numAvailable)
    {
        void Init(GameObject gameObj)
        {
            var button = gameObj.GetComponent<CardInLibraryButton>();
                if (card == _selectedHero.ClassCard)
                    button.InitBasic(card);
                else
                    button.Init(card, numTotal, numAvailable);
        }

        return Init;
    }
}
