using System;
using System.Linq;
using UnityEngine;

public class LibraryUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardInLibraryButton cardInLibraryButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private GameEvent heroSelected;
    [SerializeField] private Library library;
    [SerializeField] private DeckBuilderState state;

    private void OnEnable() => heroSelected.Subscribe(GenerateLibrary, this);
    private void OnDisable() => heroSelected.Unsubscribe(this);

    private void GenerateLibrary()
    {
        pageViewer.Init(cardInLibraryButtonTemplate.gameObject, emptyCard, library.UnlockedCards
            .Where(x => !x.LimitedToClass.IsPresent || x.LimitedToClass.Value == state.SelectedHeroesDeck.Hero.ClassName.Value)
            .Select(InitCardInLibraryButton)
            .ToList(), x => {});
    }

    private Action<GameObject> InitCardInLibraryButton(CardType card)
    {
        Action<GameObject> init = gameObj => gameObj.GetComponent<CardInLibraryButton>().Init(card);
        return init;
    }
}
