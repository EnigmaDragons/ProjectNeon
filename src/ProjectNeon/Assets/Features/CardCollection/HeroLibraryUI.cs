using System;
using System.Linq;
using UnityEngine;

public class HeroLibraryUI : MonoBehaviour
{
    [SerializeField] private NonDestructivePageViewer pageViewer;
    [SerializeField] private ShopCardPool allCards;
    [SerializeField] private Library library;
    [SerializeField] private HeroFaceSelector heroSelector;
    [SerializeField] private ArchetypeTints archetypeTints;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private bool cheatGainCard;

    private void Awake()
    {
        heroSelector.Init(library.UnlockedHeroes.ToArray(), GenerateCardSelection);
        GenerateCardSelection(library.UnlockedHeroes.First());
        Log.Info("Hero Library UI - Init");
    }

    private void GenerateCardSelection(BaseHero hero)
    {
        var archKeys = hero.ArchetypeKeys();
        var heroMember = hero.AsMemberForLibrary();
        var excludedCards = hero.ExcludedCards;
        var additionalCards = hero.AdditionalStartingCards;
        var cards = allCards.All
            .Where(c => archKeys.Contains(c.GetArchetypeKey()) || c.Archetypes.None())
            .Where(c => c.Rarity != Rarity.Basic)
            .Where(c => !excludedCards.Contains(c))
            .Where(c => c.Archetypes.Any())
            .Concat(additionalCards)
            .OrderBy(c => c.Archetypes.None() ? 99 : c.Archetypes.Count)
            .ThenBy(c => c.GetArchetypeKey())
            .ThenBy(c => c.Rarity)
            .ThenBy(c => c.Cost.BaseAmount)
            .ThenBy(c => c.Name)
            .Concat(hero.BasicCard)
            .Concat(hero.ParagonCards);
        
        pageViewer.Init(
            cards
                .Select(c => new Card(-1, heroMember, c))
                .Select(InitCardInLibraryButton)
                .ToList(), 
            x => x.GetComponent<CardInLibraryButton>().InitEmpty(),
            false);
    }

    private void NoOp() {}
    private Action<GameObject> InitCardInLibraryButton(Card card)
    {
        void Init(GameObject gameObj) => gameObj.GetComponent<CardInLibraryButton>().InitInfoOnly(card, cheatGainCard ? () =>
            {
                party.Add(card.CardTypeOrNothing.Value);
                party.Add(card.CardTypeOrNothing.Value);
                party.Add(card.CardTypeOrNothing.Value);
                party.Add(card.CardTypeOrNothing.Value);
                Message.Publish(new ToggleCardLibrary());
            }
            : (Action)NoOp);
        return Init;
    }
}
