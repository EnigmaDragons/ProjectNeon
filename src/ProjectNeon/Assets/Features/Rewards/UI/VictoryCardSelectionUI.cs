using System;
using TMPro;
using UnityEngine;

public class VictoryCardSelectionUI : OnMessage<GetUserSelectedCard>
{
    [SerializeField] private GameObject view;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private BattleState state;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI creditsLabel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI clinicVouchersLabel;

    private bool _hasSelected = false;
    private Action<Maybe<Card>> _onSelectedCard = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();

    protected override void Execute(GetUserSelectedCard msg)
    {
        _hasSelected = false;
        _onSelectedCard = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(cardPresenter, optionsParent.transform)
                .Set(o, () => SelectCard(o)));
        creditsLabel.text = state.RewardCredits.ToString() + 0;
        clinicVouchersLabel.text = state.RewardClinicVouchers.ToString();
        view.SetActive(true);
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
