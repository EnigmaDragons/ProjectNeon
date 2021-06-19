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

    private void Awake()
    {
        heroSelector.Init(library.UnlockedHeroes.Cast<HeroCharacter>().ToArray(), GenerateCardSelection);
        GenerateCardSelection(library.UnlockedHeroes.First());
    }

    private void GenerateCardSelection(HeroCharacter hero)
    {
        var archKeys = hero.ArchetypeKeys();
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            allCards.All
                .Where(c => archKeys.Contains(c.GetArchetypeKey()) || c.Archetypes.None())
                .Where(c => c.Rarity != Rarity.Basic)
                .OrderBy(c => c.Archetypes.None() ? 99 : c.Archetypes.Count)
                .ThenBy(c => c.GetArchetypeKey())
                .ThenBy(c => c.Rarity)
                .ThenBy(c => c.Cost.BaseAmount)
                .ThenBy(c => c.Name)
                .Concat(hero.ClassCard)
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
}
