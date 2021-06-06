using System;
using TMPro;
using UnityEngine;

public class VictoryCardSelectionUI : OnMessage<GetUserSelectedCard>
{
    [SerializeField] private GameObject view;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI creditsLabel;

    private bool _hasSelected = false;
    private Action<Maybe<CardTypeData>> _onSelected = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();

    protected override void Execute(GetUserSelectedCard msg)
    {
        _hasSelected = false;
        _onSelected = msg.OnSelected;
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
        _onSelected(new Maybe<CardTypeData>(c));
        ClearView();
        Instantiate(cardPresenter, optionsParent.transform).Set(c, () => {});
    }
}
