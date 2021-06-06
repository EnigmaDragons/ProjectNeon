using System;
using TMPro;
using UnityEngine;

public class BattleCardSelectionUI : OnMessage<PresentCardSelection>
{
    [SerializeField] private GameObject view;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject optionsParent;

    private bool _hasSelected = false;
    private Action<Card> _onSelected = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();

    private void Awake() => view.SetActive(false);
    
    protected override void Execute(PresentCardSelection msg)
    {
        _hasSelected = false;
        _onSelected = msg.Selectons.OnCardSelected;
        ClearView();
        msg.Selectons.CardSelectionOptions.ForEach(o =>
            Instantiate(cardPresenter, optionsParent.transform)
                .Set(o, () => SelectCard(o)));
        view.SetActive(true);
    }
    
    private void SelectCard(Card c)
    {        
        if (_hasSelected)
            return;

        _hasSelected = true;
        Debug.Log($"Selected Card {c.Name}");
        ClearView();
        view.SetActive(false);
        _onSelected(c);
    }
}