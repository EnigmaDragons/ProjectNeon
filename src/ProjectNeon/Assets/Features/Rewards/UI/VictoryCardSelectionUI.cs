using System;
using TMPro;
using UnityEngine;

public class VictoryCardSelectionUI : OnMessage<GetUserSelectedCardType, GetUserSelectedCard>
{
    [SerializeField] private GameObject view;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI creditsLabel;

    private bool _hasSelected = false;
    private Action<Maybe<CardTypeData>> _onSelectedType = _ => { };
    private Action<Maybe<Card>> _onSelectedCard = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();

    protected override void Execute(GetUserSelectedCardType msg)
    {
        _hasSelected = false;
        _onSelectedType = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(cardPresenter, optionsParent.transform)
                .Set(o, () => SelectCard(o)));
        creditsLabel.text = state.RewardCredits.ToString();
        view.SetActive(true);
    }

    protected override void Execute(GetUserSelectedCard msg)
    {
        _hasSelected = false;
        _onSelectedCard = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(cardPresenter, optionsParent.transform)
                .Set(o, () => SelectCard(o)));
        creditsLabel.text = state.RewardCredits.ToString();
        view.SetActive(true);
    }

    private void SelectCard(CardTypeData c)
    {        
        if (_hasSelected)
            return;

        _hasSelected = true;
        Debug.Log($"Selected Card {c.Name}");
        _onSelectedType(new Maybe<CardTypeData>(c));
        ClearView();
        Instantiate(cardPresenter, optionsParent.transform).Set(c, () => {});
    }
    
    private void SelectCard(Card c)
    {        
        if (_hasSelected)
            return;

        _hasSelected = true;
        Debug.Log($"Selected Card {c.Name}");
        _onSelectedCard(new Maybe<Card>(c));
        ClearView();
        Instantiate(cardPresenter, optionsParent.transform).Set(c, () => {});
    }
}
