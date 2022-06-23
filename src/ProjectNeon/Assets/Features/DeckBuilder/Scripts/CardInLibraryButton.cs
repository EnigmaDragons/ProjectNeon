﻿using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CardInLibraryButton : OnMessage<SetSuperFocusDeckBuilderControl>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI numCopiesLabel;
    [SerializeField] private GameObject superFocus;

    public CardInLibraryButton InitInfoOnly(Card card, Action action)
    {
        presenter.Set(card, action);
        numCopiesLabel.text = "";
        return this;
    }

    public CardInLibraryButton InitInfoOnly(CardTypeData card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "";
        return this;
    }

    public CardInLibraryButton Init(Card card, int numTotal, int numAvailable, bool superFocusEnabled)
    {
        presenter.Set(card, CreateCardAction(card, numAvailable));
        UpdateNumberText(numTotal, numAvailable);
        superFocus.SetActive(superFocusEnabled);
        return this;
    }
    
    public CardInLibraryButton Init(CardTypeData card, int numTotal, int numAvailable, bool superFocusEnabled)
    {
        presenter.Set(card, CreateCardAction(card, numAvailable));
        UpdateNumberText(numTotal, numAvailable);
        superFocus.SetActive(superFocusEnabled);
        return this;
    }

    private Action CreateCardAction(CardTypeData c, int numAvailable) =>
        numAvailable > 0 
        && state.SelectedHeroesDeck.Deck.Count(x => x.Id == c.Id) < 4
        && (state.SelectedHeroesDeck.Deck.GroupBy(x => x.Name).Count() < 12 || state.SelectedHeroesDeck.Deck.Any(x => x.Name == c.Name))
            ? (Action)(() => AddCard(c))
            : () => Message.Publish(new CardAddToDeckAttemptRejected(transform));
    
    private void UpdateNumberText(int numTotal, int numAvailable) 
        => numCopiesLabel.text = $"{numAvailable}/{numTotal}";

    public void SetNumberText(string value) => numCopiesLabel.text = value;
    
    public CardInLibraryButton InitBasic(CardTypeData card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "Basic";
        return this;
    }
    
    public CardInLibraryButton InitBasic(Card card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "Basic";
        return this;
    }

    public void AddCard(CardTypeData card)
    {
        if (card.Rarity == Rarity.Starter && card.Archetypes.None() && !CurrentAcademyData.Data.ReceivedNoticeAboutGeneralStarterCards)
        {
            CurrentAcademyData.Mutate(a => a.ReceivedNoticeAboutGeneralStarterCards = true);
            Message.Publish(new ShowInfoDialog("The general starter cards (Lite Charge Shield/Improvise/Scrounge) are playable, but usually weaker than your hero's card set. " +
                                               "Choose wisely! Be sure you want them.", "Got it!"));
        }
        state.SelectedHeroesDeck.Deck.Add(card);
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
        Message.Publish(new CardAddedToDeck(transform));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Message.Publish(new CardHovered(transform));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    protected override void Execute(SetSuperFocusDeckBuilderControl msg)
    {
        if (msg.Name == DeckBuilderControls.CardInLibrary)
            superFocus.SetActive(msg.Enabled);
    }
}
