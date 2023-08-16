using System;
using TMPro;
using UnityEngine;

public class BattleCardSelectionUI : OnMessage<PresentCardSelection>
{
    [SerializeField] private GameObject view;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private float fadeInSeconds = 0.5f;

    private float _alpha;
    private bool _hasSelected = false;
    private Action<Card> _onSelected = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();

    private void Awake() => view.SetActive(false);

    private void Update()
    {
        if (_alpha >= 1 || _hasSelected)
            return;
        _alpha = Math.Min(1, _alpha + Time.deltaTime / fadeInSeconds);
        canvasGroup.alpha = _alpha;
    }
    
    protected override void Execute(PresentCardSelection msg)
    {
        _hasSelected = false;
        _onSelected = msg.Selectons.OnCardSelected;
        ClearView();
        msg.Selectons.CardSelectionOptions.ForEach(o =>
        {
            var cardP = Instantiate(cardPresenter, optionsParent.transform);
            cardP.Set(o, () => SelectCard(o));
            cardP.SetHoverAction(() => cardP.SetHoverHighlight(true), () => cardP.SetHoverHighlight(false));
        });
        _alpha = 0;
        canvasGroup.alpha = 0;
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