using System;
using System.Linq;
using UnityEngine;

public class FullLibraryUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private ShopCardPool allCards;
    [SerializeField] private GameObject emptyCard;

    private void Awake()
    {
        GenerateLibrary();
    }
    
    private void GenerateLibrary()
    {
        pageViewer.Init(
            cardInLibraryButtonTemplate.gameObject, 
            emptyCard, 
            allCards.All
                .OrderBy(c => c.LimitedToClass.Select(l => l.Name, ""))
                .ThenBy(c => c.Rarity)
                .ThenBy(c => c.Cost.BaseAmount)
                .ThenBy(c => c.Name)
                .Select(InitCardInLibraryButton)
                .ToList(), 
            x => {},
            false);
    }

    private Action<GameObject> InitCardInLibraryButton(CardType card)
    {
        void Init(GameObject gameObj) => gameObj.GetComponent<CardInLibraryButton>().InitInfoOnly(card);
        return Init;
    }
}