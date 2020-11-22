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
    
    protected override void Execute(GetUserSelectedCard msg)
    {
        _onSelected = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(cardPresenter, optionsParent.transform)
                .Set(o, () => SelectCard(o)));
        creditsLabel.text = state.RewardCredits.ToString();
        view.SetActive(true);
    }
    
    private Action<Maybe<CardType>> _onSelected = _ => { };
    
    private void ClearView() => optionsParent.DestroyAllChildren();
    private void SelectCard(CardType c)
    {
        Debug.Log($"Selected Card {c.Name}");
        FinishSelection(() => _onSelected(new Maybe<CardType>(c)));
        ClearView();
        Instantiate(cardPresenter, optionsParent.transform).Set(c, () => {});
    }

    private void FinishSelection(Action s) => s();
}
