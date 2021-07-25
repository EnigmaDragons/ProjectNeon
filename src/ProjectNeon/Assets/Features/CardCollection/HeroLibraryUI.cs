using System;
using System.Linq;
using UnityEngine;

public class HeroLibraryUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private ShopCardPool allCards;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private Library library;
    [SerializeField] private HeroFaceSelector heroSelector;
    [SerializeField] private ArchetypeTints archetypeTints;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private bool cheatGainCard;

    private void Awake()
    {
        heroSelector.Init(library.UnlockedHeroes.Cast<HeroCharacter>().ToArray(), GenerateCardSelection);
        GenerateCardSelection(library.UnlockedHeroes.First());
    }

    private void GenerateCardSelection(HeroCharacter hero)
    {
        var archKeys = hero.ArchetypeKeys();
        var heroMember = hero.AsMemberForLibrary();
        var cards = allCards.All
            .Where(c => archKeys.Contains(c.GetArchetypeKey()) || c.Archetypes.None())
            .Where(c => c.Rarity != Rarity.Basic)
            .OrderBy(c => c.Archetypes.None() ? 99 : c.Archetypes.Count)
            .ThenBy(c => c.GetArchetypeKey())
            .ThenBy(c => c.Rarity)
            .ThenBy(c => c.Cost.BaseAmount)
            .ThenBy(c => c.Name)
            .Concat(hero.ClassCard);
        
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            cards
                .Select(c => new Card(-1, heroMember, c))
                .Select(InitCardInLibraryButton)
                .ToList(), 
            x => {},
            false);
    }

    private Action<GameObject> InitCardInLibraryButton(CardTypeData card)
    {
        void Init(GameObject gameObj) => gameObj.GetComponent<CardInLibraryButton>().InitInfoOnly(card);
        return Init;
    }
    
    private Action<GameObject> InitCardInLibraryButton(Card card)
    {
        void Init(GameObject gameObj) => gameObj.GetComponent<CardInLibraryButton>().InitInfoOnly(card, cheatGainCard ? () =>
            {
                party.Add(card.BaseType);
                Message.Publish(new ToggleCardLibrary());
            }
            : (Action)(() => {}));
        return Init;
    }
}
